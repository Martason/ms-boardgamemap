/*
Måste kunna skicka förfrågnar till eclispe och monopoly,
vänta in dessa svaren
Sammanställa svaren till någon typ av rapport 
sedan kunna skicka den raporten som en serialiserad svar till en client
*/

using BoardgameReportsService.BusinessLogic;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddTransient<IReportService, ReportService>();
builder.Services.AddEndpointsApiExplorer();


var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/{town}", async (string town, IReportService reportService) =>
{
    var report = await reportService.BuildBoardgameReport(town);
    return Results.Ok(report);
});


app.Run();


