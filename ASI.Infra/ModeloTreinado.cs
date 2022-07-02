

using ASI.Core.Entities;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ASI.Infra.ML
{
    public class ModeloTreinado
    {
        public int Id { get; set; }
        public byte[] Dados { get; set; }
        public int ParametrosModeloId { get; set; }

        public Modelo Modelo { get; set; }

        public Output Predict(Dictionary<string, string> dados)
        {
            Utils.ValidaDados(new List<Dictionary<string, string>>() { dados }, Modelo, true);

            MLContext mlContext = new MLContext();
            DataViewSchema dvs;
            ITransformer transformer = mlContext.Model.Load(new MemoryStream(Dados), out dvs);

            var sd = Utils.ConstroiDefinicaoSchema(Modelo, true);

            var eng = mlContext.Model.CreatePredictionEngine<TrainingDataRow, Output>(transformer, true, sd);
            var dadosPredict = Utils.ConverteParaDadosDeTreino(Modelo, new List<Dictionary<string, string>>() { dados }, true).First();

            var output = eng.Predict(dadosPredict);

            return output;
        }
    }

    public class Output
    {
        // ColumnName attribute is used to change the column name from
        // its default value, which is the name of the field.
        [ColumnName("PredictedLabel")]
        public string Prediction { get; set; }

        public float[] Score { get; set; }
    }
}