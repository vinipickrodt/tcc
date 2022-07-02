using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Core.Entities
{
    public class Treinamento
    {
        public int Id { get; set; }
        public DateTime Data{ get; set; }
        public double Acuracia { get; set; }
        public int Duracao { get; set; }
        public int ModeloId { get; set; }
        public Modelo Modelo { get; set; }
    }
}
