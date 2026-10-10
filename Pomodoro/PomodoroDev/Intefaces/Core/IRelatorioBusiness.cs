using WebApplication1.ViewModels;

namespace WebApplication1.Intefaces.Core;

public interface IRelatorioBusiness
{
    Task<ResumoDoRelatorioViewModel> ObterResumoDoPeriodo(DateTimeOffset inicio, DateTimeOffset fim);

    Task<List<DetalhePorTarefaViewModel>> ObterDetalhePorTarefa(DateTimeOffset inicio, DateTimeOffset fim);
    
}