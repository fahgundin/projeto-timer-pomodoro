/* ==========================================================
   Migration : 004_AdicaoDeDuracaoPlanejadaNoCiclo
   Objetivo  : Guardar em cada ciclo a duracao planejada no momento
               em que ele foi iniciado, para que editar a tarefa
               nao altere o historico
   Autor     : Robert Gean
   Data      : 2026-10-03
   Rollback  : (opcional)

   Checklist depois de executar:
     [ ] Atualizar o arquivo de estado atual (Tables/Ciclos.sql)
     [ ] Rodar o Reverse Engineer no PomodoroDev
     [ ] Commit de tudo junto
   ========================================================== */
USE Pomodoro;
GO

IF EXISTS (SELECT 1 FROM dbo.HistoricoMigracoes WHERE Nome = '004_AdicaoDeDuracaoPlanejadaNoCiclo')
    BEGIN
        PRINT 'Migration 004_AdicaoDeDuracaoPlanejadaNoCiclo já aplicada. Nada a fazer.';
        SET NOEXEC ON;
    END
GO

SET XACT_ABORT ON;
BEGIN TRY
    BEGIN TRANSACTION;

    IF COL_LENGTH('dbo.Ciclos', 'DuracaoPlanejadaSegundos') IS NULL
    ALTER TABLE dbo.Ciclos
        ADD DuracaoPlanejadaSegundos INT NOT NULL
            CONSTRAINT DF_Ciclos_DuracaoPlanejadaSegundos DEFAULT (0);

    EXEC('UPDATE c SET c.DuracaoPlanejadaSegundos = CASE c.TipoDoCiclo
            WHEN 0 THEN t.DuracaoFocoSegundos
            WHEN 1 THEN t.DuracaoPausaLonga
            ELSE t.DuracaoPausaCurta END
          FROM dbo.Ciclos c
          JOIN dbo.Tarefas t ON t.TarefaId = c.TarefaId
          WHERE c.DuracaoPlanejadaSegundos = 0');

    INSERT INTO dbo.HistoricoMigracoes (Nome) VALUES ('004_AdicaoDeDuracaoPlanejadaNoCiclo');

    COMMIT TRANSACTION;
    PRINT 'Migration 004_AdicaoDeDuracaoPlanejadaNoCiclo aplicada com sucesso.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

    PRINT 'ERRO na migration 004_AdicaoDeDuracaoPlanejadaNoCiclo: ' + ERROR_MESSAGE();
    THROW;
END CATCH
GO

SET NOEXEC OFF;
GO