using WebApplication1.Intefaces.Core;
using WebApplication1.Intefaces.Repositorios;
using WebApplication1.ViewModels;
using WebApplication1.ViewModels.Enums;

namespace WebApplication1.Mocks;

public class MockCicloRepository : ICicloRepository 
{
    private static List<CicloViewModel>? _ciclos;
    
    public List<CicloViewModel> ObterCiclosConcluidosDeUmaTarefa(TarefaViewModel tarefa)
    {
        if (_ciclos == null)
            return new List<CicloViewModel>();
        var ciclos = _ciclos
            .Where(c => c.TarefaId == tarefa)
            .Where(c => c.Concluido = true)
            .ToList();

        return ciclos;
    }

    public CicloViewModel ObterCicloAtual(TarefaViewModel tarefaAtual)
    {
        if (_ciclos == null)
        {
            return CriarCicloCasoNenhumExista(tarefaAtual);
        }

        CicloViewModel cicloAtual = _ciclos!
            .Where(c => c.TarefaId == tarefaAtual)
            .OrderByDescending(c => c.DataHoraInicio)
            .FirstOrDefault(c => c.DataHoraFim == null) ?? CriarCicloCasoNenhumExista(tarefaAtual);
        
        return cicloAtual;
    }

    private CicloViewModel CriarCicloCasoNenhumExista(TarefaViewModel tarefaAtual)
    {
        var novoCiclo = new CicloViewModel
        {
            CicloId = 0,
            TarefaId = tarefaAtual,
            DataHoraInicio = DateTimeOffset.Now,
            Concluido = false,
            TipoDoCiclo = TiposDeCiclo.Focus,
        };
        CriarCiclo(novoCiclo);
        return novoCiclo;
    }

    public void DefinirCicloComoConcluido(CicloViewModel ciclo)
    {
        if (_ciclos == null)
            return;
        var cicloDaLista = _ciclos
            .FirstOrDefault((c) => c.CicloId == ciclo.CicloId);

        if (cicloDaLista == null)
            throw new InvalidOperationException("Ciclo Informado não existe");
        
        cicloDaLista.Concluido = true;
    }

    public void CriarCiclo(CicloViewModel ciclo)
    {
        if (_ciclos == null)
            _ciclos = new List<CicloViewModel>();
        _ciclos.Add(ciclo);
    }
    
}