using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ASI.Infra.Data.Models
{
    public partial class Modelo
    {
        public Modelo()
        {
            ParametrosModelo = new HashSet<ParametrosModelo>();
            Treinamento = new HashSet<Treinamento>();
        }

        public int Id { get; set; }
        public string Nome { get; set; }
        public int TipoModelo { get; set; }
        public string FrequenciaTreinamentos { get; set; }
        public DateTime? DataUltimoTreinamento { get; set; }
        public int Status { get; set; }
        public int StatusTreinamento { get; set; }

        public virtual ICollection<ParametrosModelo> ParametrosModelo { get; set; }
        public virtual ICollection<Treinamento> Treinamento { get; set; }
    }
}
