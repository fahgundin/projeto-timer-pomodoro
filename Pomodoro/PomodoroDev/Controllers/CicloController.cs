using Microsoft.AspNetCore.Mvc;
using WebApplication1.Intefaces.Core;

namespace WebApplication1.Controllers;

public class CicloController : Controller
{
    private readonly ICicloBusiness _cicloBusiness;
        
    public CicloController(ICicloBusiness cicloBusiness)
    {
        _cicloBusiness = cicloBusiness;
    }

    [HttpGet]
    public async Task<JsonResult> PausarOuFocar()
    {
        var ciclo = await _cicloBusiness.MudarDeCiclo();
        return new JsonResult(ciclo);
    }

    [HttpGet]
    public async Task<JsonResult> ObterCicloAtual()
    {
        var ciclo = await _cicloBusiness.ObterCicloAtual();
        return new JsonResult(ciclo);
    }

    [HttpPost]
    public async Task<IActionResult> FinalizarCicloAtual()
    {
        var cicloFinalizado = await _cicloBusiness.FinalizarCicloAtual();
        return Ok(cicloFinalizado);
    }

    [HttpPost]
    public async Task<IActionResult> IniciarCiclo([FromQuery] int tarefaId)
    {
        var cicloIniciado = await _cicloBusiness.IniciarCiclo(tarefaId);
        return Ok(cicloIniciado);
    }
}