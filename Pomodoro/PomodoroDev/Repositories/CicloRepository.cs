using Microsoft.EntityFrameworkCore;
using WebApplication1.Context;
using WebApplication1.Entities;
using WebApplication1.Intefaces.Repositorios;
using WebApplication1.ViewModels;

namespace WebApplication1.Repositories;

public class CicloRepository(PomodoroDbContext contexto) : ICicloRepository
{
    public Task<List<Ciclo>> ObterCiclosConcluidosDeUmaTarefa(int tarefaId)
    {
        // TODO: TEM QUE SER EXECUTADO POR UMA PROCEDURE
        throw new NotImplementedException();
    }

    public Task<Ciclo?> ObterCicloAtual()
    {
        // TODO: TAMBEM TEM QUE SER EXECUTADO POR UMA PROCEDURE
        throw new NotImplementedException();
    }

    public Task<int> ConsultarQuantidadeDeFocosConcluidosDesdeAUltimaPausaLonga(int tarefaId)
    {
        // TODO: TAMBEM TEM QUE SER EXECUTADO POR UMA PROCEDURE
        //
        // var ciclosDaTarefa = _cicloRepository.ObterCiclosConcluidosDeUmaTarefa(cicloASerPausado.TarefaId);
        //
        // var ultimaPausaLonga = ciclosDaTarefa
        //     .Where(c => c.TipoDoCiclo == TiposDeCiclo.PausaLonga)
        //     .OrderByDescending(c => c.DataHoraInicio)
        //     .FirstOrDefault();
        //
        // var focosConcluidos = ConsultarFocosConcluidosDesdeAUltimaPausaLonga(ciclosDaTarefa, ultimaPausaLonga);
        throw new NotImplementedException();
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