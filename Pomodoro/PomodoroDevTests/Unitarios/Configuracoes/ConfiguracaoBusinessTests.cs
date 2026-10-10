using NUnit.Framework;
using PomodoroDev.Tests.Apoio.Falsos;
using WebApplication1.Core;
using WebApplication1.ViewModels;

namespace PomodoroDev.Tests.Unitarios.Configuracoes;

[TestFixture]
public class ConfiguracaoBusinessTests
{
    private ConfiguracaoRepositorioFalso _repositorio = null!;
    private ConfiguracaoBusiness _business = null!;

    [SetUp]
    public void Preparar()
    {
        _repositorio = new ConfiguracaoRepositorioFalso();
        _business = new ConfiguracaoBusiness(_repositorio);
    }

    [Test]
    public async Task ObterConfiguracao_SempreDeveRetornarValores()
    {
        // Act
        var resultado = await _business.ObterConfiguracao();

        // Assert
        Assert.That(resultado, Is.Not.Null);
        Assert.That(resultado.DuracaoFocoMinutos, Is.GreaterThan(0));
    }

    [Test]
    public async Task SalvarConfiguracao_ValoresValidos_DeveSalvarSemErro()
    {
        // Arrange
        var configuracao = new ConfiguracaoViewModel
        {
            DuracaoFocoMinutos = 50,
            DuracaoPausaCurtaMinutos = 10,
            DuracaoPausaLongaMinutos = 20,
            CiclosParaPausaLonga = 4,
            PausaLongaAtivada = true
        };

        // Act e Assert
        Assert.DoesNotThrowAsync(async () =>
            await _business.SalvarConfiguracao(configuracao));
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(91)]
    public void SalvarConfiguracao_FocoInvalido_DeveLancarExcecao(int focoInvalido)
    {
        // Arrange
        var configuracao = new ConfiguracaoViewModel
        {
            DuracaoFocoMinutos = focoInvalido,
            DuracaoPausaCurtaMinutos = 10,
            DuracaoPausaLongaMinutos = 20,
            CiclosParaPausaLonga = 4,
            PausaLongaAtivada = true
        };

        // Assert
        Assert.ThrowsAsync<ArgumentException>(async () =>
            await _business.SalvarConfiguracao(configuracao));
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(31)]
    public void SalvarConfiguracao_PausaCurtaInvalida_DeveLancarExcecao(int pausaCurtaInvalida)
    {
        // Arrange
        var configuracao = new ConfiguracaoViewModel
        {
            DuracaoFocoMinutos = 50,
            DuracaoPausaCurtaMinutos = pausaCurtaInvalida,
            DuracaoPausaLongaMinutos = 20,
            CiclosParaPausaLonga = 4,
            PausaLongaAtivada = true
        };

        // Assert
        Assert.ThrowsAsync<ArgumentException>(async () =>
            await _business.SalvarConfiguracao(configuracao));
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(61)]
    public void SalvarConfiguracao_PausaLongaInvalida_DeveLancarExcecao(int pausaLongaInvalida)
    {
        // Arrange
        var configuracao = new ConfiguracaoViewModel
        {
            DuracaoFocoMinutos = 50,
            DuracaoPausaCurtaMinutos = 10,
            DuracaoPausaLongaMinutos = pausaLongaInvalida,
            CiclosParaPausaLonga = 4,
            PausaLongaAtivada = true
        };

        // Assert
        Assert.ThrowsAsync<ArgumentException>(async () =>
            await _business.SalvarConfiguracao(configuracao));
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(11)]
    public void SalvarConfiguracao_CiclosInvalidos_DeveLancarExcecao(int ciclosInvalidos)
    {
        // Arrange
        var configuracao = new ConfiguracaoViewModel
        {
            DuracaoFocoMinutos = 50,
            DuracaoPausaCurtaMinutos = 10,
            DuracaoPausaLongaMinutos = 20,
            CiclosParaPausaLonga = ciclosInvalidos,
            PausaLongaAtivada = true
        };

        // Assert
        Assert.ThrowsAsync<ArgumentException>(async () =>
            await _business.SalvarConfiguracao(configuracao));
    }

    [Test]
    public void SalvarConfiguracao_PausaLongaMenorQueCurta_DeveLancarExcecao()
    {
        // Arrange
        var configuracao = new ConfiguracaoViewModel
        {
            DuracaoFocoMinutos = 50,
            DuracaoPausaCurtaMinutos = 20,
            DuracaoPausaLongaMinutos = 10,
            CiclosParaPausaLonga = 4,
            PausaLongaAtivada = true
        };

        // Assert
        Assert.ThrowsAsync<ArgumentException>(async () =>
            await _business.SalvarConfiguracao(configuracao));
    }

    [Test]
    public void SalvarConfiguracao_PausaLongaIgualACurta_DeveLancarExcecao()
    {
        // Arrange
        var configuracao = new ConfiguracaoViewModel
        {
            DuracaoFocoMinutos = 50,
            DuracaoPausaCurtaMinutos = 15,
            DuracaoPausaLongaMinutos = 15,
            CiclosParaPausaLonga = 4,
            PausaLongaAtivada = true
        };

        // Assert
        Assert.ThrowsAsync<ArgumentException>(async () =>
            await _business.SalvarConfiguracao(configuracao));
    }

    [Test]
    public async Task SalvarConfiguracao_PausaLongaDesativada_DeveAceitarPausaLongaMenorQueCurta()
    {
        // Arrange
        var configuracao = new ConfiguracaoViewModel
        {
            DuracaoFocoMinutos = 50,
            DuracaoPausaCurtaMinutos = 20,
            DuracaoPausaLongaMinutos = 10,
            CiclosParaPausaLonga = 4,
            PausaLongaAtivada = false
        };

        // Act e Assert
        Assert.DoesNotThrowAsync(async () =>
            await _business.SalvarConfiguracao(configuracao));
    }

    [Test]
    public async Task SalvarConfiguracao_ValoresValidos_DevePassarOsValoresParaORepositorio()
    {
        // Arrange
        var configuracao = new ConfiguracaoViewModel
        {
            DuracaoFocoMinutos = 50,
            DuracaoPausaCurtaMinutos = 10,
            DuracaoPausaLongaMinutos = 20,
            CiclosParaPausaLonga = 4,
            PausaLongaAtivada = true
        };

        // Act
        await _business.SalvarConfiguracao(configuracao);

        // Assert
        Assert.That(_repositorio.ConfiguracaoSalva, Is.Not.Null);
        Assert.That(_repositorio.ConfiguracaoSalva!.DuracaoFocoMinutos, Is.EqualTo(50));
        Assert.That(_repositorio.ConfiguracaoSalva.PausaLongaAtivada, Is.True);
    }
}