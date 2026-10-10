using Microsoft.AspNetCore.Mvc;
using WebApplication1.Intefaces.Core;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers;

public class ConfiguracaoController(IConfiguracaoBusiness configuracaoBusiness) : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ObterConfiguracao()
    {
        var configuracao = await configuracaoBusiness.ObterConfiguracao();
        return Ok(configuracao);
    }

    [HttpPost]
    public async Task<IActionResult> SalvarConfiguracao([FromBody] ConfiguracaoViewModel configuracao)
    {
        try
        {
            await configuracaoBusiness.SalvarConfiguracao(configuracao);
            return Ok();
        }
        catch (ArgumentException excecao)
        {
            return BadRequest(new { mensagem = excecao.Message });
        }
    }
}