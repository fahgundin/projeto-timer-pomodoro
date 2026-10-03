using WebApplication1.Intefaces.Core;
using WebApplication1.Intefaces.Repositorios;
using WebApplication1.Repositories;

namespace WebApplication1.Core;

public static class InjetorDeDepencencias
{
    public static IServiceCollection InjetarDependencias(this IServiceCollection services)
    {
        services.AddScoped<ITarefaRepository, TarefaRepository>();
        services.AddScoped<ICicloRepository, CicloRepository>();

        services.AddScoped<ITarefaBusiness, TarefaBusiness>();
        services.AddScoped<ICicloBusiness, CicloBusiness>();

        return services;
    }
}