
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Eclipse.Model;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<EclipseDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("EclipseConnection")));
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

});
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", (EclipseDbContext db) =>
{
    var allEclipseGames = db.EclipseGames;
    return (allEclipseGames is null) ? Results.NoContent() : Results.Ok(allEclipseGames);
});


app.MapPost("/{town}", [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] async (string town, EclipseDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(town)) return Results.BadRequest();

    var game = new EclipseGame
    {
        GameName = "Eclipse",
        DateOfGame = DateTime.Now,
        Town = town,
        WinningScore = 10,
    };

    db.EclipseGames.Add(game);
    await db.SaveChangesAsync();
    return Results.Created($"game/{town}", game);

});

// Does not work yet. Add Authorization 
app.MapDelete("/{gameId}", async (string id, EclipseDbContext db) =>
{
    var guid = new Guid(id);
    if (await db.EclipseGames.FindAsync(guid) is EclipseGame game)
    {
        db.Remove(game);
        await db.SaveChangesAsync();
        return Results.Ok($"{game.GameName} with id {game.Id}, is successfully removed");

    }
    return Results.NotFound(id);
});

app.Run();

