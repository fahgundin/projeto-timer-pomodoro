using Microsoft.EntityFrameworkCore;
using WebApplication1.Context;
using WebApplication1.Intefaces.Repositorios;
using WebApplication1.ViewModels;

namespace WebApplication1.Repositories;

public class RelatorioRepository(PomodoroDbContext contexto) : IRelatorioRepository
{
    private const int tipoFoco = 0;

    public async Task<ResumoDoRelatorioViewModel> ObterResumoDoPeriodo(DateTimeOffset inicio, DateTimeOffset fim)
    {
        var resultado = await contexto.Ciclos
            .AsNoTracking()
            .Where(c =>
                c.TipoDoCiclo == tipoFoco &&
                c.Concluido &&
                c.DataHoraInicio >= inicio &&
                c.DataHoraInicio < fim)
            .GroupBy(_ => 1)
            .Select(g => new ResumoDoRelatorioViewModel
            {
                TotalDePomodoros = g.Count(),
                TotalDeSegundosFocados = g.Sum(c => c.DuracaoPlanejadaSegundos)
            })
            .FirstOrDefaultAsync();

        return resultado ?? new ResumoDoRelatorioViewModel();
    }

    public async Task<List<DetalhePorTarefaViewModel>> ObterDetalhePorTarefa(DateTimeOffset inicio, DateTimeOffset fim)
    {
        return await contexto.Ciclos
            .AsNoTracking()
            .Where(c =>
                c.TipoDoCiclo == tipoFoco &&
                c.Concluido &&
                c.DataHoraInicio >= inicio &&
                c.DataHoraInicio < fim)
            .GroupBy(c => new { c.TarefaId, c.Tarefa.NomeDaTarefa })
            .Select(g => new DetalhePorTarefaViewModel
            {
                TarefaId = g.Key.TarefaId,
                NomeDaTarefa = g.Key.NomeDaTarefa,
                Pomodoros = g.Count(),
                SegundosFocados = g.Sum(c => c.DuracaoPlanejadaSegundos)
            })
            .OrderByDescending(d => d.Pomodoros)
            .ToListAsync();
    }
}