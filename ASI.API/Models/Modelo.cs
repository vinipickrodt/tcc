using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.API.Models
{
    public class Modelo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DataUltimoTreinamento { get; set; }
        public double Acuracia { get; set; }
        public bool Habilitado { get; set; }
    }
}
