/* ==========================================================
   Migration : 007_AdicionarPersonalizadoTarefas
   Objetivo  : Adiciona a coluna booleana Personalizado em dbo.Tarefas.
               Linhas existentes recebem o valor padrão 0 (falso).
   Autor     : Arthur Fagundes
   Data      : 2026-10-14
   Rollback  : Rollback/007_AdicionarPersonalizadoTarefas.rollback.sql (opcional)

   Checklist depois de executar:
     [ ] Atualizar o arquivo de estado atual (Tables/, Views/, etc.)
     [ ] Rodar o Reverse Engineer no PomodoroDev
     [ ] Commit de tudo junto
   ========================================================== */
USE Pomodoro;
GO

-- Se já foi aplicada, pula todos os lotes até o SET NOEXEC OFF do final
IF EXISTS (SELECT 1 FROM dbo.HistoricoMigracoes WHERE Nome = '007_AdicionarPersonalizadoTarefas')
    BEGIN
        PRINT 'Migration 007_AdicionarPersonalizadoTarefas já aplicada. Nada a fazer.';
        SET NOEXEC ON;
    END
GO

SET XACT_ABORT ON;  -- qualquer erro derruba a transação inteira
BEGIN TRY
    BEGIN TRANSACTION;

    /* ------------------------------------------------------
       ALTERAÇÕES AQUI (mantenha idempotente quando possível)
       ------------------------------------------------------ */

    -- NOT NULL com DEFAULT: o SQL Server preenche as linhas existentes com 0
    IF COL_LENGTH('dbo.Tarefas', 'Personalizado') IS NULL
    ALTER TABLE dbo.Tarefas
        ADD Personalizado BIT NOT NULL
            CONSTRAINT DF_Tarefas_Personalizado DEFAULT 0;

    /* ------------------------------------------------------
       REGISTRO NO HISTÓRICO (sempre a última instrução)
       ------------------------------------------------------ */
    INSERT INTO dbo.HistoricoMigracoes (Nome) VALUES ('007_AdicionarPersonalizadoTarefas');

    COMMIT TRANSACTION;
    PRINT 'Migration 007_AdicionarPersonalizadoTarefas aplicada com sucesso.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

    PRINT 'ERRO na migration 007_AdicionarPersonalizadoTarefas: ' + ERROR_MESSAGE();
    THROW;
END CATCH
GO

SET NOEXEC OFF;
GO