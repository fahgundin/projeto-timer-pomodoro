angular.module('appTimer').controller('ControladorTimer', function($scope, $http) {

    const deslocamentoMinimoCirculo = 8;

    const tiposDeCicloDoServidor = {
        0: 'foco',
        1: 'pausaLonga',
        2: 'pausaCurta'
    };

    $scope.listaDeTarefas = [];
    $scope.tarefasCarregadas = false;
    $scope.tarefaSelecionada = null;
    $scope.telaAtual = 'lista';

    $scope.tipoDoCicloAtual = 'foco';
    $scope.contagemEmAndamento = false;
    $scope.segundosRestantes = 0;
    $scope.segundosTotaisDoCiclo = 1;
    $scope.tempoFormatado = '00:00';
    $scope.estadoCronometro = 'Pausado';
    $scope.deslocamentoMinimoCirculo = deslocamentoMinimoCirculo;

    let cronometroInterno = null;
    let aguardandoServidor = false;

    function aguardarServidor(requisicao) {
        aguardandoServidor = true;
        return requisicao.finally(function() {
            aguardandoServidor = false;
        });
    }

    function carregarTarefasDoServidor() {
        $http.get('/Tarefa/ListarTarefas').then(function(resposta) {
            $scope.listaDeTarefas = resposta.data.filter(function(tarefa) {
                return !tarefa.arquivado;
            });
            $scope.tarefasCarregadas = true;
        }, function() {
            alert('Não foi possível carregar as tarefas');
        });
    }

    function encerrarCiclosEsquecidosNoServidor() {
        return $http.get('/Ciclo/ObterCicloAtual').then(function(resposta) {
            if (!resposta.data) {
                return;
            }
            return $http.post('/Ciclo/FinalizarCicloAtual').then(encerrarCiclosEsquecidosNoServidor);
        });
    }

    $scope.formatarDuracaoDaTarefa = function(segundos) {
        if (!segundos) { return '0s'; }
        if (segundos % 60 === 0) { return (segundos / 60) + 'min'; }
        return segundos + 's';
    };

    function formatarSegundosEmContagem(segundos) {
        const minutos = Math.floor(segundos / 60).toString().padStart(2, '0');
        const segundosResto = (segundos % 60).toString().padStart(2, '0');
        return minutos + ':' + segundosResto;
    }

    function duracaoDoTipo(tipo) {
        if (tipo === 'foco') { return $scope.tarefaSelecionada.duracaoFocoSegundos; }
        if (tipo === 'pausaCurta') { return $scope.tarefaSelecionada.duracaoPausaCurta; }
        return $scope.tarefaSelecionada.duracaoPausaLonga;
    }

    function textoDoTipo(tipo) {
        if (tipo === 'foco') { return 'Em foco'; }
        if (tipo === 'pausaCurta') { return 'Pausa curta'; }
        return 'Pausa longa';
    }

    function prepararCiclo(tipo) {
        const duracaoDesteCiclo = duracaoDoTipo(tipo);
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

    function finalizarCicloCompleto() {
        pararCronometroInterno();
        $scope.telaAtual = 'lista';
    }

    function avancarProximoCiclo() {

        aguardarServidor($http.get('/Ciclo/PausarOuFocar')).then(function(resposta) {
            const proximoTipo = tiposDeCicloDoServidor[resposta.data.tipoDoCiclo];
            console.log(resposta.data)
            if (!proximoTipo) {
                finalizarCicloCompleto();
                alert('O servidor devolveu uma fase desconhecida');
                return;
            }
            prepararCiclo(proximoTipo);
            iniciarContagem();
        }, function() {
            finalizarCicloCompleto();
            alert('Não foi possível registrar a troca de fase');
        });
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

    function finalizarCicloAbertoNoServidor() {
        pararCronometroInterno();
        aguardarServidor($http.post('/Ciclo/FinalizarCicloAtual')).then(function() {
            $scope.telaAtual = 'lista';
        }, function() {
            $scope.telaAtual = 'lista';
            alert('Não foi possível encerrar o ciclo no servidor');
        });
    }

    $scope.selecionarTarefa = function(tarefa) {
        if ($scope.tarefaSelecionada === tarefa) {
            $scope.tarefaSelecionada = null;
            return;
        }
        $scope.tarefaSelecionada = tarefa;
    };

    $scope.iniciarCiclo = function() {
        if (!$scope.tarefaSelecionada) {
            alert('Selecione uma tarefa antes de iniciar');
            return;
        }
        if (aguardandoServidor) {
            return;
        }

        const tarefaId = $scope.tarefaSelecionada.tarefaId;
        
        

        aguardarServidor(
            encerrarCiclosEsquecidosNoServidor().then(function() {
                return $http.post('/Ciclo/IniciarCiclo', null, { params: { tarefaId: tarefaId } });
            })
        ).then(function() {
            $scope.telaAtual = 'cronometro';
            prepararCiclo('foco');
            iniciarContagem();
        }, function() {
            alert('Não foi possível iniciar o ciclo');
        });
    };

    $scope.alternarPausa = function() {
        if (aguardandoServidor) {
            return;
            }
        avancarProximoCiclo();
        // if (aguardandoServidor) {
        //     return;
        // }
        // if ($scope.contagemEmAndamento) {
        //     pararCronometroInterno();
        //     $scope.estadoCronometro = 'Pausado';
        // } else {
        //     iniciarContagem();
        // }
    };

    $scope.encerrarCiclo = function() {
        if (aguardandoServidor) {
            return;
        }
        finalizarCicloAbertoNoServidor();
    };

    $scope.cancelarCiclo = function() {
        if (aguardandoServidor) {
            return;
        }
        finalizarCicloAbertoNoServidor();
    };

    carregarTarefasDoServidor();

});