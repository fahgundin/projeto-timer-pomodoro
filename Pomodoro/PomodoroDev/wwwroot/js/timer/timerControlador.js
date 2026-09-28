angular.module('appTimer').controller('ControladorTimer', function($scope, $http) {

    $scope.listaDeTarefas = [
        { tarefaId: 1, nomeDaTarefa: 'Estudar', duracaoFocoSegundos: 0, arquivado: false, segundosFocadosHoje: 0 },
        { tarefaId: 2, nomeDaTarefa: 'Estudar C#', duracaoFocoSegundos: 0, arquivado: false, segundosFocadosHoje: 0 },
        { tarefaId: 3, nomeDaTarefa: 'Ler', duracaoFocoSegundos: 0, arquivado: false, segundosFocadosHoje: 0 }
    ];

    $scope.configuracao = {
        duracaoFocoSegundos: 5,
        duracaoPausaCurtaSegundos: 5,
        duracaoPausaLongaSegundos: 10,
        ciclosParaPausaLonga: 2
    };
    
    const deslocamentoMinimoCirculo = 8;

    $scope.tarefaSelecionada = null;
    $scope.telaAtual = 'lista';
    $scope.cicloDoServidor = null;

    $scope.tipoDoCicloAtual = 'foco';
    $scope.contagemEmAndamento = false;
    $scope.segundosRestantes = $scope.configuracao.duracaoFocoSegundos;
    $scope.segundosTotaisDoCiclo = $scope.configuracao.duracaoFocoSegundos;
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

    function carregarCicloAtualDoServidor() {
        $http.get('/Ciclo/ObterCicloAtual').then(function(resposta) {
            const ciclo = resposta.data;
            $scope.cicloDoServidor = ciclo;

            $scope.configuracao.duracaoFocoSegundos = ciclo.tarefaId.duracaoFocoSegundos;
            $scope.configuracao.duracaoPausaCurtaSegundos = ciclo.tarefaId.duracaoPausaCurta;
            $scope.configuracao.duracaoPausaLongaSegundos = ciclo.tarefaId.duracaoPausaLonga;
            $scope.configuracao.ciclosParaPausaLonga = ciclo.tarefaId.ciclosParaPausaLonga;

            $scope.listaDeTarefas.forEach(function(tarefa) {
                tarefa.duracaoFocoSegundos = $scope.configuracao.duracaoFocoSegundos;
            });

            if ($scope.telaAtual === 'lista') {
                prepararCiclo('foco');
            }
        }, function() {
            alert('Não foi possível obter o ciclo atual');
        });
    }

    function avisarServidorDaTrocaDeCiclo() {
        $http.get('/Ciclo/PausarOuFocar').then(function(resposta) {
            $scope.cicloDoServidor = resposta.data;
        }, function() {
            alert('Não foi possível registrar a troca de fase');
        });
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
        if (tipo === 'foco') { return $scope.configuracao.duracaoFocoSegundos; }
        if (tipo === 'pausaCurta') { return $scope.configuracao.duracaoPausaCurtaSegundos; }
        return $scope.configuracao.duracaoPausaLongaSegundos;
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
            $scope.tarefaSelecionada.segundosFocadosHoje += $scope.configuracao.duracaoFocoSegundos;
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
        avisarServidorDaTrocaDeCiclo();

        if (tipoQueTerminou === 'foco') {
            ciclosFocoConcluidos++;
            if (ciclosFocoConcluidos >= $scope.configuracao.ciclosParaPausaLonga) {
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

    carregarCicloAtualDoServidor();

});