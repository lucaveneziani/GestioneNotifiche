using GestioneNotificaQuadratureOra.Config;
using GestioneNotificheQuadratureOre.Serivces;


var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "Gestione Notifiche Ore";
});

var options = new ConfigurationOption();
options = builder.Configuration
                        .GetSection(nameof(ConfigurationOption))
                        .Get<ConfigurationOption>();

if(options.EnableNotificaQuadrature)
    builder.Services.AddHostedService<NotificaQuadratureOreService>();
if(options.EnableReminderImpegni)
    builder.Services.AddHostedService<ReminderImpegniService>();
#pragma warning disable CS8634 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'class' constraint.
builder.Services.AddSingleton(options);
#pragma warning restore CS8634 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'class' constraint.
var host = builder.Build();

host.Run();
