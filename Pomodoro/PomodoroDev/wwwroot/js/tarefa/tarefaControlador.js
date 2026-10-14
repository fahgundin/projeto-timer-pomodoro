angular.module('appTarefa').controller('ControladorTarefa', function($scope, $http) {

    const segundosPorMinuto = 60;

    $scope.coresDisponiveis = ['#8bc34a', '#d946ef', '#60a5fa', '#ef4444', '#f59e0b', '#10b981'];

    const formularioInicial = {
        nomeDaTarefa: '',
        personalizado: false,
        duracaoFocoSegundos: 25,
        duracaoPausaCurtaSegundos: 5,
        duracaoPausaLongaSegundos: 15,
        ciclosParaPausaLonga: 4,
        cor: $scope.coresDisponiveis[0]
    };

    $scope.listaDeTarefas = [];
    $scope.abaAtual = 'ativas';
    $scope.telaAtual = 'lista';
    $scope.tarefaEmEdicao = null;
    $scope.formulario = angular.copy(formularioInicial);
    $scope.salvando = false;
    $scope.mensagemDeErro = '';
    $scope.menuAbertoId = null;

    function converterSegundosEmMinutos(segundos) {
        return Math.max(1, Math.round(segundos / segundosPorMinuto));
    }

    function carregarTarefas() {
        $http.get('/Tarefa/ListarTarefas').then(function(resposta) {
            $scope.listaDeTarefas = resposta.data;
        }, function() {
            $scope.mensagemDeErro = 'Não foi possível carregar as tarefas';
        });
    }

    function preencherFormulario(tarefa) {
        $scope.formulario = {
            nomeDaTarefa: tarefa.nomeDaTarefa,
            personalizado: tarefa.personalizado === true,
            duracaoFocoSegundos: tarefa.duracaoFocoSegundos,
            duracaoPausaCurtaSegundos: tarefa.duracaoPausaCurta,
            duracaoPausaLongaSegundos: tarefa.duracaoPausaLonga,
            ciclosParaPausaLonga: tarefa.ciclosParaPausaLonga,
            cor: tarefa.cor
        };
    }

    function obterTarefa(id) {
        $http.post('/Tarefa/ObterTarefa', null, { params: { tarefaId: id } }).then(function(resposta) {
            $scope.tarefaEmEdicao = resposta.data;
            preencherFormulario(resposta.data);
        }, function() {
            $scope.mensagemDeErro = 'Não foi possível carregar tarefa';
        });
    }

    function montarDadosDoFormulario() {
        return {
            nomeDaTarefa: $scope.formulario.nomeDaTarefa.trim(),
            personalizado: $scope.formulario.personalizado,
            duracaoFocoSegundos: $scope.formulario.duracaoFocoSegundos,
            duracaoPausaCurta: $scope.formulario.duracaoPausaCurtaSegundos,
            duracaoPausaLonga: $scope.formulario.duracaoPausaLongaSegundos,
            ciclosParaPausaLonga: $scope.formulario.ciclosParaPausaLonga,
            cor: $scope.formulario.cor
        };
    }

    $scope.correspondeAAba = function(tarefa) {
        return $scope.abaAtual === 'ativas' ? !tarefa.arquivado : tarefa.arquivado;
    };

    $scope.trocarAba = function(aba) {
        $scope.menuAbertoId = null;
        $scope.mensagemDeErro = '';
        $scope.abaAtual = aba;
    };

    $scope.possuiDataDeCriacao = function(tarefa) {
        return !!tarefa.dataHoraInicio && tarefa.dataHoraInicio.indexOf('0001') !== 0;
    };

    $scope.alternarMenu = function(tarefa) {
        $scope.menuAbertoId = $scope.menuAbertoId === tarefa.tarefaId ? null : tarefa.tarefaId;
    };

    $scope.fecharMenu = function() {
        $scope.menuAbertoId = null;
    };

    $scope.novaTarefa = function() {
        $scope.menuAbertoId = null;
        $scope.mensagemDeErro = '';
        $scope.tarefaEmEdicao = null;
        $scope.formulario = angular.copy(formularioInicial);
        $scope.telaAtual = 'formulario';
    };

    $scope.iniciarEdicao = function(tarefa) {
        $scope.menuAbertoId = null;
        $scope.mensagemDeErro = '';
        preencherFormulario(tarefa);   // mostra já o que veio na lista
        obterTarefa(tarefa.tarefaId);  // e atualiza com o estado salvo no servidor
        $scope.telaAtual = 'formulario';
    };

    $scope.voltarParaLista = function() {
        $scope.mensagemDeErro = '';
        $scope.tarefaEmEdicao = null;
        $scope.telaAtual = 'lista';
    };

    $scope.salvarTarefa = function(formularioDaTarefa) {
        if (formularioDaTarefa.$invalid || $scope.salvando) {
            return;
        }

        $scope.salvando = true;
        $scope.mensagemDeErro = '';

        const dadosDoFormulario = montarDadosDoFormulario();
        const requisicao = $scope.tarefaEmEdicao
            ? $http.patch('/Tarefa/EditarTarefa', angular.extend({}, $scope.tarefaEmEdicao, dadosDoFormulario))
            : $http.post('/Tarefa/CriarTarefa', angular.extend(dadosDoFormulario, {
                dataHoraInicio: new Date().toISOString(),
                arquivado: false
            }));

        requisicao.then(function() {
            $scope.tarefaEmEdicao = null;
            $scope.telaAtual = 'lista';
            carregarTarefas();
        }, function() {
            $scope.mensagemDeErro = 'Não foi possível salvar a tarefa';
        }).finally(function() {
            $scope.salvando = false;
        });
    };

    $scope.arquivarTarefa = function(tarefa) {
        $scope.menuAbertoId = null;
        $scope.mensagemDeErro = '';
        $http.post('/Tarefa/ArquivarTarefa', JSON.stringify(tarefa.tarefaId)).then(function() {
            carregarTarefas();
        }, function() {
            $scope.mensagemDeErro = 'Não foi possível arquivar a tarefa';
        });
    };

    $scope.desarquivarTarefa = function(tarefa) {
        $scope.menuAbertoId = null;
        $scope.mensagemDeErro = '';
        $http.patch('/Tarefa/EditarTarefa', angular.extend({}, tarefa, { arquivado: false })).then(function() {
            carregarTarefas();
        }, function() {
            $scope.mensagemDeErro = 'Não foi possível desarquivar a tarefa';
        });
    };

    $scope.excluirTarefa = function(tarefa) {
        $scope.menuAbertoId = null;

        if (!confirm('Excluir a tarefa "' + tarefa.nomeDaTarefa + '" e todo o histórico dela? Essa ação não pode ser desfeita.')) {
            return;
        }

        $scope.mensagemDeErro = '';
        $http.post('/Tarefa/ExcluirTarefa', JSON.stringify(tarefa.tarefaId)).then(function() {
            carregarTarefas();
        }, function(resposta) {
            $scope.mensagemDeErro = (resposta.data && resposta.data.mensagem) || 'Não foi possível excluir a tarefa';
        });
    };

    carregarTarefas();

});