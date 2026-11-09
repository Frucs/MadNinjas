using System.Collections.Concurrent;
using Blackjack;

var builder = WebApplication.CreateBuilder(args);
// Default to this computer only; --urls can enable sharing on the local network.
if (string.IsNullOrWhiteSpace(builder.Configuration["urls"]))
    builder.WebHost.UseUrls("http://127.0.0.1:5180");
var app = builder.Build();
var dataDirectory = builder.Configuration["Leaderboard:Directory"] ?? Path.Combine(app.Environment.ContentRootPath, "App_Data");
Directory.CreateDirectory(dataDirectory);
// Derin's ScoreBoard uses a relative path. Keep its JSON in isolated application data.
Directory.SetCurrentDirectory(dataDirectory);
var board = new ScoreBoard();
var boardGate = new object();
var sessions = new ConcurrentDictionary<string, WebSession>();
const string cookieName = "madninjas.blackjack.session";

app.Use(async (context, next) =>
{
    // Only same-origin browser requests may change game state.
    if (HttpMethods.IsPost(context.Request.Method))
    {
        var origin = context.Request.Headers.Origin.ToString();
        if (origin.Length > 0 && origin != $"{context.Request.Scheme}://{context.Request.Host}")
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsJsonAsync(new { error = "Use the game page on this server." });
            return;
        }
    }
    try { await next(); }
    catch (Exception error) when (error is ArgumentException or InvalidOperationException)
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsJsonAsync(new { error = error.Message });
    }
    catch (Exception error) when (error is IOException or UnauthorizedAccessException)
    {
        app.Logger.LogError(error, "Could not access leaderboard storage");
        context.Response.StatusCode = 503;
        await context.Response.WriteAsJsonAsync(new { error = "The leaderboard could not be saved. Please try ending the session again." });
    }
});
app.UseDefaultFiles();
app.UseStaticFiles(new StaticFileOptions { OnPrepareResponse = ctx => ctx.Context.Response.Headers.CacheControl = "no-cache" });

WebSession? Find(HttpContext context) => context.Request.Cookies.TryGetValue(cookieName, out var id) && sessions.TryGetValue(id, out var session) ? session : null;
void SaveResult(WebSession session)
{
    var state = session.Game.WebState();
    if (session.Saved || state.Rounds == 0) return;
    lock (boardGate) board.AddResult(state.PlayerName, state.Balance, state.Rounds);
    session.Saved = true;
}
WebGameState View(WebSession session) => session.Game.WebState() with { ResultSaved = session.Saved };
IResult Mutate(HttpContext context, Func<Game, object> change)
{
    var session = Find(context);
    if (session is null) return Results.Json(new { error = "Your session has expired. Take a seat to start again." }, statusCode: 404);
    lock (session.Gate)
    {
        session.LastUsed = DateTimeOffset.UtcNow;
        var result = change(session.Game);
        var state = session.Game.WebState();
        if (state.Phase == "resolved" && state.Balance == 0) SaveResult(session);
        return Results.Ok(View(session));
    }
}
app.MapGet("/api/state", (HttpContext context) =>
{
    var session = Find(context);
    if (session is null) return Results.NotFound(new { error = "No active session." });
    lock (session.Gate) return Results.Ok(View(session));
});
app.MapPost("/api/session", (SessionRequest request, HttpContext context) =>
{
    var previous = Find(context);
    if (previous is not null)
    {
        lock (previous.Gate)
        {
            var prior = previous.Game.WebState();
            if (prior.Phase == "playing" || (prior.Phase != "ended" && prior.Balance > 0 && prior.Rounds > 0))
                return Results.BadRequest(new { error = "Finish your current session before starting another." });
            if (prior.Rounds > 0) SaveResult(previous);
        }
    }
    var game = new Game();
    var state = game.WebStart(request.Name ?? "");
    var id = Guid.NewGuid().ToString("N");
    sessions[id] = new WebSession(game);
    if (context.Request.Cookies.TryGetValue(cookieName, out var oldId)) sessions.TryRemove(oldId, out _);
    context.Response.Cookies.Append(cookieName, id, new CookieOptions { HttpOnly = true, SameSite = SameSiteMode.Strict, IsEssential = true, Path = "/" });
    return Results.Ok(state);
});
app.MapPost("/api/deal", (BetRequest request, HttpContext context) => Mutate(context, game => game.WebDeal(request.Bet)));
app.MapPost("/api/hit", (HttpContext context) => Mutate(context, game => game.WebHit()));
app.MapPost("/api/stand", (HttpContext context) => Mutate(context, game => game.WebStand()));
app.MapPost("/api/end", (HttpContext context) =>
{
    var session = Find(context);
    if (session is null) return Results.NotFound(new { error = "No active session." });
    lock (session.Gate)
    {
        var state = session.Game.WebState();
        if (state.Phase != "ended") state = session.Game.WebEnd();
        SaveResult(session);
        return Results.Ok(View(session));
    }
});
app.MapGet("/api/scores", () =>
{
    lock (boardGate)
    {
        var rows = board.Load().Where(x => x is not null && !string.IsNullOrWhiteSpace(x.Name) && x.RoundsPlayed > 0)
            .OrderByDescending(x => x.FinalBalance).Take(5)
            .Select(x => new { name = x.Name, balance = x.FinalBalance, rounds = x.RoundsPlayed }).ToArray();
        return Results.Ok(rows);
    }
});
app.Run();
record SessionRequest(string? Name);
record BetRequest(int Bet);
sealed class WebSession(Game game)
{
    public Game Game { get; } = game;
    public object Gate { get; } = new();
    public bool Saved { get; set; }
    public DateTimeOffset LastUsed { get; set; } = DateTimeOffset.UtcNow;
}
