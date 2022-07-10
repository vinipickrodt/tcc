using System;
using System.Net;
using System.Threading.Tasks;

namespace ASI.Trainer
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            while (true)
            {
                Clientes.SasiClient cliente = new Clientes.SasiClient("https://localhost:44381/", new System.Net.Http.HttpClient());

                try
                {
                    var modelo = await cliente.ObterProximoModeloParaTreinarAsync();

                    if (modelo != null)
                    {
                        Console.WriteLine($"TREINAMENTO INICIADO (Modelo = {modelo.Id.ToString().PadLeft(4)}, Data = {DateTime.Now.ToString("dd/MM/yy HH:mm")})");

                        var dadosTreinamento = await cliente.ObterDadosTreinamentoAsync(modelo.ParametrosModeloId);
                        var resultadoTreinamento = new Treinador(modelo).Treinar(dadosTreinamento);

                        var resultado = await cliente.AtualizarModeloTreinamentoAsync(new Clientes.TreinoModeloArquivo()
                        {
                            ModeloId = modelo.Id,
                            ModeloTreinadoBase64 = Convert.ToBase64String(resultadoTreinamento)
                        });

                        Console.WriteLine($"TREINAMENTO FINALIZADO (Modelo = {modelo.Id.ToString().PadLeft(4)}, Data = {DateTime.Now.ToString("dd/MM/yy HH:mm")})");
                    }
                    else
                    {
                        Console.WriteLine("---");
                    }
                }
                catch (Clientes.ApiException apiEx)
                {
                    if (apiEx.StatusCode != ((int)HttpStatusCode.NotFound))
                    {
                        Console.WriteLine(apiEx.Message);
                    }
                }

                await Task.Delay(5000);
            }
        }
    }
}
