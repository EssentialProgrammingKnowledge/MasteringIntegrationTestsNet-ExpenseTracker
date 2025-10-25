using Microsoft.Extensions.DependencyInjection;
using ExpenseTracker.UI.Services;
using ExpenseTracker.IntegrationTests.Setup.Services;

namespace ExpenseTracker.IntegrationTests.Setup.Services
{
    internal static class Extensions
    {
        public static IServiceCollection DecorateExistingServices(this IServiceCollection services)
        {
            return services.DecorateService<ICategoryService, ObservableCategoryService>()
                           .AddScoped(sp => (IObservableCategoryService)sp.GetRequiredService<ICategoryService>())
                           .DecorateService<IExpenseService, ObservableExpenseService>()
                           .AddScoped(sp => (IObservableExpenseService)sp.GetRequiredService<IExpenseService>());
        }

        private static IServiceCollection DecorateService<TDefinition, TDecorator>(this IServiceCollection services)
            where TDefinition : class
            where TDecorator : TDefinition
        {
            var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(TDefinition))
                ?? throw new InvalidOperationException($"{typeof(TDefinition).FullName} not registered!");
            services.Remove(descriptor);

            services.Add(new ServiceDescriptor(typeof(TDefinition), sp =>
            {
                object inner;
                if (descriptor.ImplementationInstance != null)
                {
                    inner = descriptor.ImplementationInstance;
                }
                else if (descriptor.ImplementationFactory != null)
                {
                    inner = descriptor.ImplementationFactory(sp);
                }
                else
                {
                    inner = ActivatorUtilities.CreateInstance(sp, descriptor.ImplementationType!);
                }

                return ActivatorUtilities.CreateInstance<TDecorator>(sp, inner);
            }, descriptor.Lifetime));
            return services;
        }
    }
}
