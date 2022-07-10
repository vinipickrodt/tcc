using ASI.Core.Entities;
using ASI.Infra.ML;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ASI.Infra
{
    public class Repository
    {
        private Data.SasiContext ctx = new Data.SasiContext();

        public Repository()
        {
        }

        public int CriarModelo(Modelo modelo)
        {
            var m = new Data.Models.Modelo();
            m.Nome = modelo.Nome;
            m.FrequenciaTreinamentos = modelo.TreinamentoFrequencia ?? "";
            m.TipoModelo = (int)modelo.Tipo;
            m.Status = 0;
            m.StatusTreinamento = 0;

            m.DataUltimoTreinamento = (m.DataUltimoTreinamento == DateTime.MinValue ? new DateTime(1970, 1, 1) : m.DataUltimoTreinamento);

            var pm = new Data.Models.ParametrosModelo();
            pm.Modelo = m;
            pm.CamposEntrada = JsonConvert.SerializeObject(modelo.CamposEntrada);
            pm.CamposSaida = JsonConvert.SerializeObject(modelo.CamposSaida);

            ctx.Add(m);
            ctx.Add(pm);

            ctx.SaveChanges();

            return m.Id;
        }

        public IEnumerable<string> ObtemDadosTreinamento(int parametrosModeloId)
        {
            return ctx.DadosTreinamento
                .Where(dt => dt.ParametrosModeloId == parametrosModeloId)
                .Select(dt => dt.Dados)
                .ToList();
        }

        public ModeloTreinado BuscaModeloTreinado(int modeloId)
        {
            var modelo = BuscaModelo(modeloId);
            var modelTreinado = ctx.ModeloTreinado.Where(mt => mt.ParametrosModeloId == modelo.ParametrosModeloId).FirstOrDefault();

            var result = ConvertUtils.ConvertModeloTreinadoDb(modelTreinado);
            result.Modelo = modelo;

            return result;
        }

        public Modelo ObtemProximoModeloParaTreinar()
        {
            var hora = DateTime.Now.AddMinutes(-30);

            var modeloDb = ctx.Modelo
               .Where(m => m.DataUltimoTreinamento == null || m.DataUltimoTreinamento <= hora)
               .OrderBy(m => m.DataUltimoTreinamento)
               .ThenBy(m => m.Id)
               .Include(m => m.ParametrosModelo)
               .FirstOrDefault();

            if (modeloDb == null)
            {
                return null;
            }

            var modelo = BuscaModelo(modeloDb.Id);

            var existemDadosTreinamento = ctx.DadosTreinamento.Any(dt => dt.ParametrosModeloId == modelo.ParametrosModeloId);

            if (!existemDadosTreinamento)
            {
                return null;
            }

            return ConvertUtils.ConverterModeloDb(modeloDb);
        }

        public Modelo BuscaModelo(int modeloId)
        {
            var modelo = ctx.Modelo
                .Where(m => m.Id == modeloId)
                .Include(m => m.ParametrosModelo)
                .FirstOrDefault();

            if (modelo == null)
                return null;

            return ConvertUtils.ConverterModeloDb(modelo);
        }

        public void InserirDadosDeTreinamentoEmLote(DadosTreinamentoEmLote modeloViewModel)
        {
            var modelo = BuscaModelo(modeloViewModel.ModeloId);

            foreach (var linha in modeloViewModel.Dados)
            {
                var dadosTreinamento = new Data.Models.DadosTreinamento()
                {
                    Dados = JsonConvert.SerializeObject(linha),
                    ParametrosModeloId = modelo.ParametrosModeloId
                };

                ctx.DadosTreinamento.Add(dadosTreinamento);
            }

            ctx.SaveChanges();
        }

        public void GravaTreinamentoModelo(int modeloId, byte[] dados)
        {
            var modelo = BuscaModelo(modeloId);
            var modeloDb = ctx.Modelo.Find(modeloId);
            modeloDb.DataUltimoTreinamento = DateTime.Now;

            ctx.ModeloTreinado.Add(new Data.Models.ModeloTreinado()
            {
                Dados = dados,
                ParametrosModeloId = modelo.ParametrosModeloId,
            });

            ctx.Modelo.Update(modeloDb);

            ctx.SaveChanges();
        }
    }
}