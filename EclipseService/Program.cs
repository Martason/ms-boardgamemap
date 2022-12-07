
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Eclipse.Model;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using EclipseService.Services;

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
    options.SaveToken = true;
});
builder.Services.AddGrpc();
builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EclipseDbContext>();
    db.Database.Migrate();
}
app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<GameService>();

app.MapGet("/", (EclipseDbContext db) =>
{
    var allEclipseGames = db.EclipseGames;
    return (allEclipseGames is null) ? Results.NoContent() : Results.Ok(allEclipseGames);
});


app.MapPost("/{town}", [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] async (string town, EclipseDbContext db, HttpContext http) =>
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


app.MapDelete("/{id}", [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] async (string id, EclipseDbContext db, HttpContext http) =>
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

