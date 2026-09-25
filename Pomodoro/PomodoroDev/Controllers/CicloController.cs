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
    public JsonResult PausarOuFocar()
    {
        var ciclo = _cicloBusiness.MudarDeCiclo();
        return new JsonResult(ciclo);
    }

    [HttpGet]
    public JsonResult ObterCicloAtual()
    {
        var ciclo = _cicloBusiness.ObterCicloAtual();
        return new JsonResult(ciclo);
    }
}