using WebApplication1.Entities;
using WebApplication1.ViewModels;

namespace WebApplication1.Intefaces.Core;

public interface ITarefaBusiness
{
    Task<List<Tarefa>> ObterTarefas();

    Task AtualizarTarefa(TarefaViewModel tarefa);

    Task ArquivarTarefa(int tarefaId);

    Task ExcluirTarefa(int tarefaId);

    void CriarTarefa(TarefaViewModel tarefa);
}