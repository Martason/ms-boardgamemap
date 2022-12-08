

using BoardgameReportsService;
using BoardgameReportsService.Clients;
using BoardgameReportsService.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<EclipseClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("EclipseUrl"));
});
builder.Services.AddHttpClient<MonopolyClient>(client =>
{
    client.BaseAddress = new Uri("http://monopolyservice");
});
builder.Services.AddHttpClient<AuthenticationClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("AuthenticationUrl"));
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateActor = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
    options.SaveToken = true;
});
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

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


app.MapGet("/allGames", async (EclipseClient eclipseClient, MonopolyClient monopolyClient) =>
{
    var eclipseGames = await eclipseClient.GetEclipseGames();
    //var monopolyGames = await monopolyClient.GetMonopolyGames();

    var allGames = new List<Game>();
    allGames.AddRange(eclipseGames);
    //allGames.AddRange(monopolyGames);

    if (allGames == null)
    {
        return Results.NotFound("There are no EclipseGames");
    }
    return Results.Ok(allGames);
});


app.MapPost("/eclipse/{town}", [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] async (string town, EclipseClient eclipseClient, HttpContext http) =>
{
    if (string.IsNullOrWhiteSpace(town)) return Results.BadRequest();

    var userName = http.User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
    if (userName is null) return Results.BadRequest("Failed to authorize user");

    var input = new GameInput
    {
        UserName = userName,
        Town = town,

    };

    var postSucceeded = await eclipseClient.PostEclipseGame(input);
    if (postSucceeded) return Results.Ok(input);
    return Results.BadRequest();
});




app.Run();

public record LoginCredentials(string Password, string Email) { };
public record RegisterInput(string Password, string UserName, string Email) { };