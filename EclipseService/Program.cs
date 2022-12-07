
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Eclipse.Model;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<EclipseDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTION")));
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

// Löser migrationer till databasen om förändring skett. Om databasen inte finns, skapas den. 
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EclipseDbContext>();
    db.Database.Migrate();
}
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/eclipse", (EclipseDbContext db) =>
{
    var allEclipseGames = db.EclipseGames;
    return (allEclipseGames is null) ? Results.NoContent() : Results.Ok(allEclipseGames);
});

app.MapGet("/eclipse/{id}", async (string id, EclipseDbContext db) =>
{
    var gameGuid = new Guid(id);
    var game = await db.EclipseGames.FirstOrDefaultAsync(g => g.Id == gameGuid);

    return game;
});

app.MapPost("/eclipse/{town}", [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] async (string town, EclipseDbContext db, HttpContext http) =>
{
    if (string.IsNullOrWhiteSpace(town)) return Results.BadRequest();

    var _userId = http.User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
    if (_userId is null) return Results.BadRequest("Failed to authorize user");

    var game = new EclipseGame
    {
        GameName = "Eclipse",
        DateOfGame = DateTime.Now,
        Town = town,
        WinningScore = 10,
        UserName = _userId
    };

    db.EclipseGames.Add(game);
    await db.SaveChangesAsync();
    return Results.Created($"game/{town}", game);

});


app.MapDelete("/eclipse/{id}", [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] async (string id, EclipseDbContext db, HttpContext http) =>
{
    var UserName = http.User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
    if (UserName is null) return Results.BadRequest("Failed to authorize user");

    var gameGuid = new Guid(id);
    if (await db.EclipseGames.FirstOrDefaultAsync(g => g.Id == gameGuid && g.UserName == UserName) is EclipseGame game)
    {
        db.Remove(game);
        await db.SaveChangesAsync();
        return Results.Ok($"{game.GameName} with id {game.Id}, is successfully removed");

    }
    return Results.NotFound(id);
});

app.Run();

