using WebApplication1.Intefaces.Repositorios;
using WebApplication1.ViewModels;
using WebApplication1.ViewModels.Enums;

namespace WebApplication1.Mocks;

public class MockCicloRepository : ICicloRepository 
{
    public List<CicloViewModel> ObterCiclosConcluidosDeUmaTarefa(TarefaViewModel tarefa)
    {
        throw new NotImplementedException();
    }

    public CicloViewModel ObterCicloAtual(TarefaViewModel tarefa)
    {
        CicloViewModel novoCicloMockado = new CicloViewModel
        {
            CicloId = 1,
            TarefaId = tarefa,
            DataHoraInicio =  DateTime.Now,
            TipoDoCiclo = TiposDeCiclo.Focus
            
            
        };

        return novoCicloMockado;
    }

    public void DefinirCicloComoConcluido(CicloViewModel ciclo)
    {
        ciclo.Concluido = true;
    }

    public void CriarCiclo(TarefaViewModel tarefa, CicloViewModel ciclo)
    {
        ciclo.DataHoraInicio = new DateTimeOffset(DateTime.Now);
    }
}