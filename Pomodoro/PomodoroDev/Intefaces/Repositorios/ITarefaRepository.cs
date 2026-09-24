using WebApplication1.ViewModels;

namespace WebApplication1.Intefaces.Repositorios;

public interface ITarefaRepository
{
    public List<TarefaViewModel> ObterTarefas();
    
    public void AtualizarTarefa(TarefaViewModel tarefa);
    
    public void ExcluirTarefa(TarefaViewModel tarefa);
    
    public void CriarTarefa(TarefaViewModel tarefa);
}