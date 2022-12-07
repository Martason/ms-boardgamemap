/*
Måste kunna skicka förfrågnar till eclispe och monopoly,
vänta in dessa svaren
Sammanställa svaren till någon typ av rapport 
sedan kunna skicka den raporten som en serialiserad svar till en client
*/

using BoardgameReportsService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
// builder.Services.AddTransient<IReportService, ReportService>();
builder.Services.AddHttpClient<EclipseClient>(client =>
{
    client.BaseAddress = new Uri("http://eclipseservice");
});
builder.Services.AddHttpClient<MonopolyClient>(client =>
{
    client.BaseAddress = new Uri("http://monopolyservice");
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


app.MapPost("/eclipse/{town}", [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] async (string town, EclipseClient eclipseClient) =>
{
    var eclipsegame = await eclipseClient.PostEclipseGame(town);
    return Results.Ok(eclipsegame);
    // Funkar ej? Internal server error
});



/*
1. fixa alla endoints här i report service
    app.MapGet("Login")
    app.MapGet("Register")
    app.MapGet("/Monolpoly")
    app.MapGet("/Eclipse")
2. få till att kunna hämta data från monopoly och skapa en rapport
3. client class

*/

app.Run();


