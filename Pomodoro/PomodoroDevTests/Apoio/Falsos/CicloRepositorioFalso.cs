using WebApplication1.Entities;
using WebApplication1.Intefaces.Repositorios;

namespace PomodoroDev.Tests.Apoio.Falsos;

public class CicloRepositorioFalso : ICicloRepository
{
    public Ciclo? CicloAtual { get; set; }
    public Ciclo? CicloCriado { get; set; }
    public int FocosConcluidosDesdeAUltimaPausaLonga { get; set; }

    public Task<Ciclo?> ObterCicloAtual() => Task.FromResult(CicloAtual);

    public Task CriarCiclo(Ciclo ciclo)
    {
        ciclo.CicloId = 99;
        CicloCriado = ciclo;
        return Task.CompletedTask;
    }

    public Task<Ciclo> FinalizarCiclo(Ciclo ciclo)
    {
        ciclo.DataHoraFim = DateTimeOffset.UtcNow;
        return Task.FromResult(ciclo);
    }

    public Task DefinirCicloComoConcluido(int cicloId)  => Task.CompletedTask;

    public Task<int> ConsultarQuantidadeDeFocosConcluidosDesdeAUltimaPausaLonga(int tarefaId) => Task.FromResult(FocosConcluidosDesdeAUltimaPausaLonga);

    public Task<List<Ciclo>> ObterCiclosConcluidosDeUmaTarefa(int tarefaId) => Task.FromResult(new List<Ciclo>());
}