using Microsoft.AspNetCore.Mvc;
using WebApplication1.Entities;
using WebApplication1.Intefaces.Core;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers;

public class TarefaController(ITarefaBusiness tarefaBusiness) : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<JsonResult> ListarTarefas()
    {
        var tarefas = await tarefaBusiness.ObterTarefas();
        var tarefasViewModel = tarefas.Select(MapearParaViewModel).ToList();
        return new JsonResult(tarefasViewModel);
    }

    [HttpPost]
    public JsonResult CriarTarefa([FromBody] TarefaViewModel tarefa)
    {
        tarefaBusiness.CriarTarefa(tarefa);
        return new JsonResult(tarefa);
    }

    [HttpPatch]
    public async Task<JsonResult> EditarTarefa([FromBody] TarefaViewModel tarefa)
    {
        await tarefaBusiness.AtualizarTarefa(tarefa);
        return new JsonResult(tarefa);
    }

    [HttpPost]
    public async Task<JsonResult> ArquivarTarefa([FromBody] int tarefaId)
    {
        await tarefaBusiness.ArquivarTarefa(tarefaId);
        return new JsonResult(tarefaId);
    }

    [HttpPost]
    public async Task<IActionResult> ExcluirTarefa([FromBody] int tarefaId)
    {
        try
        {
            await tarefaBusiness.ExcluirTarefa(tarefaId);
            return Ok(tarefaId);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { mensagem = "Tarefa não encontrada" });
        }
    }

    private static TarefaViewModel MapearParaViewModel(Tarefa tarefa)
    {
        return new TarefaViewModel
        {
            TarefaId = tarefa.TarefaId,
            NomeDaTarefa = tarefa.NomeDaTarefa,
            DuracaoFocoSegundos = tarefa.DuracaoFocoSegundos,
            DuracaoPausaLonga = tarefa.DuracaoPausaLonga,
            DuracaoPausaCurta = tarefa.DuracaoPausaCurta,
            CiclosParaPausaLonga = tarefa.CiclosParaPausaLonga,
            DataHoraInicio = tarefa.DataHoraInicio ?? default,
            DataHoraFim = tarefa.DataHoraFim,
            Arquivado = tarefa.Arquivado,
            Cor = tarefa.Cor
        };
    }
}