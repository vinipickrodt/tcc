using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ASI.Infra.Data.Models
{
    public partial class Treinamento
    {
        public int Id { get; set; }
        public int ModeloId { get; set; }
        public double Acuracia { get; set; }
        public int Duracao { get; set; }
        public DateTime Data { get; set; }

        public virtual Modelo Modelo { get; set; }
    }
}
