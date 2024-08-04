using BrickApi.Client;
using BuilderCatalogue.Managers;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

var builder = WebApplication.CreateBuilder(args);

var apiAuthProvider = new AnonymousAuthenticationProvider();
var apiAdapter = new HttpClientRequestAdapter(apiAuthProvider);

builder.Services.AddTransient(_ => new BrickApiClient(apiAdapter))
                .AddTransient<ISetDataManager, SetDataManager>()
                .AddTransient<IUserDataManager, UserDataManager>()
                .AddTransient<ISolutionsManager, SolutionsManager>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

static void MapEndpoints(WebApplication app)
{
    var solutionsManager = app.Services.GetRequiredService<ISolutionsManager>();
    var assignmentsApi = app.MapGroup("api/assigments").WithTags("Assignments").WithOpenApi();
    assignmentsApi.MapGet("/1", async () => await solutionsManager.SolveFirstAssignment());
    assignmentsApi.MapGet("/2", async () => await solutionsManager.SolveSecondAssignment());
    assignmentsApi.MapGet("/3", async () => await solutionsManager.SolveThirdAssignment());
    assignmentsApi.MapGet("/4", async () => await solutionsManager.SolveFourthAssignment());
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

MapEndpoints(app);

await app.RunAsync();
