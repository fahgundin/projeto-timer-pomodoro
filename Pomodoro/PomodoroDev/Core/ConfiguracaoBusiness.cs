using WebApplication1.Intefaces.Core;
using WebApplication1.Intefaces.Repositorios;
using WebApplication1.ViewModels;

namespace WebApplication1.Core;

public class ConfiguracaoBusiness(IConfiguracaoRepository configuracaoRepository) : IConfiguracaoBusiness
{
    private const int focoMinimo = 1;
    private const int focoMaximo = 90;
    private const int pausaCurtaMinima = 1;
    private const int pausaCurtaMaxima = 30;
    private const int pausaLongaMinima = 1;
    private const int pausaLongaMaxima = 60;
    private const int ciclosMinimo = 1;
    private const int ciclosMaximo = 10;

    public async Task<ConfiguracaoViewModel> ObterConfiguracao()
    {
        return await configuracaoRepository.ObterConfiguracao();
    }

    public async Task SalvarConfiguracao(ConfiguracaoViewModel configuracao)
    {
        var erros = Validar(configuracao);

        if (erros.Count > 0)
            throw new ArgumentException(string.Join("; ", erros));

        await configuracaoRepository.SalvarConfiguracao(configuracao);
    }

    private static List<string> Validar(ConfiguracaoViewModel configuracao)
    {
        var erros = new List<string>();

        if (configuracao.DuracaoFoco < focoMinimo || configuracao.DuracaoFoco > focoMaximo)
            erros.Add($"Foco deve ser entre {focoMinimo} e {focoMaximo} ");

        if (configuracao.DuracaoPausaCurta < pausaCurtaMinima || configuracao.DuracaoPausaCurta > pausaCurtaMaxima)
            erros.Add($"Pausa curta deve ser entre {pausaCurtaMinima} e {pausaCurtaMaxima} ");

        if (configuracao.DuracaoPausaLonga < pausaLongaMinima || configuracao.DuracaoPausaLonga > pausaLongaMaxima)
            erros.Add($"Pausa longa deve ser entre {pausaLongaMinima} e {pausaLongaMaxima} ");

        if (configuracao.CiclosParaPausaLonga < ciclosMinimo || configuracao.CiclosParaPausaLonga > ciclosMaximo)
            erros.Add($"Ciclos deve ser entre {ciclosMinimo} e {ciclosMaximo}");

        if (configuracao.PausaLongaAtivada &&
            configuracao.DuracaoPausaLonga <= configuracao.DuracaoPausaCurta)
            erros.Add("Pausa longa deve ser maior que a pausa curta");

        return erros;
    }
}