// Remove any earlier celebration before a new round starts.
export function clearCelebration() {
    document.querySelectorAll('.confetti-layer').forEach(layer => layer.remove());
}

// Shows a brief confetti burst and removes the animation layer afterward.
export function celebrateWin() {
    clearCelebration();
    if (window.matchMedia('(prefers-reduced-motion: reduce)').matches) return;
    const layer = document.createElement('div');
    layer.className = 'confetti-layer';
    layer.setAttribute('aria-hidden', 'true');
    const colors = ['#e8c775', '#f5e8bb', '#70cf9b', '#eaf9ee', '#e34b4b'];
    for (let i = 0; i < 54; i++) {
        const piece = document.createElement('span');
        piece.className = 'confetti-piece';
        const side = i % 2;
        piece.style.left = side ? '82%' : '18%';
        piece.style.setProperty('--dx', `${(side ? -1 : 1) * (40 + Math.random() * 360)}px`);
        piece.style.setProperty('--dy', `${-140 - Math.random() * 280}px`);
        piece.style.setProperty('--spin', `${Math.random() * 1000 - 500}deg`);
        piece.style.setProperty('--delay', `${Math.random() * 120}ms`);
        piece.style.backgroundColor = colors[i % colors.length];
        layer.append(piece);
    }
    document.body.append(layer);
    window.setTimeout(() => layer.remove(), 1900);
}
