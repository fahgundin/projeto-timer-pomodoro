using Microsoft.AspNetCore.Mvc;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers;

public class TarefaController : Controller
{
    [HttpPost]
    public async Task<JsonResult> CriarTarefa([FromBody] TarefaViewModel tarefa)
    {
        return new JsonResult(tarefa);
    }

    [HttpGet]
    public async Task<JsonResult> ListarTarefas()
    {
        return new JsonResult("teste");
    }

    [HttpPost]
    public async Task<JsonResult> ArquivarTarefa([FromBody] int tarefaId)
    {
        return new JsonResult(tarefaId);
    }

    [HttpPatch]
    public async Task<JsonResult> EditarTarefa([FromBody] TarefaViewModel tarefa)
    {
        return new JsonResult(tarefa);
    }
    
    
}