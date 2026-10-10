using WebApplication1.ViewModels;

namespace WebApplication1.Intefaces.Repositorios;

public interface IRelatorioRepository
{
    Task<ResumoDoRelatorioViewModel> ObterResumoDoPeriodo(DateTimeOffset inicio, DateTimeOffset fim);

    Task<List<DetalhePorTarefaViewModel>> ObterDetalhePorTarefa(DateTimeOffset inicio, DateTimeOffset fim);
}