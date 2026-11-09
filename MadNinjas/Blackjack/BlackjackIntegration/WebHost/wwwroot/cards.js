// Renders a hand without depending on game rules or changing game data.
const suits = {
  hearts: { symbol: '♥', name: 'hearts', red: true },
  diamonds: { symbol: '♦', name: 'diamonds', red: true },
  clubs: { symbol: '♣', name: 'clubs', red: false },
  spades: { symbol: '♠', name: 'spades', red: false }
};
const rankNames = { A: 'ace', J: 'jack', Q: 'queen', K: 'king' };

function element(tag, className, text) {
  const node = document.createElement(tag);
  node.className = className;
  if (text !== undefined) node.textContent = text;
  return node;
}

function createCard(card, hidden, index) {
  const node = element('div', 'playing-card');
  node.style.setProperty('--card-index', index);
  node.setAttribute('role', 'img');
  if (hidden) {
    node.classList.add('card-back');
    node.setAttribute('aria-label', 'Hidden card');
    const emblem = element('span', 'card-back-emblem', '♠');
    emblem.setAttribute('aria-hidden', 'true');
    node.append(emblem);
    return node;
  }
  const suit = suits[card.suit];
  if (!suit) throw new TypeError('Unknown card suit');
  node.classList.add(suit.red ? 'card-red' : 'card-black');
  node.setAttribute('aria-label', `${rankNames[card.rank] || card.rank} of ${suit.name}`);
  const face = element('div', 'card-face');
  face.setAttribute('aria-hidden', 'true');
  for (const position of ['top', 'bottom']) {
    const corner = element('span', `card-corner card-corner-${position}`);
    corner.append(element('span', 'card-rank', card.rank), element('span', 'card-suit', suit.symbol));
    face.append(corner);
  }
  face.append(element('span', 'card-center-pip', suit.symbol));
  node.append(face);
  return node;
}

export function renderHand(container, cards, { hideHole = false } = {}) {
  const fragment = document.createDocumentFragment();
  cards.forEach((card, index) => fragment.append(createCard(card, hideHole && index === 1, index)));
  container.replaceChildren(fragment);
  container.classList.toggle('hand-many', cards.length > 4);
}
