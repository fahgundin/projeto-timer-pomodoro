/* ==========================================================
   Migration : 006_CriacaoDaTabelaDeConfiguracoes
   Objetivo  : Criar tabela de configuracoes globais do timer
   Autor     : Robert Gean
   Data      : 2026-10-09
   ========================================================== */
USE Pomodoro;
GO

IF EXISTS (SELECT 1 FROM dbo.HistoricoMigracoes WHERE Nome = '006_CriacaoDaTabelaDeConfiguracoes')
    BEGIN
        PRINT 'Migration 006_CriacaoDaTabelaDeConfiguracoes já aplicada. Nada a fazer.';
        SET NOEXEC ON;
    END
GO

SET XACT_ABORT ON;
BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID('dbo.Configuracoes') IS NULL
        BEGIN
            CREATE TABLE dbo.Configuracoes
            (
                ConfiguracaoId           INT NOT NULL CONSTRAINT PK_Configuracoes PRIMARY KEY,
                DuracaoFocoSegundos      INT NOT NULL,
                DuracaoPausaCurta        INT NOT NULL,
                DuracaoPausaLonga        INT NOT NULL,
                CiclosParaPausaLonga     INT NOT NULL,
                PausaLongaAtivada        BIT NOT NULL CONSTRAINT DF_Configuracoes_PausaLonga DEFAULT 1
            );

            INSERT INTO dbo.Configuracoes
            (ConfiguracaoId, DuracaoFocoSegundos, DuracaoPausaCurta, DuracaoPausaLonga, CiclosParaPausaLonga, PausaLongaAtivada)
            VALUES
                (1, 3000, 600, 1200, 4, 1);
        END

    INSERT INTO dbo.HistoricoMigracoes (Nome) VALUES ('006_CriacaoDaTabelaDeConfiguracoes');

    COMMIT TRANSACTION;
    PRINT 'Migration 006_CriacaoDaTabelaDeConfiguracoes aplicada com sucesso.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    PRINT 'ERRO na migration 006_CriacaoDaTabelaDeConfiguracoes: ' + ERROR_MESSAGE();
    THROW;
END CATCH
GO

SET NOEXEC OFF;
GO