angular.module('appRelatorio').controller('ControladorRelatorio', function($scope, $http) {

    const hoje = new Date();
    const formatarData = function(data) {
        return data.toISOString().split('T')[0];
    };

    $scope.filtro = {
        dataInicio: formatarData(hoje),
        dataFim: formatarData(hoje)
    };

    $scope.resumo = null;
    $scope.detalhe = [];
    $scope.mensagemDeErro = '';
    $scope.buscou = false;

    $scope.formatarSegundos = function(segundos) {
        if (!segundos) { return '0min'; }
        const horas = Math.floor(segundos / 3600);
        const minutos = Math.floor((segundos % 3600) / 60);
        if (horas > 0) { return horas + 'h ' + minutos + 'min'; }
        return minutos + 'min';
    };

    $scope.buscar = function() {
        $scope.mensagemDeErro = '';
        $scope.buscou = false;

        const inicio = new Date($scope.filtro.dataInicio + 'T00:00:00');
        const fim = new Date($scope.filtro.dataFim + 'T23:59:59');

        if (inicio > fim) {
            $scope.mensagemDeErro = 'A data de início deve ser anterior à data de fim';
            return;
        }

        const inicioUtc = inicio.toISOString();
        const fimUtc = fim.toISOString();

        $http.get('/Relatorio/ObterResumo', { params: { inicio: inicioUtc, fim: fimUtc } })
            .then(function(resposta) {
                $scope.resumo = resposta.data;
                $scope.buscou = true;
            }, function() {
                $scope.mensagemDeErro = 'Não foi possível carregar o resumo';
            });

        $http.get('/Relatorio/ObterDetalhePorTarefa', { params: { inicio: inicioUtc, fim: fimUtc } })
            .then(function(resposta) {
                $scope.detalhe = resposta.data;
            }, function() {
                $scope.mensagemDeErro = 'Não foi possível carregar o detalhe por tarefa';
            });
    };

    $scope.buscar();
});