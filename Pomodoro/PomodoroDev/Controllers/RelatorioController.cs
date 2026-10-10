using Microsoft.AspNetCore.Mvc;
using WebApplication1.Intefaces.Core;

namespace WebApplication1.Controllers;

public class RelatorioController(IRelatorioBusiness relatorioBusiness) : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ObterResumo(DateTimeOffset inicio, DateTimeOffset fim)
    {
        try
        {
            var resumo = await relatorioBusiness.ObterResumoDoPeriodo(inicio, fim);
            return Ok(resumo);
        }
        catch (ArgumentException excecao)
        {
            return BadRequest(new { mensagem = excecao.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> ObterDetalhePorTarefa(DateTimeOffset inicio, DateTimeOffset fim)
    {
        try
        {
            var detalhe = await relatorioBusiness.ObterDetalhePorTarefa(inicio, fim);
            return Ok(detalhe);
        }
        catch (ArgumentException excecao)
        {
            return BadRequest(new { mensagem = excecao.Message });
        }
    }
}