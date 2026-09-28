angular.module('appTimer').controller('ControladorTimer', function($scope) {

    $scope.listaDeTarefas = [
        { tarefaId: 1, nomeDaTarefa: 'Estudar', duracaoFocoSegundos: 5, arquivado: false, segundosFocadosHoje: 0 },
        { tarefaId: 2, nomeDaTarefa: 'Estudar C#', duracaoFocoSegundos: 5, arquivado: false, segundosFocadosHoje: 0 },
        { tarefaId: 3, nomeDaTarefa: 'Ler', duracaoFocoSegundos: 5, arquivado: false, segundosFocadosHoje: 0 }
    ];

    const duracaoFocoSegundos = 5;
    const duracaoPausaCurtaSegundos = 5;
    const duracaoPausaLongaSegundos = 10;
    const ciclosParaPausaLonga = 2;
    const deslocamentoMinimoCirculo = 8;

    $scope.tarefaSelecionada = null;
    $scope.telaAtual = 'lista';

    $scope.tipoDoCicloAtual = 'foco';
    $scope.contagemEmAndamento = false;
    $scope.segundosRestantes = duracaoFocoSegundos;
    $scope.segundosTotaisDoCiclo = duracaoFocoSegundos;
    $scope.tempoFormatado = '00:00';
    $scope.estadoCronometro = 'Pausado';
    $scope.deslocamentoMinimoCirculo = deslocamentoMinimoCirculo;

    let cronometroInterno = null;
    let ciclosFocoConcluidos = 0;
    let dataDoUltimoRegistro = obterDataDeHoje();
    let idDoCicloAtual = 0;
    let idDoUltimoCicloRegistrado = -1;

    function obterDataDeHoje() {
        return new Date().toDateString();
    }

    function garantirContadorDoDiaAtual(tarefa) {
        const hoje = obterDataDeHoje();
        if (dataDoUltimoRegistro !== hoje) {
            $scope.listaDeTarefas.forEach(function(tarefaDaLista) {
                tarefaDaLista.segundosFocadosHoje = 0;
            });
            dataDoUltimoRegistro = hoje;
        }
        if (typeof tarefa.segundosFocadosHoje !== 'number') {
            tarefa.segundosFocadosHoje = 0;
        }
    }

    $scope.formatarSegundosEmMinutos = function(segundos) {
        const minutos = Math.floor(segundos / 60);
        return minutos + 'min';
    };

    function formatarSegundosEmContagem(segundos) {
        const minutos = Math.floor(segundos / 60).toString().padStart(2, '0');
        const segundosResto = (segundos % 60).toString().padStart(2, '0');
        return minutos + ':' + segundosResto;
    }

    function duracaoDoTipo(tipo) {
        if (tipo === 'foco') { return duracaoFocoSegundos; }
        if (tipo === 'pausaCurta') { return duracaoPausaCurtaSegundos; }
        return duracaoPausaLongaSegundos;
    }

    function textoDoTipo(tipo) {
        if (tipo === 'foco') { return 'Em foco'; }
        if (tipo === 'pausaCurta') { return 'Pausa curta'; }
        return 'Pausa longa';
    }

    function registrarCicloConcluido(tipo) {
        if (idDoUltimoCicloRegistrado === idDoCicloAtual) {
            return;
        }
        idDoUltimoCicloRegistrado = idDoCicloAtual;

        if (tipo === 'foco' && $scope.tarefaSelecionada) {
            garantirContadorDoDiaAtual($scope.tarefaSelecionada);
            $scope.tarefaSelecionada.segundosFocadosHoje += duracaoFocoSegundos;
        }
    }

    function prepararCiclo(tipo) {
        const duracaoDesteCiclo = duracaoDoTipo(tipo);
        idDoCicloAtual++;
        $scope.tipoDoCicloAtual = tipo;
        $scope.segundosTotaisDoCiclo = duracaoDesteCiclo;
        $scope.segundosRestantes = duracaoDesteCiclo;
        $scope.tempoFormatado = formatarSegundosEmContagem($scope.segundosRestantes);
        $scope.estadoCronometro = textoDoTipo(tipo);
    }

    function pararCronometroInterno() {
        if (cronometroInterno !== null) {
            clearInterval(cronometroInterno);
            cronometroInterno = null;
        }
        $scope.contagemEmAndamento = false;
    }

    function avancarProximoCiclo() {
        const tipoQueTerminou = $scope.tipoDoCicloAtual;
        registrarCicloConcluido(tipoQueTerminou);

        if (tipoQueTerminou === 'foco') {
            ciclosFocoConcluidos++;
            if (ciclosFocoConcluidos >= ciclosParaPausaLonga) {
                ciclosFocoConcluidos = 0;
                prepararCiclo('pausaLonga');
                iniciarContagem();
            } else {
                prepararCiclo('pausaCurta');
                iniciarContagem();
            }
        } else if (tipoQueTerminou === 'pausaCurta') {
            prepararCiclo('foco');
            iniciarContagem();
        } else {
            finalizarCicloCompleto();
        }
    }

    function finalizarCicloCompleto() {
        pararCronometroInterno();
        $scope.telaAtual = 'lista';
    }

    function iniciarContagem() {
        if (cronometroInterno !== null) {
            return;
        }
        $scope.contagemEmAndamento = true;
        $scope.estadoCronometro = textoDoTipo($scope.tipoDoCicloAtual);

        cronometroInterno = setInterval(function() {
            $scope.segundosRestantes--;
            $scope.tempoFormatado = formatarSegundosEmContagem($scope.segundosRestantes);

            if ($scope.segundosRestantes <= 0) {
                pararCronometroInterno();
                $scope.$apply(avancarProximoCiclo);
            } else {
                $scope.$apply();
            }
        }, 1000);
    }

    $scope.selecionarTarefa = function(tarefa) {
        if ($scope.tarefaSelecionada === tarefa) {
            $scope.tarefaSelecionada = null;
            return;
        }
        garantirContadorDoDiaAtual(tarefa);
        $scope.tarefaSelecionada = tarefa;
    };

    $scope.iniciarCiclo = function() {
        if (!$scope.tarefaSelecionada) {
            alert('Selecione uma tarefa antes de iniciar');
            return;
        }
        $scope.telaAtual = 'cronometro';
        ciclosFocoConcluidos = 0;
        prepararCiclo('foco');
        iniciarContagem();
    };

    $scope.alternarPausa = function() {
        if ($scope.contagemEmAndamento) {
            pararCronometroInterno();
            $scope.estadoCronometro = 'Pausado';
        } else {
            iniciarContagem();
        }
    };

    $scope.encerrarCiclo = function() {
        pararCronometroInterno();
        $scope.telaAtual = 'lista';
    };

    $scope.cancelarCiclo = function() {
        pararCronometroInterno();
        prepararCiclo('foco');
        $scope.telaAtual = 'lista';
    };

});