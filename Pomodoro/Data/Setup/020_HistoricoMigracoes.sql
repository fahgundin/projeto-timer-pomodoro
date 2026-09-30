IF OBJECT_ID('dbo.HistoricoMigracoes') IS NULL
CREATE TABLE [dbo].[HistoricoMigracoes]
(
  
    Nome                  NVARCHAR(200) NOT NULL,
    AplicadoEm            DATETIME2     NOT NULL CONSTRAINT DF_HistoricoMigracoes_AplicadoEm DEFAULT SYSUTCDATETIME(),
    AplicadoPor           NVARCHAR(128) NOT NULL CONSTRAINT DF_HistoricoMigracoes_Por DEFAULT SUSER_SNAME(),
    CONSTRAINT PK_HistoricoMigracao PRIMARY KEY (Nome)
);