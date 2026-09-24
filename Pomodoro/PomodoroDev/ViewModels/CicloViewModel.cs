using WebApplication1.ViewModels.Enums;

namespace WebApplication1.ViewModels;

public class CicloViewModel
{
    
    public int CicloId { get; set; }
    
    public required TarefaViewModel TarefaId { get; set; }
    
    public TiposDeCiclo TipoDoCiclo { get; set; }
    
    public DateTimeOffset DataHoraInicio { get; set; }
    
    public DateTimeOffset DataHoraFim { get; set; }
    
    public bool Concluido { get; set; }
}