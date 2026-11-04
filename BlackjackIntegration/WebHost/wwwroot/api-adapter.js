// Keeps the browser interface separate from the C# game and JSON persistence.
const initialState = () => ({
    phase: 'idle', playerName: '', balance: 100, bet: 0, rounds: 0,
    playerCards: [], dealerCards: [], playerTotal: 0, dealerTotal: 0,
    dealerVisibleTotal: 0, hideDealerHole: false,
    message: 'Choose your bet and deal the cards.', tone: 'neutral', outcome: null
});

export class ApiAdapter {
    constructor() { this.state = initialState(); this.connected = false; }

    async request(path, { method = 'GET', body, allowMissing = false } = {}) {
        let response;
        try {
            response = await fetch(path, {
                method, credentials: 'same-origin', cache: 'no-store',
                headers: body === undefined ? { Accept: 'application/json' } : { Accept: 'application/json', 'Content-Type': 'application/json' },
                ...(body === undefined ? {} : { body: JSON.stringify(body) })
            });
        } catch {
            this.connected = false;
            throw new Error('Cannot connect to the C# server. Start the WebHost and reopen this page at its local address.');
        }
        this.connected = true;
        if (allowMissing && response.status === 404) return null;
        let data;
        try { data = await response.json(); }
        catch { throw new Error('The C# server returned an unreadable response. Check the WebHost terminal and try again.'); }
        if (!response.ok) throw new Error(data.error || `The C# server could not complete this request (${response.status}).`);
        return data;
    }

    async update(path, body) {
        try {
            const state = await this.request(path, { method: 'POST', body });
            this.state = state;
            return this.getState();
        } catch (error) {
            // A payment may already have settled even when its response failed.
            // Reconcile with authoritative state; never replay the mutating request.
            try { await this.load(); } catch { /* Preserve the original error. */ }
            throw error;
        }
    }

    async load() {
        const state = await this.request('/api/state', { allowMissing: true });
        this.state = state || initialState();
        return this.getState();
    }
    startSession(name) { return this.update('/api/session', { name }); }
    deal(bet) { return this.update('/api/deal', { bet }); }
    hit() { return this.update('/api/hit', {}); }
    stand() { return this.update('/api/stand', {}); }
    endSession() { return this.update('/api/end', {}); }
    loadScores() { return this.request('/api/scores'); }
    getState() { return structuredClone(this.state); }
}
