angular.module('appTimer').controller('ControladorTimer', function($scope) {

    $scope.listaDeTarefas = [
        { tarefaId: 1, nomeDaTarefa: 'Estudar', duracaoFocoSegundos: 480, arquivado: false },
        { tarefaId: 2, nomeDaTarefa: 'Estudar C#', duracaoFocoSegundos: 300, arquivado: false },
        { tarefaId: 3, nomeDaTarefa: 'Ler', duracaoFocoSegundos: 600, arquivado: false }
    ];

    $scope.tarefaSelecionada = null;
    $scope.telaAtual = 'lista';
    $scope.tempoFormatado = '00:00';
    $scope.estadoCronometro = 'Pausado';
    $scope.contagemEmAndamento = false;

    $scope.selecionarTarefa = function(tarefa) {
        $scope.tarefaSelecionada = tarefa;
    };

    $scope.iniciarCiclo = function() {
        if (!$scope.tarefaSelecionada) {
            alert('Selecione uma tarefa antes de iniciar');
            return;
        }
        $scope.telaAtual = 'cronometro';
        console.log('Iniciando ciclo para a tarefa:', $scope.tarefaSelecionada.nomeDaTarefa);
    };

    $scope.voltarParaLista = function() {
        $scope.telaAtual = 'lista';
    };

    $scope.alternarPausa = function() {
        $scope.contagemEmAndamento = !$scope.contagemEmAndamento;
        $scope.estadoCronometro = $scope.contagemEmAndamento ? 'Em foco' : 'Pausado';
    };

    $scope.encerrarCiclo = function() {
        $scope.telaAtual = 'lista';
        $scope.contagemEmAndamento = false;
    };

    $scope.cancelarCiclo = function() {
        $scope.telaAtual = 'lista';
        $scope.contagemEmAndamento = false;
    };
    
    $scope.formatarSegundosEmMinutos = function(segundos) {
        var minutos = Math.floor(segundos / 60);
        return minutos + 'min';
    };
    
});