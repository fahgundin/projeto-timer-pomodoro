angular.module('appTimer').controller('ControladorTimer', function($scope, $http, $interval, $document) {

    const deslocamentoMinimoCirculo = 8;
    const intervaloDaContagemMs = 250;
    const chaveDeSessao = 'timerEmAndamento';
    const rotaParaInterromperCiclo = '/Ciclo/FinalizarCicloAtual';
    const tituloOriginal = $document[0].title;

    const tiposDeCicloDoServidor = {
        0: 'foco',
        1: 'pausaLonga',
        2: 'pausaCurta'
    };

    $scope.listaDeTarefas = [];
    $scope.tarefasCarregadas = false;
    $scope.tarefaSelecionada = null;
    $scope.telaAtual = 'lista';
    $scope.mensagemDeErro = '';
    $scope.aguardandoServidor = false;

    $scope.tipoDoCicloAtual = 'foco';
    $scope.contagemEmAndamento = false;
    $scope.segundosRestantes = 0;
    $scope.segundosTotaisDoCiclo = 1;
    $scope.tempoFormatado = '00:00';
    $scope.estadoCronometro = 'Pausado';
    $scope.deslocamentoMinimoCirculo = deslocamentoMinimoCirculo;

    let cronometroInterno = null;
    let fimPrevistoMs = 0;
    let segundosAoPausar = 0;

    function lerMarcaDeSessao() {
        try { return sessionStorage.getItem(chaveDeSessao) === 'sim'; } catch (e) { return false; }
    }

    function gravarMarcaDeSessao() {
        try { sessionStorage.setItem(chaveDeSessao, 'sim'); } catch (e) {}
    }

    function limparMarcaDeSessao() {
        try { sessionStorage.removeItem(chaveDeSessao); } catch (e) {}
    }

    const retomarAoCarregar = lerMarcaDeSessao();

    $scope.$watch('telaAtual', function(novaTela, telaAnterior) {
        if (novaTela === telaAnterior) { return; }
        if (novaTela === 'cronometro') {
            gravarMarcaDeSessao();
        } else {
            limparMarcaDeSessao();
            $document[0].title = tituloOriginal;
        }
    });

    function mostrarErro(texto) {
        $scope.mensagemDeErro = texto;
    }

    function aguardarServidor(requisicao) {
        $scope.aguardandoServidor = true;
        return requisicao.finally(function() {
            $scope.aguardandoServidor = false;
        });
    }

    function converterDataDoServidor(texto) {
        return Date.parse(texto.replace(/(\.\d{3})\d+/, '$1'));
    }

    function formatarSegundosEmContagem(segundos) {
        const minutos = Math.floor(segundos / 60).toString().padStart(2, '0');
        const segundosResto = (segundos % 60).toString().padStart(2, '0');
        return minutos + ':' + segundosResto;
    }

    $scope.formatarDuracaoDaTarefa = function(segundos) {
        if (!segundos) { return '0s'; }
        if (segundos % 60 === 0) { return (segundos / 60) + 'min'; }
        return segundos + 's';
    };

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

    function atualizarTituloDaAba() {
        if ($scope.telaAtual === 'cronometro') {
            $document[0].title = $scope.tempoFormatado + ' - ' + $scope.estadoCronometro;
        }
    }

    function atualizarContagemNaTela() {
        $scope.segundosRestantes = Math.max(0, Math.ceil((fimPrevistoMs - Date.now()) / 1000));
        $scope.tempoFormatado = formatarSegundosEmContagem($scope.segundosRestantes);
        atualizarTituloDaAba();
    }

    function aplicarCiclo(ciclo, inicioMs) {
        const tipo = tiposDeCicloDoServidor[ciclo.tipoDoCiclo];
        if (!tipo) {
            return false;
        }
        const duracao = ciclo.duracaoPlanejadaSegundos || duracaoDoTipo(tipo);
        fimPrevistoMs = inicioMs + duracao * 1000;
        $scope.tipoDoCicloAtual = tipo;
        $scope.segundosTotaisDoCiclo = duracao;
        $scope.estadoCronometro = textoDoTipo(tipo);
        atualizarContagemNaTela();
        return true;
    }

    function pararCronometroInterno() {
        if (cronometroInterno !== null) {
            $interval.cancel(cronometroInterno);
            cronometroInterno = null;
        }
        $scope.contagemEmAndamento = false;
    }

    function finalizarCicloCompleto() {
        pararCronometroInterno();
        $scope.telaAtual = 'lista';
    }

    function contarTempo() {
        atualizarContagemNaTela();
        if ($scope.segundosRestantes <= 0) {
            pararCronometroInterno();
            avancarProximoCiclo();
        }
    }

    function iniciarContagem() {
        if (cronometroInterno !== null) {
            return;
        }
        $scope.contagemEmAndamento = true;
        $scope.estadoCronometro = textoDoTipo($scope.tipoDoCicloAtual);
        cronometroInterno = $interval(contarTempo, intervaloDaContagemMs);
    }

    function avancarProximoCiclo() {
        if ($scope.aguardandoServidor) {
            return;
        }

        if ($scope.tipoDoCicloAtual === 'pausaLonga') {
            aguardarServidor($http.post('/Ciclo/FinalizarCicloAtual')).then(function() {
                finalizarCicloCompleto();
            }, function() {
                finalizarCicloCompleto();
                mostrarErro('Não foi possível registrar o fim do ciclo');
            });
            return;
        }

        aguardarServidor($http.get('/Ciclo/PausarOuFocar')).then(function(resposta) {
            if (!aplicarCiclo(resposta.data, Date.now())) {
                finalizarCicloCompleto();
                mostrarErro('O servidor devolveu uma fase desconhecida');
                return;
            }
            iniciarContagem();
        }, function() {
            finalizarCicloCompleto();
            mostrarErro('Não foi possível registrar a troca de fase');
        });
    }

    function finalizarCicloAbertoNoServidor() {
        pararCronometroInterno();
        aguardarServidor($http.post(rotaParaInterromperCiclo)).then(function() {
            $scope.telaAtual = 'lista';
        }, function() {
            $scope.telaAtual = 'lista';
            mostrarErro('Não foi possível encerrar o ciclo no servidor');
        });
    }

    function encerrarCiclosEsquecidosNoServidor() {
        return $http.get('/Ciclo/ObterCicloAtual').then(function(resposta) {
            if (!resposta.data) {
                return;
            }
            return $http.post(rotaParaInterromperCiclo).then(encerrarCiclosEsquecidosNoServidor);
        });
    }

    function carregarTarefasDoServidor() {
        return $http.get('/Tarefa/ListarTarefas').then(function(resposta) {
            $scope.listaDeTarefas = resposta.data.filter(function(tarefa) {
                return !tarefa.arquivado;
            });
            $scope.tarefasCarregadas = true;
        }, function() {
            mostrarErro('Não foi possível carregar as tarefas');
        });
    }

    function retomarCicloEmAndamento() {
        return $http.get('/Ciclo/ObterCicloAtual').then(function(resposta) {
            const ciclo = resposta.data;
            if (!ciclo) {
                limparMarcaDeSessao();
                return;
            }

            const tarefaDoCiclo = $scope.listaDeTarefas.find(function(tarefa) {
                return tarefa.tarefaId === ciclo.tarefaId;
            }) || ciclo.tarefa;

            if (!tarefaDoCiclo) {
                return;
            }

            $scope.tarefaSelecionada = tarefaDoCiclo;

            if (!aplicarCiclo(ciclo, converterDataDoServidor(ciclo.dataHoraInicio))) {
                return;
            }

            $scope.telaAtual = 'cronometro';

            if ($scope.segundosRestantes <= 0) {
                avancarProximoCiclo();
            } else {
                iniciarContagem();
            }
        }, function() {
            mostrarErro('Não foi possível verificar o ciclo em andamento');
        });
    }

    function aoVoltarParaAba() {
        if (!$document[0].hidden && $scope.contagemEmAndamento) {
            $scope.$evalAsync(contarTempo);
        }
    }

    $document.on('visibilitychange', aoVoltarParaAba);

    $scope.$on('$destroy', function() {
        $document.off('visibilitychange', aoVoltarParaAba);
        pararCronometroInterno();
    });

    $scope.selecionarTarefa = function(tarefa) {
        if ($scope.tarefaSelecionada === tarefa) {
            $scope.tarefaSelecionada = null;
            return;
        }
        $scope.tarefaSelecionada = tarefa;
    };

    $scope.iniciarCiclo = function() {
        if (!$scope.tarefaSelecionada || $scope.aguardandoServidor) {
            return;
        }

        $scope.mensagemDeErro = '';
        const tarefaId = $scope.tarefaSelecionada.tarefaId;

        aguardarServidor(
            encerrarCiclosEsquecidosNoServidor().then(function() {
                return $http.post('/Ciclo/IniciarCiclo', null, { params: { tarefaId: tarefaId } });
            })
        ).then(function(resposta) {
            if (!aplicarCiclo(resposta.data, Date.now())) {
                mostrarErro('O servidor devolveu uma fase desconhecida');
                return;
            }
            $scope.telaAtual = 'cronometro';
            iniciarContagem();
        }, function() {
            mostrarErro('Não foi possível iniciar o ciclo');
        });
    };

    $scope.alternarPausa = function() {
        if ($scope.aguardandoServidor) {
            return;
        }
        if ($scope.contagemEmAndamento) {
            pararCronometroInterno();
            segundosAoPausar = $scope.segundosRestantes;
            $scope.estadoCronometro = 'Pausado';
        } else {
            fimPrevistoMs = Date.now() + segundosAoPausar * 1000;
            iniciarContagem();
        }
        atualizarTituloDaAba();
    };

    $scope.encerrarCiclo = function() {
        if ($scope.aguardandoServidor) {
            return;
        }
        finalizarCicloAbertoNoServidor();
    };

    $scope.cancelarCiclo = function() {
        if ($scope.aguardandoServidor) {
            return;
        }
        finalizarCicloAbertoNoServidor();
    };

    carregarTarefasDoServidor().then(function() {
        if (retomarAoCarregar) {
            return retomarCicloEmAndamento();
        }
    });

});