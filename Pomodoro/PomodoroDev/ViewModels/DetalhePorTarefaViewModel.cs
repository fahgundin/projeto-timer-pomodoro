namespace WebApplication1.ViewModels;

public class DetalhePorTarefaViewModel
{
    public int TarefaId { get; set; }

    public string NomeDaTarefa { get; set; } = string.Empty;

    public int Pomodoros { get; set; }

    public int SegundosFocados { get; set; }
}