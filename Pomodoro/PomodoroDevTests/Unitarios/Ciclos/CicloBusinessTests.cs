using NUnit.Framework;
using PomodoroDev.Tests.Apoio.Falsos;
using WebApplication1.Core;
using WebApplication1.Entities;
using WebApplication1.ViewModels.Enums;

namespace PomodoroDev.Tests.Unitarios.Ciclos;

[TestFixture]
public class CicloBusinessTests
{
    private CicloRepositorioFalso _repositorio = null!;
    private CicloBusiness _business = null!;

    [SetUp]
    public void Preparar()
    {
        _repositorio = new CicloRepositorioFalso();
        _business = new CicloBusiness(_repositorio);
    }

    [Test]
    public async Task MudarDeCiclo_ComFocoAberto_DeveDevolvePausaCurta()
    {
        // Arrange
        var tarefa = new Tarefa
        {
            TarefaId = 1,
            NomeDaTarefa = "Estudar C#",
            DuracaoFocoSegundos = 3000,
            DuracaoPausaCurta = 600,
            DuracaoPausaLonga = 1200,
            CiclosParaPausaLonga = 4
        };

        _repositorio.CicloAtual = new Ciclo
        {
            CicloId = 1,
            TarefaId = tarefa.TarefaId,
            TipoDoCiclo = (int) TiposDeCiclo.Focus,
            DataHoraInicio = DateTimeOffset.UtcNow.AddHours(-1),
            Concluido = false,
            DuracaoPlanejadaSegundos = 3000,
            Tarefa = tarefa
        };

        _repositorio.FocosConcluidosDesdeAUltimaPausaLonga = 0;

        // Act
        var resultado = await _business.MudarDeCiclo();

        // Assert
        Assert.That(resultado.TipoDoCiclo, Is.EqualTo((int) TiposDeCiclo.PausaCurta));
    }

    [Test]
    public async Task MudarDeCiclo_ComFocoAbertoECiclosCompletos_DeveDevolvePausaLonga()
    {
        // Arrange
        var tarefa = new Tarefa
        {
            TarefaId = 1,
            NomeDaTarefa = "Estudar C#",
            DuracaoFocoSegundos = 3000,
            DuracaoPausaCurta = 600,
            DuracaoPausaLonga = 1200,
            CiclosParaPausaLonga = 4
        };

        _repositorio.CicloAtual = new Ciclo
        {
            CicloId = 1,
            TarefaId = tarefa.TarefaId,
            TipoDoCiclo = (int) TiposDeCiclo.Focus,
            DataHoraInicio = DateTimeOffset.UtcNow.AddHours(-1),
            Concluido = false,
            DuracaoPlanejadaSegundos = 3000,
            Tarefa = tarefa
        };

        _repositorio.FocosConcluidosDesdeAUltimaPausaLonga = 4;

        // Act
        var resultado = await _business.MudarDeCiclo();

        // Assert
        Assert.That(resultado.TipoDoCiclo, Is.EqualTo((int) TiposDeCiclo.PausaLonga));
    }

    [Test]
    public async Task MudarDeCiclo_ComPausaAberta_DeveDevolverFoco()
    {
        // Arrange
        var tarefa = new Tarefa
        {
            TarefaId = 1,
            NomeDaTarefa = "Estudar C#",
            DuracaoFocoSegundos = 3000,
            DuracaoPausaCurta = 600,
            DuracaoPausaLonga = 1200,
            CiclosParaPausaLonga = 4
        };

        _repositorio.CicloAtual = new Ciclo
        {
            CicloId = 1,
            TarefaId = tarefa.TarefaId,
            TipoDoCiclo = (int) TiposDeCiclo.PausaCurta,
            DataHoraInicio = DateTimeOffset.UtcNow.AddMinutes(-5),
            Concluido = false,
            DuracaoPlanejadaSegundos = 600,
            Tarefa = tarefa
        };

        // Act
        var resultado = await _business.MudarDeCiclo();

        // Assert
        Assert.That(resultado.TipoDoCiclo, Is.EqualTo((int) TiposDeCiclo.Focus));
    }

    [Test]
    public void MudarDeCiclo_SemCicloAberto_DeveLancarExcecao()
    {
        // Arrange
        _repositorio.CicloAtual = null;

        // Assert
        Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await _business.MudarDeCiclo());
    }
}