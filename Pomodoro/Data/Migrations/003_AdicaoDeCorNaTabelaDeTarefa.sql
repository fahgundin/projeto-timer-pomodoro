/* ==========================================================
   Migration : 003_AdicaoDeCorNaTabelaDeTarefa
   Objetivo  : Guardar no banco a cor de destaque de cada tarefa
               (padrao: cinza escuro)
   Autor     : Robert Gean
   Data      : 2026-10-03
   Rollback  : (opcional)

   Checklist depois de executar:
     [ ] Atualizar o arquivo de estado atual (Tables/Tarefas.sql)
     [ ] Rodar o Reverse Engineer no PomodoroDev
     [ ] Commit de tudo junto
   ========================================================== */
USE Pomodoro;
GO

IF EXISTS (SELECT 1 FROM dbo.HistoricoMigracoes WHERE Nome = '003_AdicaoDeCorNaTabelaDeTarefa')
    BEGIN
        PRINT 'Migration 003_AdicaoDeCorNaTabelaDeTarefa já aplicada. Nada a fazer.';
        SET NOEXEC ON;
    END
GO

SET XACT_ABORT ON;
BEGIN TRY
    BEGIN TRANSACTION;

    IF COL_LENGTH('dbo.Tarefas', 'Cor') IS NULL
    ALTER TABLE dbo.Tarefas
        ADD Cor VARCHAR(7) NOT NULL
            CONSTRAINT DF_Tarefas_Cor DEFAULT ('#4b5563');

    INSERT INTO dbo.HistoricoMigracoes (Nome) VALUES ('003_AdicaoDeCorNaTabelaDeTarefa');

    COMMIT TRANSACTION;
    PRINT 'Migration 003_AdicaoDeCorNaTabelaDeTarefa aplicada com sucesso.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

    PRINT 'ERRO na migration 003_AdicaoDeCorNaTabelaDeTarefa: ' + ERROR_MESSAGE();
    THROW;
END CATCH
GO

SET NOEXEC OFF;
GO