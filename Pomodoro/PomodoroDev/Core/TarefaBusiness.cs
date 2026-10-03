using WebApplication1.Entities;
using WebApplication1.Intefaces.Core;
using WebApplication1.Intefaces.Repositorios;
using WebApplication1.ViewModels;

namespace WebApplication1.Core;

public class TarefaBusiness(ITarefaRepository tarefaRepository) : ITarefaBusiness
{
    public async Task<List<Tarefa>> ObterTarefas()
    {
        var tarefas = await tarefaRepository.ObterTarefas();
        return tarefas;
    }

    public async Task AtualizarTarefa(TarefaViewModel tarefa)
    {
        await tarefaRepository.AtualizarTarefa(tarefa);
    }

    public async Task ArquivarTarefa(int tarefaId)
    {
        await tarefaRepository.ArquivarTarefa(tarefaId);
    }

    public async Task ExcluirTarefa(int tarefaId)
    {
        await tarefaRepository.ExcluirTarefa(tarefaId);
    }

    public void CriarTarefa(TarefaViewModel tarefa)
    {
        tarefaRepository.CriarTarefa(tarefa);
    }
}