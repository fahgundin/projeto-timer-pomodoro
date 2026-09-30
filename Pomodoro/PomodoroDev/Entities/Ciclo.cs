using System;
using System.Collections.Generic;

namespace WebApplication1.Entities;

public partial class Ciclo
{
    public int CicloId { get; set; }

    public int TarefaId { get; set; }

    public int TipoDoCiclo { get; set; }

    public DateTimeOffset? DataHoraInicio { get; set; }

    public DateTimeOffset? DataHoraFim { get; set; }

    public bool Concluido { get; set; }

    public virtual Tarefa Tarefa { get; set; } = null!;
}
