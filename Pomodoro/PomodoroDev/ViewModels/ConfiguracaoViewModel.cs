namespace WebApplication1.ViewModels;

public class ConfiguracaoViewModel
{
    public int DuracaoFocoMinutos { get; set; }

    public int DuracaoPausaCurtaMinutos { get; set; }

    public int DuracaoPausaLongaMinutos { get; set; }

    public int CiclosParaPausaLonga { get; set; }

    public bool PausaLongaAtivada { get; set; }
}