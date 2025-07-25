public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        // Ex: DbContext, Repos, External services
        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Ex: UseCases, Services, Validators
        return services;
    }
}