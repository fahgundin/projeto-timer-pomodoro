using WebApplication1.Intefaces.Repositorios;
using WebApplication1.ViewModels;

namespace WebApplication1.Mocks;

public class MockTarefaRepository : ITarefaRepository 
{
    
    public List<TarefaViewModel> ObterTarefas()
    {
        throw new NotImplementedException();
    }

    public void AtualizarTarefa(TarefaViewModel tarefa)
    {
        throw new NotImplementedException();
    }

    public void ExcluirTarefa(TarefaViewModel tarefa)
    {
        throw new NotImplementedException();
    }

    public void CriarTarefa(TarefaViewModel tarefa)
    {
        throw new NotImplementedException();
    }

    public TarefaViewModel ObterTarefaAtual()
    {
        return new TarefaViewModel
        {
            TarefaId = 1,
            CiclosParaPausaLonga = 2,
            DataHoraInicio = DateTime.Now,
            DuracaoFocoSegundos = 15,
            DuracaoPausaCurta = 5,
            DuracaoPausaLonga = 10,
            Arquivado = false,
            NomeDaTarefa = "Teste do Pomodoro",
        };
    }
}