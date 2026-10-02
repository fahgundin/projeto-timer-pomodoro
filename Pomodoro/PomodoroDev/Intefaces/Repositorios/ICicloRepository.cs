using WebApplication1.Entities;
namespace WebApplication1.Intefaces.Repositorios;

public interface ICicloRepository
{
    Task<List<Ciclo>> ObterCiclosConcluidosDeUmaTarefa(int tarefaId);
    
    Task<Ciclo> ObterCicloAtual();
    
    Task DefinirCicloComoConcluido(int cicloId);
    
    Task CriarCiclo(Ciclo ciclo);
    
    Task<Ciclo> FinalizarCiclo(Ciclo ciclo);

    Task<int> ConsultarQuantidadeDeFocosConcluidosDesdeAUltimaPausaLonga(int tarefaId);



}