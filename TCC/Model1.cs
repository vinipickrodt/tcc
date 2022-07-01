using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace TCC
{
    public class DadosContext : DbContext
    {
        public DadosContext()
            : base("name=Dados")
        {
        }

        public virtual DbSet<Modelo> Modelos { get; set; }
        public virtual DbSet<Treinamento> Treinamentos { get; set; }
    }

    public class Modelo
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int DataUltimoTreinamento
        {
            get => default;
            set
            {
            }
        }
    }

    public class Treinamento
    {
        public int Id { get; set; }
        public int ModeloId { get; set; }

        [ForeignKey(nameof(ModeloId))]
        public Modelo Modelo { get; set; }
    }

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
}