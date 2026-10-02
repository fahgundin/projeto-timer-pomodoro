using WebApplication1.Entities;
using WebApplication1.Intefaces.Core;
using WebApplication1.Intefaces.Repositorios;
using WebApplication1.ViewModels;
using WebApplication1.ViewModels.Enums;

namespace WebApplication1.Mocks;

public class MockCicloRepository : ICicloRepository 
{
    private ICicloRepository _cicloRepositoryImplementation;
    private static List<CicloViewModel>? _ciclos;

    public Task<List<Ciclo>> ObterCiclosConcluidosDeUmaTarefa(int tarefaId)
    {
        throw new NotImplementedException();
    }

    public Task<Ciclo> ObterCicloAtual()
    {
        throw new NotImplementedException();
    }

    public Task DefinirCicloComoConcluido(int cicloId)
    {
        throw new NotImplementedException();
    }

    public Task CriarCiclo(Ciclo ciclo)
    {
        throw new NotImplementedException();
    }

    public Task<Ciclo> FinalizarCiclo(Ciclo ciclo)
    {
        throw new NotImplementedException();
    }

    public Task<int> ConsultarQuantidadeDeFocosConcluidosDesdeAUltimaPausaLonga(int tarefaId)
    {
        throw new NotImplementedException();
    }
}