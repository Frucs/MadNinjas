"""Exercise the real host using isolated temporary JSON storage and cookie sessions."""
import concurrent.futures
import http.cookiejar
import json
import pathlib
import shutil
import subprocess
import tempfile
import time
import urllib.error
import urllib.request

root = pathlib.Path(__file__).resolve().parents[1]
with tempfile.TemporaryDirectory(prefix='blackjack-http-') as storage:
    log = tempfile.TemporaryFile()
    process = subprocess.Popen([
        shutil.which('dotnet'), 'run', '--project', str(root / 'WebHost/BlackjackWeb.csproj'),
        '--no-build', '--', '--urls', 'http://127.0.0.1:5182',
        '--Leaderboard:Directory', storage
    ], stdout=log, stderr=log)
    try:
        base = 'http://127.0.0.1:5182'
        def client():
            return urllib.request.build_opener(urllib.request.HTTPCookieProcessor(http.cookiejar.CookieJar()), urllib.request.ProxyHandler({}))
        def request(opener, path, body=None):
            data = None if body is None else json.dumps(body).encode()
            req = urllib.request.Request(base + path, data=data, headers={'Content-Type':'application/json'})
            try:
                with opener.open(req, timeout=5) as response:
                    return response.status, json.load(response)
            except urllib.error.HTTPError as error:
                return error.code, json.load(error)
        first, second = client(), client()
        for attempt in range(100):
            try:
                if request(first, '/api/scores')[0] == 200: break
            except (urllib.error.URLError, TimeoutError): time.sleep(.1)
        else: raise AssertionError('Host did not start')
        assert request(first, '/api/state')[0] == 404
        assert request(first, '/api/session', {'name':'Player one'})[1]['balance'] == 100
        assert request(second, '/api/session', {'name':'Player two'})[1]['balance'] == 100
        assert request(first, '/api/deal', {'bet':101})[0] == 400
        status, state = request(first, '/api/deal', {'bet':20})
        assert status == 200
        if state['phase'] == 'playing':
            assert state['dealerCards'][1] == {'rank':'?', 'suit':'hidden'}
            assert state['dealerTotal'] is None
            assert request(first, '/api/end', {})[0] == 400
            assert request(first, '/api/deal', {'bet':20})[0] == 400
            status, state = request(first, '/api/stand', {})
        assert state['phase'] == 'resolved'
        balance = state['balance']
        assert request(second, '/api/state')[1]['balance'] == 100
        assert request(second, '/api/state')[1]['rounds'] == 0
        assert request(first, '/api/stand', {})[0] == 400
        assert request(first, '/api/state')[1]['balance'] == balance
        # Force a write error without touching the real leaderboard.
        blocker = pathlib.Path(storage) / 'json1.json.tmp'
        blocker.mkdir()
        assert request(first, '/api/end', {})[0] == 503
        failed = request(first, '/api/state')[1]
        assert failed['phase'] == 'ended' and failed['balance'] == balance
        assert failed['resultSaved'] is False
        blocker.rmdir()
        with concurrent.futures.ThreadPoolExecutor(max_workers=2) as pool:
            results = list(pool.map(lambda _: request(first, '/api/end', {}), range(2)))
        assert all(status == 200 for status, _ in results)
        assert request(first, '/api/state')[1]['resultSaved'] is True
        rows = request(first, '/api/scores')[1]
        assert len(rows) == 1 and rows[0]['name'] == 'Player one'
        assert rows[0]['balance'] == balance and rows[0]['rounds'] == 1
        saved = json.loads((pathlib.Path(storage)/'json1.json').read_text())
        assert len(saved) == 1
        # Restart the process: results persist; active in-memory sessions expire.
        process.terminate(); process.wait(timeout=10)
        process = subprocess.Popen([
            shutil.which('dotnet'), 'run', '--project', str(root/'WebHost/BlackjackWeb.csproj'),
            '--no-build', '--', '--urls', base, '--Leaderboard:Directory', storage
        ], stdout=log, stderr=log)
        for attempt in range(100):
            try:
                if request(first, '/api/scores')[0] == 200: break
            except (urllib.error.URLError, TimeoutError): time.sleep(.1)
        assert request(first, '/api/state')[0] == 404
        assert request(first, '/api/scores')[1] == rows
        print('PASS: independent sessions, server validation, hidden-card privacy, phase guards, failed-save recovery, concurrent save idempotency, and JSON persistence after restart.')
    finally:
        process.terminate()
        try: process.wait(timeout=10)
        except subprocess.TimeoutExpired: process.kill()
        log.close()
