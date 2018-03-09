using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Autonomous.Impl;
using Microsoft.Xna.Framework;

namespace Autonomous
{
    public class GameOptions
    {
        public float MinLength { get; set; }
        public float MaxLength { get; set; }
        public float MinTraffic { get; set; }
        public float MaxTraffic { get; set; }
        public float TimeAcceleration { get; set; }
        public int Rounds { get; set; }

        public bool PlayerCollision { get; set; }
    }

    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 0 && (args[0][1] == '?' || args[0][1] == 'h'))
            {
                PrintHelp();
                return;
            }

            var options = OptionsFromArguments(args);
                if (args.Length > 0 && args[0] == "-quiet")
            {
                RunInSilentMode(options);
                return;
            }
            if (args.Length > 0 && args[0] == "-tournament")
            {
               RunTournament(options);
                return;
            }

            RunGui(options);
        }

        private static void PrintHelp()
        {
            Console.WriteLine("Autonomous -(quiet|tournament) -traffic:<traffic density (0-1)> -length:<course length> -timeAcceleration:<time boost> -playerCollision:(0|1)");
            Console.WriteLine();
            Console.WriteLine("Autonomous -quiet -timeAcceleration:2      -- starts game in quiet mode time is 2 times faster");
            Console.WriteLine("Autonomous -tournament -traffic:0.1-0.5 -length:1000-2000  -rounds:5    -- starts game in tournament mode, 5 round, course length is random between 1000 and 2000");
        }

        private static GameOptions OptionsFromArguments(string[] args)
        {
            Random r = new Random();
            var lengthStr = GetArg(args, "-length");
            float minLength = 1000f, maxLength = 1000f;
            if (lengthStr != null) { 
                var parts = lengthStr.Split('-');
                if (parts.Length == 1)
                {
                    minLength = float.Parse(lengthStr);
                    maxLength = float.Parse(lengthStr);
                }
                else
                {
                    minLength = float.Parse(parts[0]);
                    maxLength = float.Parse(parts[1]);
                }
            }

            var trafficStr = GetArg(args, "-traffic");
            float mintraffic = 0.1f, maxtraffic = 0.2f;
            if (trafficStr != null)
            {
                var parts = trafficStr.Split('-');
                if (parts.Length == 1)
                {
                    mintraffic = float.Parse(trafficStr);
                    maxtraffic = float.Parse(trafficStr);
                }
                else
                {
                    mintraffic = float.Parse(parts[0]);
                    maxtraffic = float.Parse(parts[1]);
                }
            }

            var roundStr = GetArg(args, "-rounds");
            int round = 10;
            if (roundStr != null)
            {
                    round = int.Parse(roundStr);
            }

            var timeStr = GetArg(args, "-timeAcceleration");
            float time = 1f;
            if (timeStr!= null)
            {
                time = float.Parse(timeStr);
            }
            bool playerCollision = false;
            var collsionStr = GetArg(args, "-playerCollision");
            if (collsionStr != null)
            {
                if (collsionStr == "1" || collsionStr.ToLower() == "true")
                    playerCollision = true;

            }

            return new GameOptions() {MinLength = minLength, MaxLength = maxLength, Rounds = round, TimeAcceleration = time, MinTraffic = mintraffic, MaxTraffic = maxtraffic, PlayerCollision = playerCollision};
        }

        private static string GetArg(string[] args, string name)
        {   
            foreach (var arg in args)
            {

                var parts = arg.Split(':');
                if (parts[0].ToLower() == name.ToLower())
                    return parts[1];
            }
            return null;
        }

        private static void RunInSilentMode(GameOptions options)
        {
            Random r= new Random();
            var length = (float) r.NextDouble() * (options.MaxLength - options.MinLength) + options.MinLength;
            var traffic = (float)r.NextDouble() * (options.MaxTraffic - options.MinTraffic) + options.MinTraffic;
            using (var game = new CarGame(length, traffic, options.PlayerCollision))
            {
                game.InitializeModel();
                var sw = Stopwatch.StartNew();
                int lastTick = (int)(sw.Elapsed.TotalMilliseconds * options.TimeAcceleration);
                while (!game.Stopped)
                {
                    int frameStart = (int)(sw.Elapsed.TotalMilliseconds * options.TimeAcceleration);
                    var elapsed = frameStart - lastTick;
                    lastTick = frameStart;
                    game.UpdateModel(new GameTime(TimeSpan.FromMilliseconds(frameStart), TimeSpan.FromMilliseconds(elapsed)));
                }
            }
        }

        private static void RunTournament(GameOptions options)
        {
            Random r = new Random();
            for (int i = 0; i < options.Rounds; i++)
            {
                var length = (float)r.NextDouble() * (options.MaxLength - options.MinLength) + options.MinLength;
                var traffic = (float)r.NextDouble() * (options.MaxTraffic - options.MinTraffic) + options.MinTraffic;
                using (var game = new CarGame(length, traffic, options.PlayerCollision))
                {
                    game.Run();
                }
            }
        }

        private static void RunGui(GameOptions options)
        {
            Random r = new Random();
            var length = (float)r.NextDouble() * (options.MaxLength - options.MinLength) + options.MinLength;
            var traffic = (float)r.NextDouble() * (options.MaxTraffic - options.MinTraffic) + options.MinTraffic;

            using (var game = new CarGame(length, traffic, options.PlayerCollision))
            {
                game.Run();
            }
        }

    }
}
