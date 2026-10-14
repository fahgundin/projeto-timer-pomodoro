namespace WebApplication1.ViewModels;

public class RelatorioTarefaViewModel
{
    public int TarefaId { get; set; }
    public required string TarefaNome { get; set; }
    public required RelatorioResumoViewModel ResumoDaTarefa { get; set; }
}