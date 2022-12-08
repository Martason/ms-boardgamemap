
/*
Måste kunna skicka förfrågnar till eclispe och monopoly,
vänta in dessa svaren
Sammanställa svaren till någon typ av rapport 
sedan kunna skicka den raporten som en serialiserad svar till en client
*/

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
    client.BaseAddress = new Uri("http://eclipseservice");
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
    if (eclipseGames == null)
    {
        return Results.NotFound("There are no EclipseGames");
    }
    var monopolyGames = await monopolyClient.GetMonopolyGames();
    if (monopolyGames == null)
    {
        return Results.NotFound("There are no MonopolyGames");
    }

    // TODO Slå ihop listorna och returnera denna istället

    return Results.Ok(monopolyGames);
});


app.MapPost("/eclipse/{town}", [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] async (string town, EclipseClient eclipseClient, HttpContext http) =>
{
    if (string.IsNullOrWhiteSpace(town)) return Results.BadRequest();

    var userName = http.User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
    if (userName is null) return Results.BadRequest("Failed to authorize user");

    var input = new GameInput
    {
        UserName = userName,
        Town = town
    };

    var postSucceeded = await eclipseClient.PostEclipseGame(input);
    if (postSucceeded) return Results.Ok(input);
    return Results.BadRequest();
});

app.MapPost("/monopoly/{town}", [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] async (string town, MonopolyClient monopolyClient, HttpContext http) =>
{
    if (string.IsNullOrWhiteSpace(town)) return Results.BadRequest();

    var userName = http.User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
    if (userName is null) return Results.BadRequest("Failed to authorize user");

    var input = new GameInput
    {
        UserName = userName,
        Town = town
    };

    var postSucceeded = await monopolyClient.PostMonopolyGame(input);
    if (postSucceeded) return Results.Ok(input);
    return Results.BadRequest();
});


app.Run();

public record LoginCredentials(string Password, string Email) { };
public record RegisterInput(string Password, string UserName, string Email) { };