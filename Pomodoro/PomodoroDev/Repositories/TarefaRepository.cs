using Microsoft.EntityFrameworkCore;
using WebApplication1.Context;
using WebApplication1.Entities;
using WebApplication1.Intefaces.Repositorios;
using WebApplication1.ViewModels;

namespace WebApplication1.Repositories;

public class TarefaRepository(PomodoroDbContext contexto) : ITarefaRepository
{
    public async Task<List<Tarefa>> ObterTarefas()
    {
        return await contexto.Tarefas
            .AsNoTracking()
            .OrderBy(t => t.TarefaId)
            .ToListAsync();
    }

    public async Task AtualizarTarefa(TarefaViewModel tarefa)
    {
        if (tarefa.TarefaId == null)
            throw new ArgumentException("Por favor inserir o Id da tarefa", nameof(tarefa));

        var tarefaDoBanco = contexto.Tarefas.FirstOrDefault(t => t.TarefaId == tarefa.TarefaId);
        if (tarefaDoBanco == null)
            throw new KeyNotFoundException($"Tarefa {tarefa.TarefaId} não encontrada.");

        tarefaDoBanco.TarefaId = tarefa.TarefaId.Value;
        tarefaDoBanco.NomeDaTarefa = tarefa.NomeDaTarefa;
        tarefaDoBanco.DuracaoFocoSegundos = tarefa.DuracaoFocoSegundos;
        tarefaDoBanco.DuracaoPausaLonga = tarefa.DuracaoPausaLonga;
        tarefaDoBanco.DuracaoPausaCurta = tarefa.DuracaoPausaCurta;
        tarefaDoBanco.CiclosParaPausaLonga = tarefa.CiclosParaPausaLonga;
        tarefaDoBanco.Arquivado = tarefa.Arquivado;
        tarefaDoBanco.Cor = tarefa.Cor;

        await contexto.SaveChangesAsync();
    }

    public async Task ArquivarTarefa(int tarefaId)
    {
        var linhasAfetadas = await contexto.Tarefas.Where(t => t.TarefaId == tarefaId)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.Arquivado, true));

        if (linhasAfetadas == 0)
        {
            var existe = await contexto.Tarefas.AnyAsync(t => t.TarefaId == tarefaId);
            if (!existe)
                throw new KeyNotFoundException("Tarefa não encontrada");
        }
    }

    public async Task ExcluirTarefa(int tarefaId)
    {
        await using var transacao = await contexto.Database.BeginTransactionAsync();

        await contexto.Ciclos
            .Where(c => c.TarefaId == tarefaId)
            .ExecuteDeleteAsync();

        var linhasAfetadas = await contexto.Tarefas
            .Where(t => t.TarefaId == tarefaId)
            .ExecuteDeleteAsync();

        if (linhasAfetadas == 0)
            throw new KeyNotFoundException("Tarefa não encontrada");

        await transacao.CommitAsync();
    }

    public void CriarTarefa(TarefaViewModel tarefa)
    {
        var tarefaASerCriada = new Tarefa()
        {
            NomeDaTarefa = tarefa.NomeDaTarefa,
            DuracaoFocoSegundos = tarefa.DuracaoFocoSegundos,
            DuracaoPausaLonga = tarefa.DuracaoPausaLonga,
            DuracaoPausaCurta = tarefa.DuracaoPausaCurta,
            CiclosParaPausaLonga = tarefa.CiclosParaPausaLonga,
            DataHoraInicio = DateTimeOffset.Now,
            Arquivado = false,
            Cor = tarefa.Cor,
        };
        contexto.Tarefas.Add(tarefaASerCriada);
        contexto.SaveChanges();
    }
}