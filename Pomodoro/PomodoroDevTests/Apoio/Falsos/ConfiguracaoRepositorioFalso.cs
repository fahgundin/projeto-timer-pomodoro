using WebApplication1.Intefaces.Repositorios;
using WebApplication1.ViewModels;

namespace PomodoroDev.Tests.Apoio.Falsos;

public class ConfiguracaoRepositorioFalso : IConfiguracaoRepository
{
    public ConfiguracaoViewModel ConfiguracaoADevolver { get; set; } = new()
    {
        DuracaoFoco = 50,
        DuracaoPausaCurta = 10,
        DuracaoPausaLonga = 20,
        CiclosParaPausaLonga = 4,
        PausaLongaAtivada = true
    };

    public ConfiguracaoViewModel? ConfiguracaoSalva { get; private set; }

    public Task<ConfiguracaoViewModel> ObterConfiguracao()
        => Task.FromResult(ConfiguracaoADevolver);

    public Task SalvarConfiguracao(ConfiguracaoViewModel configuracao)
    {
        ConfiguracaoSalva = configuracao;
        return Task.CompletedTask;
    }
}