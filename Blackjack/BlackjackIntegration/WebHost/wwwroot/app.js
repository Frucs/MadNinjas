import { renderHand } from './cards.js';
import { validateBet } from './validation.js';
import { celebrateWin, clearCelebration } from './confetti.js';
import { ApiAdapter } from './api-adapter.js';

// The server owns all game state and saves completed sessions to JSON.
const game = new ApiAdapter();
const $ = id => document.getElementById(id);
let sessionStarted = false;
let celebratedRound = 0;
let busy = false;
let connected = false;
const money = value => `${value.toLocaleString('en-GB')} kr`;

function render() {
    const s = game.getState();
    const playing = s.phase === 'playing';
    sessionStarted = Boolean(s.playerName);
    if (s.phase === 'resolved' && ['win', 'blackjack'].includes(s.outcome) && celebratedRound !== s.rounds) {
        celebratedRound = s.rounds;
        celebrateWin();
    }
    renderHand($('dealer-cards'), s.dealerCards, { hideHole: s.hideDealerHole });
    renderHand($('player-cards'), s.playerCards);
    $('player-name').textContent = s.playerName || 'Your seat';
    $('player-score').textContent = s.playerCards.length ? s.playerTotal : '—';
    $('dealer-score').textContent = s.hideDealerHole ? `${s.dealerVisibleTotal} + ?` : s.dealerCards.length ? s.dealerTotal : '—';
    $('balance').textContent = money(s.balance);
    $('session-result').textContent = sessionStarted ? `Session result: ${s.balance - 100 > 0 ? '+' : ''}${money(s.balance - 100)}${playing ? ' (bet deducted)' : ''}` : 'Starting balance when you take a seat';
    if (s.outcome && s.phase !== 'playing') {
        const profit = s.outcome === 'blackjack' ? Math.floor(s.bet * 1.5) : s.outcome === 'win' ? s.bet : s.outcome === 'loss' ? -s.bet : 0;
        $('round-summary').textContent = `Balance before round: ${money(s.balance - profit)} · Result: ${profit > 0 ? '+' : ''}${money(profit)} · New balance: ${money(s.balance)}`;
    } else $('round-summary').textContent = '';
    $('round-count').textContent = `${s.rounds} rounds played`;
    $('bet-range').textContent = s.balance ? `1–${s.balance} kr` : 'No funds remaining';
    $('bet-input').max = Math.max(1, s.balance);
    $('table-bet').textContent = s.bet ? money(s.bet) : 'YOUR BET';
    $('round-message').textContent = sessionStarted ? s.message : 'Welcome to the table. Take a seat to begin.';
    $('round-message').dataset.tone = s.tone;
    if (s.phase === 'ended') $('round-message').textContent = `Thanks for playing, ${s.playerName}! Final balance: ${money(s.balance)}. ${s.rounds > 0 ? (s.resultSaved ? 'Your completed result is saved on the server. ' : 'The result is not saved yet. Retry End session. ') : ''}Take a seat to start a new session.`;
    if (sessionStarted && s.balance === 0 && s.phase !== 'playing') $('round-message').textContent = `You are out of chips. ${s.resultSaved ? 'Your result is saved on the server.' : 'The result is not saved yet. Retry End session.'} Take a seat to start a new session.`;
    $('hit-button').disabled = busy || !connected || !playing;
    $('stand-button').disabled = busy || !connected || !playing;
    $('bet-input').disabled = busy || playing || !connected;
    document.querySelectorAll('[data-bet]').forEach(button => {
        button.disabled = busy || playing || !connected;
        button.classList.toggle('is-active', button.dataset.bet === $('bet-input').value);
    });
    $('deal-button').disabled = busy || playing || !connected;
    $('deal-button').textContent = !sessionStarted || s.phase === 'ended' || s.balance === 0 ? 'Take a seat ↗' : s.phase === 'resolved' ? 'Next round ↗' : 'Deal cards ↗';
    $('end-button').disabled = busy || !connected || !sessionStarted || playing || (s.phase === 'ended' && (s.resultSaved || s.rounds === 0));
    $('name-form').querySelector('[type="submit"]').disabled = busy || !connected;
    $('reconnect-button').hidden = connected;
    document.querySelector('.demo-badge').textContent = connected ? 'C# CONNECTED' : 'SERVER OFFLINE';
    document.querySelector('.footer-note span').textContent = connected ? 'C# game server · Shared JSON leaderboard' : 'Start the C# WebHost to connect';
}

