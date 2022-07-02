using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ASI.Infra.Data.Models
{
    public partial class DadosTreinamento
    {
        public int Id { get; set; }
        public string Dados { get; set; }
        public int ParametrosModeloId { get; set; }

        public virtual ParametrosModelo ParametrosModelo { get; set; }
    }
}
