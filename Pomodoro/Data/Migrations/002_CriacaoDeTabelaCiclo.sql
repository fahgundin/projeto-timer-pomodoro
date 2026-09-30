/* ==========================================================
   Migration : 002_CriacaoTabelaDeCiclo
   Objetivo  : 
   Autor     : Arthur Fagundes
   Data      : 30-09-2026

   Checklist depois de executar:
     [ ] Atualizar o arquivo de estado atual (Tables/, Views/, etc.)
     [ ] Rodar o Reverse Engineer no PomodoroDev
     [ ] Commit de tudo junto
   ========================================================== */
USE Pomodoro;
GO

IF EXISTS (SELECT 1 FROM dbo.HistoricoMigracoes WHERE Nome = '002_CriacaoTabelaDeCiclo')
    BEGIN
        PRINT 'Migration 002_CriacaoTabelaDeCiclo já aplicada. Nada a fazer.';
        SET NOEXEC ON;
    END
GO

SET XACT_ABORT ON;  
BEGIN TRY
    BEGIN TRANSACTION;
        
        IF OBJECT_ID('dbo.Ciclos') IS NULL
            CREATE TABLE dbo.Ciclos
            (
                CicloId INT IDENTITY  NOT NULL,
                TarefaId INT NOT NULL,
                TipoDoCiclo INT NOT NULL,
                DataHoraInicio DATETIMEOFFSET,
                DataHoraFim DATETIMEOFFSET,
                Concluido bit NOT NULL CONSTRAINT DF_Ciclos_Concluidos DEFAULT 0,
            
                CONSTRAINT PK_Ciclos PRIMARY KEY (CicloId),
                CONSTRAINT FK_Tarefa FOREIGN KEY (TarefaId) REFERENCES dbo.Tarefas(TarefaId)
            )


    INSERT INTO dbo.HistoricoMigracoes (Nome) VALUES ('002_CriacaoTabelaDeCiclo');

    COMMIT TRANSACTION;
    PRINT 'Migration 002_CriacaoTabelaDeCiclo aplicada com sucesso.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

    PRINT 'ERRO na migration 002_CriacaoTabelaDeCiclo: ' + ERROR_MESSAGE();
    THROW;
END CATCH
GO

SET NOEXEC OFF;
GO