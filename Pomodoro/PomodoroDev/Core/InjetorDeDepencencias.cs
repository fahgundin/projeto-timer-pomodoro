using WebApplication1.Intefaces.Core;
using WebApplication1.Intefaces.Repositorios;
using WebApplication1.Mocks;
using WebApplication1.Repositories;

namespace WebApplication1.Core;

public static class InjetorDeDepencencias
{
    public static IServiceCollection InjetarDependencias(this IServiceCollection services)
    {
        services.AddSingleton<ITarefaRepository, TarefaRepository>();
        services.AddSingleton<ICicloRepository, CicloRepository>();

        services.AddSingleton<ITarefaBusiness, TarefaBusiness>();
        services.AddSingleton<ICicloBusiness, CicloBusiness>();
        
        
       
        
        return services;
    }
}