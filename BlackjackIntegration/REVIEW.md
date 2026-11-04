# Integration review

## Ownership and provenance

This change adds a separate `BlackjackIntegration` folder. It does not replace or modify files in the existing `Blackjack` console project. Group baselines are recorded in SOURCE-BASELINES.json.

- Johan: Card and Deck copied; the web copy of Deck stops printing drawn cards.
- Musse: existing Game fields and CalculateScore reused. The web copy is partial so Game.Web can supply the missing HTTP-driven round flow. This is new integration work for Musse to review, not a claim that his original round flow was complete.
- Derin: BalanceManager reused with net settlement once per round. ScoreBoard and ScoreEntry reused; the integration copy of ScoreBoard filters damaged entries and saves through a temporary file.
- Andreas: green frontend, forms, card rendering, results, leaderboard and effects. A server adapter replaces the standalone JavaScript demonstration.

## Review findings addressed

- A round may settle before a failed save or lost response reaches the client. Failed mutations now reconcile with GET state without repeating the payment.
- Expired sessions reset cached state and permit a new game. Transport failures expose reconnect.
- Win confetti is cleared before the next round so it cannot visually imply that a subsequent loss won.
- Null or invalid JSON rows are removed before sorting and saving. Writes replace the old file after complete serialization.
- End/save requests are idempotent and saving status is returned explicitly; retry remains available after storage failure.

## Remaining review

No exact failing round was supplied for the reported winner mismatch. Independent scoring and outcome checks found no inversion. The group must review the new round flow and API before merge. This automated review is not personal approval from Musse or Derin. The server is a local classroom host, not an internet production service. Active sessions are in memory.
