using GestioneNotifiche;
using GestioneNotifiche.Core.Mail;
using PublishPrices.Config;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<NotificaQuadratureOreService>();

var options = new ConfigurationOption();
options = builder.Configuration
                        .GetSection(nameof(ConfigurationOption))
                        .Get<ConfigurationOption>();

#pragma warning disable CS8634 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'class' constraint.
builder.Services.AddSingleton(options);
#pragma warning restore CS8634 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'class' constraint.
var host = builder.Build();

host.Run();
