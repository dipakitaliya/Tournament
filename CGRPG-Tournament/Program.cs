using Microsoft.Owin.Hosting;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CGRPG_TournamentLib;
using CGRPG_TournamentLib.Contexts;
using CGRPG_TournamentLib.Helpers;
using CommandLine;

namespace CGRPG_Tournament
{
    
    public static class CgrpgTournament
    {
        private static bool _keepRunning = true;
        
        static async Task Main(string[] args)
        {
            string baseAddress = 
#if DOCKER
            "http://*:9000/";
#else
            "http://localhost:9000/";
#endif
            

            Console.CancelKeyPress += delegate(object? sender, ConsoleCancelEventArgs e) 
            {
                e.Cancel = true;
                CgrpgTournament._keepRunning = false;
            };
            TournamentContext.Options = Parser.Default.ParseArguments<Options>(args).Value;
            
            // Start OWIN host 
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                Console.WriteLine(
#if STAGING
                    "Staging" +
#else
                    "Production" +
#endif
                    $" server started at {baseAddress}"
                );
                while (CgrpgTournament._keepRunning) 
                {
                }
            }
            Console.WriteLine("exiting");
        }
    }
}