function showError(error) {
    $('bet-error').textContent = error.message || 'Something went wrong. Please try again.';
}

// Disable game controls until each server operation finishes.
async function action(fn, onError = showError) {
    if (busy) return;
    busy = true;
    $('bet-error').textContent = '';
    $('bet-input').removeAttribute('aria-invalid');
    render();
    try { await fn(); }
    catch (error) { onError(error); }
    finally { connected = game.connected; busy = false; render(); }
}

async function showScores() {
    if (busy) return;
    $('score-list').replaceChildren();
    $('scores-empty').hidden = false;
    $('scores-empty').textContent = 'Loading server results…';
    $('scores-dialog').showModal();
    await action(async () => {
        const rows = await game.loadScores();
        $('scores-empty').hidden = rows.length > 0;
        $('scores-empty').textContent = 'No completed sessions yet. Your place on the leaderboard is waiting.';
        rows.forEach((row, index) => {
            const li = document.createElement('li');
            const name = document.createElement('span');
            name.textContent = `${index + 1}. ${row.name}`;
            const value = document.createElement('strong');
            value.textContent = money(row.balance);
            const rounds = document.createElement('small');
            rounds.textContent = `${row.rounds} rounds`;
            li.append(name, value, rounds);
            $('score-list').append(li);
        });
    }, error => { $('scores-empty').textContent = error.message; });
}

$('deal-button').addEventListener('click', () => {
    if (busy) return;
    const s = game.getState();
    if (!sessionStarted || s.phase === 'ended' || s.balance === 0) {
        $('name-error').textContent = '';
        $('welcome-dialog').showModal();
        return;
    }
    action(async () => {
        const bet = validateBet($('bet-input').value, s.balance);
        clearCelebration();
        await game.deal(bet);
    });
});
$('name-form').addEventListener('submit', event => {
    event.preventDefault();
    if (busy) return;
    const name = $('name-input').value.trim();
    if (!name) { $('name-error').textContent = 'Your name cannot be empty.'; $('name-input').focus(); return; }
    $('name-error').textContent = '';
    action(async () => {
        clearCelebration();
        await game.startSession(name);
        celebratedRound = 0;
        $('bet-input').value = '20';
        $('welcome-dialog').close();
    }, error => { $('name-error').textContent = error.message; $('name-input').focus(); });
});
$('reconnect-button').addEventListener('click', () => action(async () => { await game.load(); }));
$('hit-button').addEventListener('click', () => action(() => game.hit()));
$('stand-button').addEventListener('click', () => action(() => game.stand()));
$('end-button').addEventListener('click', () => action(() => game.endSession()));
$('rules-button').addEventListener('click', () => $('rules-dialog').showModal());
$('scores-button').addEventListener('click', showScores);
document.querySelectorAll('[data-close]').forEach(button => button.addEventListener('click', () => {
    if (!busy) button.closest('dialog').close();
}));
$('welcome-dialog').addEventListener('cancel', event => { if (busy) event.preventDefault(); });
document.querySelectorAll('[data-bet]').forEach(button => button.addEventListener('click', () => {
    if (busy) return;
    $('bet-input').value = button.dataset.bet === 'all' ? game.getState().balance : button.dataset.bet;
    $('bet-error').textContent = '';
    render();
}));
$('bet-input').addEventListener('input', () => { $('bet-error').textContent = ''; $('bet-input').removeAttribute('aria-invalid'); });

render();
await action(async () => {
    const state = await game.load();
    // Restoring an existing winning round must not replay the celebration.
    celebratedRound = state.rounds;
    connected = true;
});
