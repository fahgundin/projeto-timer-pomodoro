using WebApplication1.ViewModels;

namespace WebApplication1.Intefaces.Repositorios;

public interface IConfiguracaoRepository
{
    Task<ConfiguracaoViewModel> ObterConfiguracao();

    Task SalvarConfiguracao(ConfiguracaoViewModel configuracao);
}