using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<UserDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTION")));


var app = builder.Build();

// Löser migrationer till databasen om förändring skett. Om databasen inte finns, skapas den. 
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();

app.MapPost("/register", async (UserModel user, UserDbContext dbContext) =>
{
    dbContext.Users.Add(user);
    await dbContext.SaveChangesAsync();

    return Results.Ok();
});

app.MapPost("/login", async (LoginCredentials loginCredentials, UserDbContext dbContext) =>
{
    var user = await dbContext.Users.FirstOrDefaultAsync(user => user.Email.Equals(loginCredentials.Email) && user.Password.Equals(loginCredentials.Password));
    if (user == null) return Results.BadRequest("Bad credentials");

    var secretKey = builder.Configuration["Jwt:Key"];

    var claims = new[]
    {
        new Claim(ClaimTypes.NameIdentifier, user.Username),
        new Claim(ClaimTypes.Email, user.Email),
    };

    var token = new JwtSecurityToken
    (
        issuer: builder.Configuration["Jwt:Issuer"],
        audience: builder.Configuration["Jwt:Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(5),
        notBefore: DateTime.UtcNow,
        signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        SecurityAlgorithms.HmacSha256)
    );

    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

    return Results.Ok(tokenString);
});

app.Run();

public record LoginCredentials(string Password, string Email) { };



