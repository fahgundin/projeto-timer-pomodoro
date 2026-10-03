using Microsoft.EntityFrameworkCore;
using WebApplication1.Context;
using WebApplication1.Entities;
using WebApplication1.Intefaces.Repositorios;

namespace WebApplication1.Repositories;

public class CicloRepository(PomodoroDbContext contexto) : ICicloRepository
{
    private const int tipoFoco = 0;
    private const int tipoPausaLonga = 1;

    public async Task<List<Ciclo>> ObterCiclosConcluidosDeUmaTarefa(int tarefaId)
    {
        return await contexto.Ciclos
            .AsNoTracking()
            .Where(c => c.TarefaId == tarefaId && c.Concluido)
            .OrderBy(c => c.DataHoraInicio)
            .ToListAsync();
    }

    public async Task<Ciclo?> ObterCicloAtual()
    {
        return await contexto.Ciclos
            .Include(c => c.Tarefa)
            .Where(c => c.DataHoraFim == null)
            .OrderByDescending(c => c.DataHoraInicio)
            .FirstOrDefaultAsync();
    }

    public async Task<int> ConsultarQuantidadeDeFocosConcluidosDesdeAUltimaPausaLonga(int tarefaId)
    {
        DateTimeOffset? inicioDaUltimaPausaLonga = await contexto.Ciclos
            .Where(c => c.TarefaId == tarefaId && c.TipoDoCiclo == tipoPausaLonga && c.Concluido)
            .OrderByDescending(c => c.DataHoraInicio)
            .Select(c => c.DataHoraInicio)
            .FirstOrDefaultAsync();

        return await contexto.Ciclos.CountAsync(c =>
            c.TarefaId == tarefaId &&
            c.TipoDoCiclo == tipoFoco &&
            c.Concluido &&
            (inicioDaUltimaPausaLonga == null || c.DataHoraInicio > inicioDaUltimaPausaLonga));
    }

    public async Task DefinirCicloComoConcluido(int cicloId)
    {
        var linhasAfetadas = await contexto.Ciclos
            .Where(c => c.CicloId == cicloId)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(c => c.Concluido, true));

        if (linhasAfetadas == 0)
        {
            var existe = await contexto.Ciclos.AnyAsync(c => c.CicloId == cicloId);
            if (!existe)
                throw new KeyNotFoundException("Ciclo não encontrado");
        }
    }

    public async Task CriarCiclo(Ciclo ciclo)
    {
        var tarefa = await contexto.Tarefas
            .AsNoTracking()
            .FirstAsync(t => t.TarefaId == ciclo.TarefaId);

        ciclo.DuracaoPlanejadaSegundos = ciclo.TipoDoCiclo switch
        {
            tipoFoco => tarefa.DuracaoFocoSegundos,
            tipoPausaLonga => tarefa.DuracaoPausaLonga,
            _ => tarefa.DuracaoPausaCurta
        };

        await contexto.Ciclos.AddAsync(ciclo);
        await contexto.SaveChangesAsync();
    }

    public async Task<Ciclo> FinalizarCiclo(Ciclo ciclo)
    {
        ciclo.DataHoraFim = DateTimeOffset.UtcNow;
        await contexto.SaveChangesAsync();
        return ciclo;
    }
}