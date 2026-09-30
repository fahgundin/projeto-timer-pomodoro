/* ==========================================================
   Migration : NNN_DescricaoCurta
   Objetivo  : (o que esta migration faz e por quê)
   Autor     : Seu Nome
   Data      : AAAA-MM-DD
   Rollback  : Rollback/NNN_DescricaoCurta.rollback.sql (opcional)

   Checklist depois de executar:
     [ ] Atualizar o arquivo de estado atual (Tables/, Views/, etc.)
     [ ] Rodar o Reverse Engineer no PomodoroDev
     [ ] Commit de tudo junto
   ========================================================== */
USE Pomodoro;
GO

-- Se já foi aplicada, pula todos os lotes até o SET NOEXEC OFF do final
IF EXISTS (SELECT 1 FROM dbo.HistoricoMigracoes WHERE Nome = 'NNN_DescricaoCurta')
    BEGIN
        PRINT 'Migration NNN_DescricaoCurta já aplicada. Nada a fazer.';
        SET NOEXEC ON;
    END
GO

SET XACT_ABORT ON;  -- qualquer erro derruba a transação inteira
BEGIN TRY
    BEGIN TRANSACTION;

    /* ------------------------------------------------------
       ALTERAÇÕES AQUI (mantenha idempotente quando possível)
       ------------------------------------------------------ */

    -- Exemplo: adicionar coluna
    -- IF COL_LENGTH('dbo.Tarefas', 'NovaColuna') IS NULL
    --     ALTER TABLE dbo.Tarefas
    --     ADD NovaColuna NVARCHAR(50) NULL;

    -- Exemplo: criar tabela
    -- IF OBJECT_ID('dbo.NovaTabela') IS NULL
    --     CREATE TABLE dbo.NovaTabela
    --     (
    --         Id   INT IDENTITY(1,1) NOT NULL,
    --         Nome NVARCHAR(100)     NOT NULL,
    --         CONSTRAINT PK_NovaTabela PRIMARY KEY (Id)
    --     );

    -- Exemplo: usar uma coluna recém-criada no mesmo lote exige EXEC
    -- EXEC('UPDATE dbo.Tarefas SET NovaColuna = ''valor'' WHERE NovaColuna IS NULL');

    /* ------------------------------------------------------
       REGISTRO NO HISTÓRICO (sempre a última instrução)
       ------------------------------------------------------ */
    INSERT INTO dbo.HistoricoMigracoes (Nome) VALUES ('NNN_DescricaoCurta');

    COMMIT TRANSACTION;
    PRINT 'Migration NNN_DescricaoCurta aplicada com sucesso.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

    PRINT 'ERRO na migration NNN_DescricaoCurta: ' + ERROR_MESSAGE();
    THROW;
END CATCH
GO

SET NOEXEC OFF;
GO