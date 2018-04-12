using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ift
{
    class Program
    {
        private static long totalMs = 0;

        private static HttpClient httpClient = new HttpClient();

        static void Main(string[] args)
        {
            ServicePointManager.DefaultConnectionLimit = 100;

            getIfts().Wait();
        }

        private static async Task getIfts()
        {
            Console.WriteLine("Nombre d'utilisateurs ? ");
            string line = Console.ReadLine();
            int nbUtilisateurs = int.Parse(line);

            Console.WriteLine("Nombre d'IFT par utilisateurs ? ");
            line = Console.ReadLine();
            int nbIftParUtilisateur = int.Parse(line);

            totalMs = 0;
            var watch = System.Diagnostics.Stopwatch.StartNew();

            await getParallelIft(nbUtilisateurs, nbIftParUtilisateur);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("\n\nTemps total : " + elapsedMs);
            Console.WriteLine("Temps moyen par ift : " + (totalMs / (nbUtilisateurs * nbIftParUtilisateur)) + "\n\n");

            getIfts().Wait();
        }

        private static async Task getParallelIft(int nbUtilisateur, int nbIftParUtilisateur)
        {
            var allTasks = new List<Task>();
            for (int i = 0; i < nbUtilisateur; i++)
            {
                allTasks.Add(getChainingIft(nbIftParUtilisateur, i));
            }

            await Task.WhenAll(allTasks);
        }

        private static async Task getChainingIft(int nbIftParUtilisateur, int i)
        {
            for (int j = 0; j < nbIftParUtilisateur; j++)
            {
                string url = "https://alim-pprd.agriculture.gouv.fr/ift-api/api/ift/traitement/certifie?campagneIdMetier=2018&numeroAmmIdMetier=2010441&cultureIdMetier=1081&cibleIdMetier=146&typeTraitementIdMetier=T22&uniteIdMetier=U4&dose=1.5&facteurDeCorrection=71&produitLibelle=ACAKILL";
                var watch = System.Diagnostics.Stopwatch.StartNew();
                await httpClient.GetAsync(url);
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                totalMs += elapsedMs;
                Console.Write(".");
            }
        }
    }
}
