using WebApplication1.Entities;
using WebApplication1.Intefaces.Core;
using WebApplication1.Intefaces.Repositorios;
using WebApplication1.ViewModels;
using WebApplication1.ViewModels.Enums;

namespace WebApplication1.Core;

public class CicloBusiness(ICicloRepository cicloRepository) : ICicloBusiness
{
    
    private Ciclo IniciarCiclo(int tarefaId, TiposDeCiclo tipoDoCiclo)
    {
        var novoCiclo = new Ciclo
        {
            TarefaId = tarefaId,
            DataHoraInicio = DateTimeOffset.UtcNow,
            TipoDoCiclo = (int) tipoDoCiclo,
            Concluido = false
        };
        cicloRepository.CriarCiclo(novoCiclo);
        return novoCiclo;
    }

    private async Task<Ciclo> Pausar(Ciclo cicloASerPausado)
    {
        FinalizarCiclo(cicloASerPausado);

        var focosConcluidosDesdeAUltimaPausaLonga =
            await cicloRepository.ConsultarQuantidadeDeFocosConcluidosDesdeAUltimaPausaLonga(cicloASerPausado.TarefaId);

        var tipoDaPausa = focosConcluidosDesdeAUltimaPausaLonga >= cicloASerPausado.Tarefa.CiclosParaPausaLonga
            ? TiposDeCiclo.PausaLonga
            : TiposDeCiclo.PausaCurta;

        return IniciarCiclo(cicloASerPausado.TarefaId, tipoDaPausa);
    }

    private Ciclo Focar(Ciclo cicloASerFocado)
    {
        FinalizarCiclo(cicloASerFocado);
        
        return IniciarCiclo(cicloASerFocado.TarefaId, TiposDeCiclo.Focus);
    }

    public async Task<Ciclo?> ObterCicloAtual()
    {
        return await cicloRepository.ObterCicloAtual();
    }

    public async Task<Ciclo> MudarDeCiclo()
    {
        
        var cicloAtual = await ObterCicloAtual();
        
        if (cicloAtual == null)
            throw new KeyNotFoundException("Não existem ciclos abertos");
        
        if (cicloAtual.TipoDoCiclo == (int) TiposDeCiclo.Focus)
            return await Pausar(cicloAtual);
        else
            return Focar(cicloAtual);
    }

    public async Task<Ciclo> FinalizarCicloAtual()
    {
        var cicloAtual = await ObterCicloAtual();
        if (cicloAtual == null)
            throw new KeyNotFoundException("Não existem ciclos abertos");
        return await cicloRepository.FinalizarCiclo(cicloAtual);
    }

    public async Task<Ciclo> IniciarCiclo(int tarefaId)
    {
        var novoCiclo = IniciarCiclo(tarefaId, TiposDeCiclo.Focus);

        return novoCiclo;
    }

    private void FinalizarCiclo(Ciclo ciclo)
    {
        if ((ciclo.DataHoraFim != null) || (ciclo.Concluido == true))
            throw new InvalidOperationException("Ciclo já foi finalizado");

        cicloRepository.FinalizarCiclo(ciclo);

        if (ciclo.DataHoraFim == null || ciclo.DataHoraInicio == null)
            return;
        
        TimeSpan diferenca = (ciclo.DataHoraFim.Value - ciclo.DataHoraInicio.Value);
        
        var segundosDeDiferenca = diferenca.TotalSeconds;
        int segundosDeComparacao = ObterSegundosDeComparacao(ciclo);

        if (segundosDeDiferenca >= segundosDeComparacao)
            cicloRepository.DefinirCicloComoConcluido(ciclo.CicloId);
    }

    private static int ObterSegundosDeComparacao(Ciclo ciclo)
    {
        return ciclo.TipoDoCiclo switch
        {
            (int) TiposDeCiclo.Focus => ciclo.Tarefa.DuracaoFocoSegundos,
            (int) TiposDeCiclo.PausaLonga => ciclo.Tarefa.DuracaoPausaLonga,
            (int) TiposDeCiclo.PausaCurta => ciclo.Tarefa.DuracaoPausaCurta,
            _ => 0
        };
    }
}