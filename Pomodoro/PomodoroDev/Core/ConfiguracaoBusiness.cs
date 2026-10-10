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

        if (configuracao.DuracaoFocoMinutos < focoMinimo || configuracao.DuracaoFocoMinutos > focoMaximo)
            erros.Add($"Foco deve ser entre {focoMinimo} e {focoMaximo} minutos");

        if (configuracao.DuracaoPausaCurtaMinutos < pausaCurtaMinima || configuracao.DuracaoPausaCurtaMinutos > pausaCurtaMaxima)
            erros.Add($"Pausa curta deve ser entre {pausaCurtaMinima} e {pausaCurtaMaxima} minutos");

        if (configuracao.DuracaoPausaLongaMinutos < pausaLongaMinima || configuracao.DuracaoPausaLongaMinutos > pausaLongaMaxima)
            erros.Add($"Pausa longa deve ser entre {pausaLongaMinima} e {pausaLongaMaxima} minutos");

        if (configuracao.CiclosParaPausaLonga < ciclosMinimo || configuracao.CiclosParaPausaLonga > ciclosMaximo)
            erros.Add($"Ciclos deve ser entre {ciclosMinimo} e {ciclosMaximo}");

        if (configuracao.PausaLongaAtivada &&
            configuracao.DuracaoPausaLongaMinutos <= configuracao.DuracaoPausaCurtaMinutos)
            erros.Add("Pausa longa deve ser maior que a pausa curta");

        return erros;
    }
}