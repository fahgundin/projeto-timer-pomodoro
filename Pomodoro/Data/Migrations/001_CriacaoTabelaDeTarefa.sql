
/* ==========================================================
   Migration : 001_CriacaoTabelaDeTarefa
   Objetivo  : Cria tabela de Tarefas
   Autor     : Arthur Fagundes
   Data      : 29-09-2026
   Rollback  : 

   Checklist depois de executar:
     [ ] Atualizar o arquivo de estado atual (Tables/, Views/, etc.)
     [ ] Rodar o Reverse Engineer no PomodoroDev
     [ ] Commit de tudo junto
   ========================================================== */
USE Pomodoro;
GO

-- Se já foi aplicada, pula todos os lotes até o SET NOEXEC OFF do final
IF EXISTS (SELECT 1 FROM dbo.HistoricoMigracoes WHERE Nome = '001_CriacaoTabelaDeTarefa')
    BEGIN
        PRINT 'Migration 001_CriacaoTabelaDeTarefa já aplicada. Nada a fazer.';
        SET NOEXEC ON;
    END
GO

SET XACT_ABORT ON;  -- qualquer erro derruba a transação inteira
BEGIN TRY
    BEGIN TRANSACTION;
    
        IF OBJECT_ID('dbo.Tarefas') IS NULL
            CREATE TABLE dbo.Tarefas
            (
                TarefaId INT IDENTITY(1,1) NOT NULL ,
                NomeDaTarefa VARCHAR(100) NOT NULL,
                DuracaoFocoSegundos INT NOT NULL,
                DuracaoPausaLonga INT NOT NULL,
                DuracaoPausaCurta INT NOT NULL,
                CiclosParaPausaLonga INT NOT NULL,
                DataHoraInicio DATETIMEOFFSET,
                DataHoraFim DATETIMEOFFSET,
                Arquivado bit NOT NULL CONSTRAINT DF_Tarefas_Arquivado DEFAULT 0,

                CONSTRAINT PK_Tarefas PRIMARY KEY (TarefaId)
            )

    INSERT INTO dbo.HistoricoMigracoes (Nome) VALUES ('001_CriacaoTabelaDeTarefa');

    COMMIT TRANSACTION;
    PRINT 'Migration 001_CriacaoTabelaDeTarefa aplicada com sucesso.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

    PRINT 'ERRO na migration 001_CriacaoTabelaDeTarefa: ' + ERROR_MESSAGE();
    THROW;
END CATCH
GO

SET NOEXEC OFF;
GO