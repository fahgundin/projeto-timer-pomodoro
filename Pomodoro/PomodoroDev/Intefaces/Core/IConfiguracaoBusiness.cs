using WebApplication1.ViewModels;

namespace WebApplication1.Intefaces.Core;

public interface IConfiguracaoBusiness
{
    Task<ConfiguracaoViewModel> ObterConfiguracao();

    Task SalvarConfiguracao(ConfiguracaoViewModel configuracao);
}