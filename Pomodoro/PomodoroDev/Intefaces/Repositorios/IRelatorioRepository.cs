using WebApplication1.ViewModels;

namespace WebApplication1.Intefaces.Repositorios;

public interface IRelatorioRepository
{
    Task<RelatorioResumoViewModel> ObterResumoDoPeriodo(DateTimeOffset inicio, DateTimeOffset fim);

    Task<List<RelatorioTarefaViewModel>> ObterDetalhePorTarefa(DateTimeOffset inicio, DateTimeOffset fim);
}