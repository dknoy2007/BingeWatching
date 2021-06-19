using System;
using System.Threading.Tasks;
using BingeWatching.Services.BingeWatchingService.Menu;

namespace BingeWatching
{
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Netflix Binge watching service.\n");
            new Menu().Run();
        }
    }
}