using Microsoft.EntityFrameworkCore;
using WebApplication1.Context;
using WebApplication1.Intefaces.Repositorios;
using WebApplication1.ViewModels;

namespace WebApplication1.Repositories;

public class RelatorioRepository(PomodoroDbContext contexto) : IRelatorioRepository
{

    public async Task<RelatorioResumoViewModel> ObterResumoDoPeriodo(DateTimeOffset inicio, DateTimeOffset fim)
    {
        throw new NotImplementedException();
    }

    public async Task<List<RelatorioTarefaViewModel>> ObterDetalhePorTarefa(DateTimeOffset inicio, DateTimeOffset fim)
    {
        throw new NotImplementedException();
    }
}