document.addEventListener("DOMContentLoaded", function () {
    var itensTarefa = document.querySelectorAll(".item-tarefa");
    var barraFoco = document.getElementById("barraFoco");
    var botaoIniciar = document.getElementById("botaoIniciar");
    var elementoTempoFoco = document.getElementById("tempoFoco");

    var telaListaTarefas = document.getElementById("telaListaTarefas");
    var telaCronometro = document.getElementById("telaCronometro");
    var nomeTarefaCronometro = document.getElementById("nomeTarefaCronometro");
    var tempoCronometro = document.getElementById("tempoCronometro");
    var estadoCronometro = document.getElementById("estadoCronometro");
    var circuloProgresso = document.getElementById("circuloProgresso");
    var botaoContinuar = document.getElementById("botaoContinuar");
    var botaoFim = document.getElementById("botaoFim");
    var botaoCancelar = document.getElementById("botaoCancelar");
    var botaoVoltarCronometro = document.getElementById("botaoVoltarCronometro");

    var segundosTotaisFoco = 90 * 60;
    var perimetroCirculo = 628;

    var tarefaSelecionada = null;
    var segundosRestantes = segundosTotaisFoco;
    var intervaloContagem = null;
    var contagemEmAndamento = false;
    var cronometroJaIniciado = false;

    function formatarTempo(segundos) {
        var minutos = Math.floor(segundos / 60).toString().padStart(2, "0");
        var segundosResto = (segundos % 60).toString().padStart(2, "0");
        return minutos + ":" + segundosResto;
    }

    function pararContagem() {
        clearInterval(intervaloContagem);
        intervaloContagem = null;
        contagemEmAndamento = false;
    }

    function atualizarExibicaoTempo() {
        var tempoFormatado = formatarTempo(segundosRestantes);
        elementoTempoFoco.textContent = tempoFormatado;
        tempoCronometro.textContent = tempoFormatado;
        var fracaoRestante = segundosRestantes / segundosTotaisFoco;
        circuloProgresso.style.strokeDashoffset = (perimetroCirculo * fracaoRestante).toString();
    }

    function selecionarTarefa(item) {
        itensTarefa.forEach(function (t) { t.classList.remove("tarefa-selecionada"); });
        tarefaSelecionada = item;
        item.classList.add("tarefa-selecionada");

        if (!cronometroJaIniciado) {
            segundosRestantes = segundosTotaisFoco;
            atualizarExibicaoTempo();
        }
    }

    function desselecionarTarefa() {
        itensTarefa.forEach(function (t) { t.classList.remove("tarefa-selecionada"); });
        tarefaSelecionada = null;

        if (!cronometroJaIniciado) {
            barraFoco.classList.remove("barra-foco-visivel");
        }
    }

    itensTarefa.forEach(function (item) {
        item.addEventListener("click", function () {
            if (tarefaSelecionada === item) {
                desselecionarTarefa();
            } else {
                selecionarTarefa(item);
            }
        });
    });

    function iniciarContagem() {
        pararContagem();

        contagemEmAndamento = true;
        cronometroJaIniciado = true;
        estadoCronometro.textContent = "Em foco";
        botaoContinuar.textContent = "Pausar";

        intervaloContagem = setInterval(function () {
            segundosRestantes--;
            atualizarExibicaoTempo();

            if (segundosRestantes <= 0) {
                pararContagem();
                estadoCronometro.textContent = "Concluído";
                botaoContinuar.textContent = "Continuar";
            }
        }, 1000);
    }

    function pausarContagem() {
        pararContagem();
        estadoCronometro.textContent = "Pausado";
        botaoContinuar.textContent = "Continuar";
    }

    function abrirTelaCronometro() {
        nomeTarefaCronometro.textContent = tarefaSelecionada.dataset.nomeTarefa;
        telaListaTarefas.style.display = "none";
        telaCronometro.classList.add("tela-cronometro-visivel");
        atualizarExibicaoTempo();
    }

    function voltarParaListaMantendoCronometro() {
        telaCronometro.classList.remove("tela-cronometro-visivel");
        telaListaTarefas.style.display = "flex";
        barraFoco.classList.add("barra-foco-visivel");
    }

    function encerrarCronometroEVoltar() {
        pararContagem();
        cronometroJaIniciado = false;
        segundosRestantes = segundosTotaisFoco;
        estadoCronometro.textContent = "Pausado";
        botaoContinuar.textContent = "Continuar";
        atualizarExibicaoTempo();

        telaCronometro.classList.remove("tela-cronometro-visivel");
        telaListaTarefas.style.display = "flex";
        barraFoco.classList.remove("barra-foco-visivel");
        desselecionarTarefa();
    }

    botaoIniciar.addEventListener("click", function () {
        if (!tarefaSelecionada) {
            return;
        }
        abrirTelaCronometro();
        iniciarContagem();
    });

    botaoContinuar.addEventListener("click", function () {
        if (contagemEmAndamento) {
            pausarContagem();
        } else {
            iniciarContagem();
        }
    });

    botaoFim.addEventListener("click", encerrarCronometroEVoltar);
    botaoCancelar.addEventListener("click", encerrarCronometroEVoltar);
    botaoVoltarCronometro.addEventListener("click", voltarParaListaMantendoCronometro);
});