/*
Måste kunna skicka förfrågnar till eclispe och monopoly,
vänta in dessa svaren
Sammanställa svaren till någon typ av rapport 
sedan kunna skicka den raporten som en serialiserad svar till en client
*/




using BoardgameReportsService;

var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddTransient<IReportService, ReportService>();

builder.Services.AddHttpClient<EclipseClient>(client =>
{
    client.BaseAddress = new Uri("http://eclipseservice");
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


