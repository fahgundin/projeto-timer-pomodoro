using WebApplication1.Intefaces.Core;
using WebApplication1.Intefaces.Repositorios;
using WebApplication1.Mocks;

namespace WebApplication1.Core;

public static class InjetorDeDepencencias
{
    public static IServiceCollection InjetarDependencias(this IServiceCollection services)
    {
        services.AddSingleton<ITarefaRepository, MockTarefaRepository>();
        services.AddSingleton<ICicloRepository, MockCicloRepository>();

        services.AddSingleton<ICicloBusiness, CicloBusiness>();
        
        return services;
    }
}