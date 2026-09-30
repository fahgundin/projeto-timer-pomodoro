using System;
using System.Collections.Generic;

namespace WebApplication1.Entities;

public partial class Tarefa
{
    public int TarefaId { get; set; }

    public string NomeDaTarefa { get; set; } = null!;

    public int DuracaoFocoSegundos { get; set; }

    public int DuracaoPausaLonga { get; set; }

    public int DuracaoPausaCurta { get; set; }

    public int CiclosParaPausaLonga { get; set; }

    public DateTimeOffset? DataHoraInicio { get; set; }

    public DateTimeOffset? DataHoraFim { get; set; }

    public bool Arquivado { get; set; }

    public virtual ICollection<Ciclo> Ciclos { get; set; } = new List<Ciclo>();
}
