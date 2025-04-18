using System.Runtime.CompilerServices;
using Fc.CodeFlix.Catalog.Api.Configurations;


var builder = WebApplication.CreateBuilder(args);


builder.Services
    .AddAppConections(builder.Configuration)
    .AddUseCases(builder.Configuration)
    .AddStorageConfiguration(builder.Configuration)
    .AddAndConfigureControllers();

builder.Services.AddControllers();

var app = builder.Build();

app.UseDocumentation();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
