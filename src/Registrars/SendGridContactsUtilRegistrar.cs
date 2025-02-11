using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.SendGrid.Client.Registrars;
using Soenneker.SendGrid.Contacts.Abstract;

namespace Soenneker.SendGrid.Contacts.Registrars;

/// <summary>
/// A .NET typesafe implementation of SendGrid's Contact API
/// </summary>
public static class SendGridContactsUtilRegistrar
{
    /// <summary>
    /// Adds <see cref="ISendGridContactsUtil"/> as a singleton service. <para/>
    /// </summary>
    public static IServiceCollection AddSendGridContactsUtilAsSingleton(this IServiceCollection services)
    {
        services.AddSendGridClientUtilAsSingleton();
        services.TryAddSingleton<ISendGridContactsUtil, SendGridContactsUtil>();
        return services;
    }

    /// <summary>
    /// Adds <see cref="ISendGridContactsUtil"/> as a scoped service. <para/>
    /// </summary>
    public static IServiceCollection AddSendGridContactsUtilAsScoped(this IServiceCollection services)
    {
        services.AddSendGridClientUtilAsSingleton();
        services.TryAddScoped<ISendGridContactsUtil, SendGridContactsUtil>();
        return services;
    }
}