namespace WebApplication1.ViewModels;

public class ConfiguracaoViewModel
{
    public int DuracaoFoco { get; set; }

    public int DuracaoPausaCurta { get; set; }

    public int DuracaoPausaLonga { get; set; }

    public int CiclosParaPausaLonga { get; set; }

    public bool PausaLongaAtivada { get; set; }
}