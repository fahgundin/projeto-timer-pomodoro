using System;
using System.Collections.Generic;

namespace WebApplication1.Entities;

public partial class HistoricoMigraco
{
    public string Nome { get; set; } = null!;

    public DateTime AplicadoEm { get; set; }

    public string AplicadoPor { get; set; } = null!;
}
