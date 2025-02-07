using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Rijnkaai.Abstractions;
using Rijnkaai.Business;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

//Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
 builder.Services
     .AddApplicationInsightsTelemetryWorkerService()
     .ConfigureFunctionsApplicationInsights();

builder.Services.AddLogging();

builder.Services.AddHttpClient();
builder.Services.AddTransient<INotificationService, SlackService>();
builder.Services.AddTransient<INotificationService, TeamsService>();
builder.Services.AddTransient<IRijnkaaiService, RijnkaaiService>();

JsonConvert.DefaultSettings = () => new JsonSerializerSettings
{
    NullValueHandling = NullValueHandling.Ignore
};

builder.Build().Run();
