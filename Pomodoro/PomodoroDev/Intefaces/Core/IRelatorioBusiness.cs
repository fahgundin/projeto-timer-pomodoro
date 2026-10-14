using WebApplication1.ViewModels;

namespace WebApplication1.Intefaces.Core;

public interface IRelatorioBusiness
{
    Task<RelatorioResumoViewModel> ObterResumoDoPeriodo(DateTimeOffset inicio, DateTimeOffset fim);

    Task<List<RelatorioTarefaViewModel>> ObterDetalhePorTarefa(DateTimeOffset inicio, DateTimeOffset fim);
    
}