using WebApplication1.Intefaces.Core;
using WebApplication1.Intefaces.Repositorios;
using WebApplication1.ViewModels;
using WebApplication1.ViewModels.Enums;

namespace WebApplication1.Core;

public class CicloBusiness : ICicloBusiness
{

    private ICicloRepository _cicloRepository;
    private ITarefaRepository _tarefaRepository;
    public CicloBusiness(ICicloRepository cicloRepository, ITarefaRepository tarefaRepository)
    {
        _cicloRepository = cicloRepository;
        _tarefaRepository = tarefaRepository;
    }
    
    public void IniciarCiclo(TarefaViewModel tarefa, TiposDeCiclo tipoDoCiclo)
    {
        var cicloAtual = _cicloRepository.ObterCicloAtual(tarefa);
        var novoCiclo = new CicloViewModel
        {
            CicloId = cicloAtual.CicloId + 1,
            TarefaId = tarefa,
            DataHoraInicio = DateTime.Now,
            TipoDoCiclo = tipoDoCiclo,
            Concluido = false
        };
        _cicloRepository.CriarCiclo(novoCiclo);
    }

    public void Pausar(CicloViewModel cicloASerPausado)
    {
        FinalizarCiclo(cicloASerPausado);
        
        var ciclosDaTarefa = _cicloRepository.ObterCiclosConcluidosDeUmaTarefa(cicloASerPausado.TarefaId);

        var ultimoFocoConcluido = ConsultarUltimoFocoConcluido(ciclosDaTarefa);

        if (ultimoFocoConcluido == null)
        {
            IniciarCiclo(cicloASerPausado.TarefaId, TiposDeCiclo.PausaCurta);
            return;
        }
        
        var pausasConcluidasDesdeOUltimoFoco = ConsultarPausasConcluidasDesdeOUltimoFoco(ciclosDaTarefa, ultimoFocoConcluido);

        IniciarCiclo(
            cicloASerPausado.TarefaId,
            pausasConcluidasDesdeOUltimoFoco.Count() > cicloASerPausado.TarefaId.CiclosParaPausaLonga ?
                TiposDeCiclo.PausaLonga : TiposDeCiclo.PausaCurta
        );
        
    }

    private static IEnumerable<CicloViewModel> ConsultarPausasConcluidasDesdeOUltimoFoco(List<CicloViewModel> ciclosDaTarefa, CicloViewModel ultimoFocoConcluido)
    {
        return ciclosDaTarefa
            .Where(
                c => (c.TipoDoCiclo == TiposDeCiclo.PausaCurta) &&
                     c.DataHoraInicio > ultimoFocoConcluido.DataHoraInicio);
    }

    private static CicloViewModel? ConsultarUltimoFocoConcluido(List<CicloViewModel> ciclosDaTarefa)
    {
        return ciclosDaTarefa
            .Where(c =>
                (c.TipoDoCiclo == TiposDeCiclo.Focus) &&
                (c.Concluido == true))
            .OrderByDescending(c => c.DataHoraInicio)
            .FirstOrDefault();
    }

    public void Focar(CicloViewModel cicloASerFocado)
    {
        FinalizarCiclo(cicloASerFocado);
        
        IniciarCiclo(cicloASerFocado.TarefaId, TiposDeCiclo.PausaLonga);
    }

    public CicloViewModel ObterCicloAtual()
    {
        var tarefa = _tarefaRepository.ObterTarefaAtual();
        return _cicloRepository.ObterCicloAtual(tarefa);
    }

    public CicloViewModel MudarDeCiclo()
    {
        var tarefa = _tarefaRepository.ObterTarefaAtual();
        if (tarefa.DataHoraFim != null)
            throw new InvalidOperationException("A tarefa selecionada já foi finalizada");
        
        var cicloAtual = _cicloRepository.ObterCicloAtual(tarefa);
        if (cicloAtual.TipoDoCiclo == TiposDeCiclo.Focus)
            Pausar(cicloAtual);
        else
            Focar(cicloAtual);

        return _cicloRepository.ObterCicloAtual(tarefa);
    }
    
    public void FinalizarCiclo(CicloViewModel ciclo)
    {
        if ((ciclo.DataHoraFim != null) || (ciclo.Concluido == true))
            throw new InvalidOperationException("Ciclo já foi finalizado");
        
        ciclo.DataHoraFim = DateTime.Now;
        TimeSpan diferenca = ciclo.DataHoraFim.Value - ciclo.DataHoraInicio;
        
        var segundosDeDiferenca = diferenca.TotalSeconds;
        int segundosDeComparacao = ObterSegundosDeComparacao(ciclo);

        if (segundosDeDiferenca >= segundosDeComparacao)
            ciclo.Concluido = true;
        
    }

    private static int ObterSegundosDeComparacao(CicloViewModel ciclo)
    {
        return ciclo.TipoDoCiclo switch
        {
            TiposDeCiclo.Focus => ciclo.TarefaId.DuracaoFocoSegundos,
            TiposDeCiclo.PausaLonga => ciclo.TarefaId.DuracaoPausaLonga,
            TiposDeCiclo.PausaCurta => ciclo.TarefaId.DuracaoPausaCurta,
            _ => 0
        };
    }
}