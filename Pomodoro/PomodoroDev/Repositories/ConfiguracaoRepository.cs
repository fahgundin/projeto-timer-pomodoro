using Microsoft.EntityFrameworkCore;
using WebApplication1.Context;
using WebApplication1.Entities;
using WebApplication1.Intefaces.Repositorios;
using WebApplication1.ViewModels;

namespace WebApplication1.Repositories;

public class ConfiguracaoRepository(PomodoroDbContext contexto) : IConfiguracaoRepository
{
    private const int idDaConfiguracao = 1;

    private static readonly ConfiguracaoViewModel ConfiguracaoPadrao = new()
    {
        DuracaoFoco = 500,
        DuracaoPausaCurta = 100,
        DuracaoPausaLonga = 200,
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
            DuracaoFoco = configuracao.DuracaoFocoSegundos,
            DuracaoPausaCurta = configuracao.DuracaoPausaCurta ,
            DuracaoPausaLonga = configuracao.DuracaoPausaLonga,
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
            contexto.Configuracoes.Add(new Configuraco
            {
                ConfiguracaoId = idDaConfiguracao,
                DuracaoFocoSegundos = configuracao.DuracaoFoco,
                DuracaoPausaCurta = configuracao.DuracaoPausaCurta,
                DuracaoPausaLonga = configuracao.DuracaoPausaLonga,
                CiclosParaPausaLonga = configuracao.CiclosParaPausaLonga,
                PausaLongaAtivada = configuracao.PausaLongaAtivada
            });
        }
        else
        {
            existente.DuracaoFocoSegundos = configuracao.DuracaoFoco;
            existente.DuracaoPausaCurta = configuracao.DuracaoPausaCurta;
            existente.DuracaoPausaLonga = configuracao.DuracaoPausaLonga;
            existente.CiclosParaPausaLonga = configuracao.CiclosParaPausaLonga;
            existente.PausaLongaAtivada = configuracao.PausaLongaAtivada;
        }

        await contexto.SaveChangesAsync();
    }
}