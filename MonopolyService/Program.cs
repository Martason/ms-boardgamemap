
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Monopoly.Model;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MonopolyDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("MonopolyConnection")));
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

app.MapGet("/", (MonopolyDbContext db) =>
{
    var allMonopolyGames = db.MonopolyGames;
    return (allMonopolyGames is null) ? Results.NoContent() : Results.Ok(allMonopolyGames);
});

app.MapPost("/{town}", [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] async (string town, MonopolyDbContext db, HttpContext http) =>
{
    if (string.IsNullOrWhiteSpace(town)) return Results.BadRequest();

    var _userId = http.User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
    if (_userId is null) return Results.BadRequest("Failed to authorize user");

    var game = new MonopolyGame
    {
        GameName = "Monopoly",
        DateOfGame = DateTime.Now,
        Town = town,
        WinningScore = 10,
        UserName = _userId
    };

    db.MonopolyGames.Add(game);
    await db.SaveChangesAsync();
    return Results.Created($"game/{town}", game);

});


app.MapDelete("/{id}", [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] async (string id, MonopolyDbContext db, HttpContext http) =>
{
    var userName = http.User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
    if (userName is null) return Results.BadRequest("Failed to authorize user");

    var gameGuid = new Guid(id);
    if (await db.MonopolyGames.FirstOrDefaultAsync(g => g.Id == gameGuid && g.UserName == userName) is MonopolyGame game)
    {
        db.Remove(game);
        await db.SaveChangesAsync();
        return Results.Ok($"{game.GameName} with id {game.Id}, is successfully removed");

    }
    return Results.NotFound(id);
});

app.Run();
