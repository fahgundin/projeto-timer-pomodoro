using WebApplication1.Intefaces.Repositorios;
using WebApplication1.Mocks;

namespace WebApplication1.Core;

public static class InjetorDeDepencencias
{
    public static IServiceCollection InjetarDependencias(this IServiceCollection services)
    {
        services.AddSingleton<ITarefaRepository, MockTarefaRepository>();
        services.AddSingleton<ICicloRepository, MockCicloRepository>();

        return services;
    }
}