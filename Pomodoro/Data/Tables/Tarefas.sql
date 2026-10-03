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
    Cor VARCHAR(7) NOT NULL CONSTRAINT DF_Tarefas_Cor DEFAULT ('#4b5563'),

    CONSTRAINT PK_Tarefas PRIMARY KEY (TarefaId)
)