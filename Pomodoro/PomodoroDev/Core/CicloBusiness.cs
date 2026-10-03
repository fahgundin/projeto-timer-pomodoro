using WebApplication1.Entities;
using WebApplication1.Intefaces.Core;
using WebApplication1.Intefaces.Repositorios;
using WebApplication1.ViewModels;
using WebApplication1.ViewModels.Enums;

namespace WebApplication1.Core;

public class CicloBusiness(ICicloRepository cicloRepository) : ICicloBusiness
{
    private async Task<Ciclo> IniciarCiclo(int tarefaId, TiposDeCiclo tipoDoCiclo)
    {
        var novoCiclo = new Ciclo
        {
            TarefaId = tarefaId,
            DataHoraInicio = DateTimeOffset.UtcNow,
            TipoDoCiclo = (int)tipoDoCiclo,
            Concluido = false
        };
        await cicloRepository.CriarCiclo(novoCiclo);
        return novoCiclo;
    }

    private async Task<Ciclo> Pausar(Ciclo cicloASerPausado)
    {
        await FinalizarCiclo(cicloASerPausado);

        var focosConcluidosDesdeAUltimaPausaLonga =
            await cicloRepository.ConsultarQuantidadeDeFocosConcluidosDesdeAUltimaPausaLonga(cicloASerPausado.TarefaId);

        var tipoDaPausa = focosConcluidosDesdeAUltimaPausaLonga >= cicloASerPausado.Tarefa.CiclosParaPausaLonga
            ? TiposDeCiclo.PausaLonga
            : TiposDeCiclo.PausaCurta;

        return await IniciarCiclo(cicloASerPausado.TarefaId, tipoDaPausa);
    }

    private async Task<Ciclo> Focar(Ciclo cicloASerFocado)
    {
        await FinalizarCiclo(cicloASerFocado);

        return await IniciarCiclo(cicloASerFocado.TarefaId, TiposDeCiclo.Focus);
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

        if (cicloAtual.TipoDoCiclo == (int)TiposDeCiclo.Focus)
            return await Pausar(cicloAtual);
        else
            return await Focar(cicloAtual);
    }

    public async Task<Ciclo> FinalizarCicloAtual()
    {
        var cicloAtual = await ObterCicloAtual();
        if (cicloAtual == null)
            throw new KeyNotFoundException("Não existem ciclos abertos");

        await FinalizarCiclo(cicloAtual);
        return cicloAtual;
    }

    public async Task<Ciclo> IniciarCiclo(int tarefaId)
    {
        return await IniciarCiclo(tarefaId, TiposDeCiclo.Focus);
    }

    private async Task FinalizarCiclo(Ciclo ciclo)
    {
        if ((ciclo.DataHoraFim != null) || (ciclo.Concluido == true))
            throw new InvalidOperationException("Ciclo já foi finalizado");

        await cicloRepository.FinalizarCiclo(ciclo);

        if (ciclo.DataHoraFim == null || ciclo.DataHoraInicio == null)
            return;

        TimeSpan diferenca = ciclo.DataHoraFim.Value - ciclo.DataHoraInicio.Value;

        if (diferenca.TotalSeconds >= ciclo.DuracaoPlanejadaSegundos)
            await cicloRepository.DefinirCicloComoConcluido(ciclo.CicloId);
    }
}