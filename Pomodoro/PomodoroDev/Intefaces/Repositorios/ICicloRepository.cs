using WebApplication1.ViewModels;

namespace WebApplication1.Intefaces.Repositorios;

public interface ICicloRepository
{
    public List<CicloViewModel> ObterCiclosConcluidosDeUmaTarefa(TarefaViewModel tarefa);
    
    public CicloViewModel ObterCicloAtual(TarefaViewModel tarefaAtual);
    
    public void DefinirCicloComoConcluido(CicloViewModel ciclo);
    
    public void CriarCiclo(CicloViewModel ciclo);


}