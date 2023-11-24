using GestioneNotifiche;
using GestioneNotifiche.Core.Mail;
using PublishPrices.Config;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<NotificaQuadratureOreService>();

var options = new ConfigurationOption();
options = builder.Configuration
                        .GetSection(nameof(ConfigurationOption))
                        .Get<ConfigurationOption>();

builder.Services.AddSingleton(options);
var host = builder.Build();

host.Run();
