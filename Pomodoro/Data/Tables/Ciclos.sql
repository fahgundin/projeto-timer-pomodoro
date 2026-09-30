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