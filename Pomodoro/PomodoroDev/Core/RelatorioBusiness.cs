using WebApplication1.Intefaces.Core;
using WebApplication1.Intefaces.Repositorios;
using WebApplication1.ViewModels;

namespace WebApplication1.Core;

public class RelatorioBusiness(IRelatorioRepository relatorioRepository) : IRelatorioBusiness
{
    public async Task<ResumoDoRelatorioViewModel> ObterResumoDoPeriodo(DateTimeOffset inicio, DateTimeOffset fim)
    {
        if (inicio >= fim)
            throw new ArgumentException("O início do período deve ser anterior ao fim");

        return await relatorioRepository.ObterResumoDoPeriodo(inicio, fim);
    }

    public async Task<List<DetalhePorTarefaViewModel>> ObterDetalhePorTarefa(DateTimeOffset inicio, DateTimeOffset fim)
    {
        if (inicio >= fim)
            throw new ArgumentException("O início do período deve ser anterior ao fim");

        return await relatorioRepository.ObterDetalhePorTarefa(inicio, fim);
    }
}