using WebApplication1.Entities;

namespace WebApplication1.Intefaces.Core;

public interface ICicloBusiness
{
    Task<Ciclo?> ObterCicloAtual();
    
    Task<Ciclo> MudarDeCiclo();
    Task<Ciclo> FinalizarCicloAtual();
    Task<Ciclo> IniciarCiclo(int tarefaId);
}