using System;
using System.Threading;

namespace SincronizadorLocal
{
    public class Program
    {
        public static bool atualizado;
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Verificando Arquivos !");
                Move move = new Move();
                atualizado = move.DirectoryCopy(string.Empty, string.Empty, true, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (atualizado)
                    Console.WriteLine("Arquivos atualizados com sucesso !");
                else
                    Console.WriteLine("Nenhum arquivo desatualizado !");
                Thread.Sleep(3000);
            }
        }
    }
}