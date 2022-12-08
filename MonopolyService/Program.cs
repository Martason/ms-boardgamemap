
using Microsoft.EntityFrameworkCore;
using Monopoly.Model;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MonopolyDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTION")));
var app = builder.Build();

// Löser migrationer till databasen om förändring skett. Om databasen inte finns, skapas den. 
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MonopolyDbContext>();
    db.Database.Migrate();
}

app.MapGet("/monopoly", (MonopolyDbContext db) =>
{
    var allMonopolyGames = db.MonopolyGames;
    return (allMonopolyGames is null) ? Results.NoContent() : Results.Ok(allMonopolyGames);
});

app.MapPost("/monopoly", async (MonopolyGameInput input, MonopolyDbContext db) =>
{
    var game = new MonopolyGame
    {
        GameName = "Monopoly",
        DateOfGame = DateTime.Now,
        Town = input.town,
        WinningScore = 10,
        UserName = input.username
    };

    db.MonopolyGames.Add(game);
    await db.SaveChangesAsync();
    return Results.Ok(game);

});


app.MapDelete("/monopoly/{id}", async (string id, MonopolyDbContext db, HttpContext http) =>
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

public class MonopolyGameInput
{
    public string? town { get; set; }
    public string? username { get; set; }
};