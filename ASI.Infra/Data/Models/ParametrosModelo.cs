using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ASI.Infra.Data.Models
{
    public partial class ParametrosModelo
    {
        public ParametrosModelo()
        {
            DadosTreinamento = new HashSet<DadosTreinamento>();
            ModeloTreinado = new HashSet<ModeloTreinado>();
        }

        public int Id { get; set; }
        public string CamposEntrada { get; set; }
        public string CamposSaida { get; set; }
        public int ModeloId { get; set; }
        public byte[] Data { get; set; }

        public virtual Modelo Modelo { get; set; }
        public virtual ICollection<DadosTreinamento> DadosTreinamento { get; set; }
        public virtual ICollection<ModeloTreinado> ModeloTreinado { get; set; }
    }
}
