using Microsoft.EntityFrameworkCore;
using WebApplication1.Context;
using WebApplication1.Entities;
using WebApplication1.Intefaces.Repositorios;
using WebApplication1.ViewModels;

namespace WebApplication1.Repositories;

public class ConfiguracaoRepository(PomodoroDbContext contexto) : IConfiguracaoRepository
{
    private const int idDaConfiguracao = 1;
    private const int minutosParaSegundos = 60;

    private static readonly ConfiguracaoViewModel ConfiguracaoPadrao = new()
    {
        DuracaoFocoMinutos = 50,
        DuracaoPausaCurtaMinutos = 10,
        DuracaoPausaLongaMinutos = 20,
        CiclosParaPausaLonga = 4,
        PausaLongaAtivada = true
    };

    public async Task<ConfiguracaoViewModel> ObterConfiguracao()
    {
        var configuracao = await contexto.Configuracoes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.ConfiguracaoId == idDaConfiguracao);

        if (configuracao == null)
            return ConfiguracaoPadrao;

        return new ConfiguracaoViewModel
        {
            DuracaoFocoMinutos = configuracao.DuracaoFocoSegundos / minutosParaSegundos,
            DuracaoPausaCurtaMinutos = configuracao.DuracaoPausaCurta / minutosParaSegundos,
            DuracaoPausaLongaMinutos = configuracao.DuracaoPausaLonga / minutosParaSegundos,
            CiclosParaPausaLonga = configuracao.CiclosParaPausaLonga,
            PausaLongaAtivada = configuracao.PausaLongaAtivada
        };
    }

    public async Task SalvarConfiguracao(ConfiguracaoViewModel configuracao)
    {
        var existente = await contexto.Configuracoes
            .FirstOrDefaultAsync(c => c.ConfiguracaoId == idDaConfiguracao);

        if (existente == null)
        {
            contexto.Configuracoes.Add(new Configuracao
            {
                ConfiguracaoId = idDaConfiguracao,
                DuracaoFocoSegundos = configuracao.DuracaoFocoMinutos * minutosParaSegundos,
                DuracaoPausaCurta = configuracao.DuracaoPausaCurtaMinutos * minutosParaSegundos,
                DuracaoPausaLonga = configuracao.DuracaoPausaLongaMinutos * minutosParaSegundos,
                CiclosParaPausaLonga = configuracao.CiclosParaPausaLonga,
                PausaLongaAtivada = configuracao.PausaLongaAtivada
            });
        }
        else
        {
            existente.DuracaoFocoSegundos = configuracao.DuracaoFocoMinutos * minutosParaSegundos;
            existente.DuracaoPausaCurta = configuracao.DuracaoPausaCurtaMinutos * minutosParaSegundos;
            existente.DuracaoPausaLonga = configuracao.DuracaoPausaLongaMinutos * minutosParaSegundos;
            existente.CiclosParaPausaLonga = configuracao.CiclosParaPausaLonga;
            existente.PausaLongaAtivada = configuracao.PausaLongaAtivada;
        }

        await contexto.SaveChangesAsync();
    }
}