namespace WebApplication1.Entities;

public partial class Configuracao
{
    public int ConfiguracaoId { get; set; }

    public int DuracaoFocoSegundos { get; set; }

    public int DuracaoPausaCurta { get; set; }

    public int DuracaoPausaLonga { get; set; }

    public int CiclosParaPausaLonga { get; set; }

    public bool PausaLongaAtivada { get; set; }
}