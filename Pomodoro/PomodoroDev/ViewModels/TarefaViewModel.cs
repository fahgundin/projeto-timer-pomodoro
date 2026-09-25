namespace WebApplication1.ViewModels;

public class TarefaViewModel
{
    public int TarefaId { get; set; }
    
    public required string NomeDaTarefa { get; set; }
    
    public int DuracaoFocoSegundos { get; set; }
    
    public int DuracaoPausaLonga { get; set; }
    
    public int DuracaoPausaCurta { get; set; }
    
    public int CiclosParaPausaLonga { get; set; }
    
    public DateTimeOffset DataHoraInicio { get; set; }
    
    public DateTimeOffset? DataHoraFim { get; set; }
    
    public bool Arquivado { get; set; }
    
}