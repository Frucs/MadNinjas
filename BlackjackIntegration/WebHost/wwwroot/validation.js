// Returns a valid bet or explains exactly what the player needs to correct.
export function validateBet(raw, balance) {
    const text = String(raw ?? '').trim();
    if (!text) throw new Error('Enter a bet before dealing the cards.');
    const bet = Number(text);
    if (!Number.isFinite(bet) || !Number.isInteger(bet)) {
        throw new Error('Enter a whole number, such as 10. Letters and decimals are not accepted.');
    }
    if (bet < 1) throw new Error('The minimum bet is 1 kr.');
    if (bet > balance) throw new Error(`Your bet cannot exceed your balance of ${balance} kr.`);
    if (!/^\d+$/.test(text) || !Number.isSafeInteger(bet)) {
        throw new Error('Use digits only, such as 10.');
    }
    return bet;
}
