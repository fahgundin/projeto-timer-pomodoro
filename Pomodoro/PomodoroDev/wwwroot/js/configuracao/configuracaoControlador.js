angular.module('appConfiguracao').controller('ControladorConfiguracao', function($scope, $http, $timeout) {

    $scope.formulario = {
        duracaoFoco: 25,
        duracaoPausaCurta: 5,
        duracaoPausaLonga: 15,
        ciclosParaPausaLonga: 4
    };

    $scope.carregando = true;
    $scope.salvando = false;
    $scope.mensagemDeErro = '';
    $scope.mensagemDeSucesso = '';

    let configuracaoOriginal = null;
    let temporizadorMensagem = null;

    function aplicarConfiguracao(dados) {
        configuracaoOriginal = {
            duracaoFoco: dados.duracaoFoco,
            duracaoPausaCurta: dados.duracaoPausaCurta,
            duracaoPausaLonga: dados.duracaoPausaLonga,
            ciclosParaPausaLonga: dados.ciclosParaPausaLonga
        };
        $scope.formulario = angular.copy(configuracaoOriginal);
    }

    function exibirSucesso(mensagem) {
        $scope.mensagemDeSucesso = mensagem;
        $timeout.cancel(temporizadorMensagem);
        temporizadorMensagem = $timeout(function() {
            $scope.mensagemDeSucesso = '';
        }, 3000);
    }

    function carregarConfiguracao() {
        $scope.carregando = true;
        $scope.mensagemDeErro = '';

        $http.get('/Configuracao/ObterConfiguracao').then(function(resposta) {
            aplicarConfiguracao(resposta.data);
        }, function() {
            $scope.mensagemDeErro = 'Não foi possível carregar as configurações';
        }).finally(function() {
            $scope.carregando = false;
        });
    }

    $scope.houveAlteracao = function() {
        return !!configuracaoOriginal && !angular.equals($scope.formulario, configuracaoOriginal);
    };

    $scope.salvarConfiguracao = function(formularioDaConfiguracao) {
        if (formularioDaConfiguracao.$invalid || $scope.salvando) {
            return;
        }

        $scope.salvando = true;
        $scope.mensagemDeErro = '';
        $scope.mensagemDeSucesso = '';

        const dados = angular.copy($scope.formulario);

        $http.post('/Configuracao/SalvarConfiguracao', dados).then(function() {
            configuracaoOriginal = angular.copy(dados);
            exibirSucesso('Configurações salvas');
        }, function(resposta) {
            $scope.mensagemDeErro = (resposta.data && resposta.data.mensagem) || 'Não foi possível salvar as configurações';
        }).finally(function() {
            $scope.salvando = false;
        });
    };

    $scope.desfazerAlteracoes = function() {
        $scope.mensagemDeErro = '';
        $scope.mensagemDeSucesso = '';

        if (configuracaoOriginal) {
            $scope.formulario = angular.copy(configuracaoOriginal);
        }
    };

    carregarConfiguracao();

});