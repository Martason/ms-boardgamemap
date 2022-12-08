
using BoardgameReportsService;
using BoardgameReportsService.Clients;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<EclipseClient>(client =>
{
    client.BaseAddress = new Uri("http://eclipseservice");
});
builder.Services.AddHttpClient<AuthenticationClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("AuthenticationUrl"));
});

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/allGames", async (EclipseClient eclipseClient) =>
{
    var eclipseGames = await eclipseClient.GetEclipseGames();
    if (eclipseGames == null)
    {
        return Results.NotFound("There are no EclipseGames");
    }
    return Results.Ok(eclipseGames);
});

app.MapPost("/eclipse/{town}", async (string town, EclipseClient eclipseClient) =>
{
    var eclipsegame = await eclipseClient.PostEclipseGame(town);
    return Results.Ok(eclipsegame);
});

app.MapPost("/register", async (RegisterInput userInput, AuthenticationClient authClient) =>
{
    var succeed = await authClient.Register(userInput);
    if (succeed)
    {
        return Results.Ok();
    }
    return Results.BadRequest();
});

app.MapPost("/login", async (LoginCredentials loginCredentials, AuthenticationClient authClient) =>
{
    var tokenstring = await authClient.Login(loginCredentials);
    if (tokenstring != null) return Results.Ok(tokenstring);
    return Results.Unauthorized();
});

app.Run();

public record LoginCredentials(string Password, string Email) { };
public record RegisterInput(string Password, string UserName, string Email) { };