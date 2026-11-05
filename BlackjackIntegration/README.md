# MadNinjas Blackjack integration

This folder is a separate integration snapshot. It connects the green browser frontend to a C# web host and reuses the group’s existing classes. No files in the original group repository have been changed, pushed or merged by this integration work.

## Source baseline

The snapshot is based on main commit `8e31c98` and Musse’s work at commit `5bfdbf6`.

- Johan’s `Card` and `Deck` provide the cards and deck.
- Derin’s `BalanceManager`, `ScoreBoard` and `ScoreEntry` provide balance and saved results.
- Musse’s `Game` provides its existing fields and score calculation.
- `Game.Web` is a partial-class extension that implements the missing round flow for the web host. Musse’s game flow was unfinished at the baseline, so this extension needs the group’s review before integration into the shared project.

The integration copy changes `Game` by adding the `partial` keyword. A console print is removed from the integration copy of `Deck` so drawing cards does not write game output to the web server console. The integration copy of ScoreBoard also filters invalid JSON rows and writes results via an atomic file replacement. These changes apply only to this snapshot; the original files remain untouched.

## Requirements

Install the .NET 10 SDK on the computer that will run the host. The host serves both the frontend and API, so Node.js, npm and Python are not required to run this integration.

## Run locally

Open a terminal in this `BlackjackIntegration` folder and run these commands one at a time:

```sh
cd WebHost
```

This moves the terminal into the C# web host project folder.

```sh
dotnet run
```

This builds and starts the local web host. Open http://localhost:5180 in a browser. Press Ctrl+C in the terminal to stop the host.

## Game state and stored results

C# runs the game rules on the server, including card values, ace handling, bets, dealer actions and round outcomes. The dealer’s hidden card is not sent to the browser before it is revealed.

An HttpOnly cookie identifies the current server-memory session. Active sessions are held in memory, so restarting the server resets them. Completed session results are stored as JSON under `WebHost/App_Data` and persist after a restart. This folder is isolated from the group’s existing JSON files; those original files remain untouched.

This local school-project integration uses play money and does not provide production hosting or account management.

## Review before merging

1. Review the source baseline and reused classes together with Johan, Musse and Derin.
2. Agree on the ownership and final shape of `Game.Web`, especially how its round flow should relate to Musse’s continuing work.
3. Compare the snapshot with the latest group branch before copying any files; the group may have made further changes since the baseline commits.
4. Build and test the full game flow, error handling, hidden-card behavior, payouts and JSON persistence on Mac and PC.
5. Bring approved changes into a separate group-repository branch and review the pull request before merging.

Do not replace the group’s repository with this snapshot. The group’s console project remains separate.

## Verification

- `dotnet run --project Checks/Checks.csproj` runs 17 integration checks, including an independent scoring reference for 30,940 hands and 20,000 seeded rounds.
- `node --test Checks/api-adapter.test.mjs` runs four browser-adapter recovery checks (Node is optional for verification, not needed to run the game).
- After building WebHost, `python3 Checks/http-check.py` tests actual HTTP sessions, hidden cards, invalid bets, concurrent saves, write-failure recovery and leaderboard persistence across restart. It uses temporary data and port 5182. Python is optional for verification.
- Desktop and 390-pixel mobile browser flows were checked separately with isolated JSON data.

The reported incorrect winner could not be reproduced without the original round’s cards. The independent reference agreed with all 20,000 tested outcomes and balances. Review did find and fix stale browser state after failed server responses and lingering confetti from the preceding round. The adapter now refreshes authoritative state after a failed mutation, clears expired sessions, and provides reconnect. Result saving is reported only after the host confirms it.

The integration copies initialize ScoreEntry.Name and accept nullable console input in BalanceManager; the build has no nullable warnings. Server restarts reset active games but preserve saved JSON results.

## Share on the same Wi-Fi

Run `dotnet run -- --urls http://0.0.0.0:5180` from WebHost to listen on the local network. This explicitly enables access from other devices; the default remains loopback-only. Share `http://YOUR_LOCAL_IP:5180`. The host computer must remain awake and connected. Some school networks block connections between devices. No internet publication or router port forwarding is configured.
