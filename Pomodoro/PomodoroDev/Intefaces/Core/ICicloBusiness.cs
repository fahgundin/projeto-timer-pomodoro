using WebApplication1.ViewModels;
using WebApplication1.ViewModels.Enums;

namespace WebApplication1.Intefaces.Core;

public interface ICicloBusiness
{
    public void IniciarCiclo(TarefaViewModel tarefa, TiposDeCiclo tipoDoCiclo);
    
    public void Pausar(CicloViewModel cicloASerPausado);

    public void Focar(CicloViewModel cicloASerFocado);
    
    public void FinalizarCiclo(CicloViewModel ciclo);

    public CicloViewModel ObterCicloAtual();
    
    public CicloViewModel MudarDeCiclo();
}