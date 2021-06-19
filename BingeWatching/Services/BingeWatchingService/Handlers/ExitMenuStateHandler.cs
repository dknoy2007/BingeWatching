using System;
using System.Threading;
using BingeWatching.Models.Enums;
using BingeWatching.Services.BingeWatchingService.Handlers.Interfaces;

namespace BingeWatching.Services.BingeWatchingService.Handlers
{
    public class ExitMenuStateHandler : IMenuStateHandler
    {
        public bool CanHandle(MenuState menuState)
        {
            return menuState == MenuState.Exit;
        }

        public MenuState MenuState { get; set; }

        public void Handle()
        {
            ExitBingeWatchingMenuGracefully();
        }

        private static void ExitBingeWatchingMenuGracefully()
        {
            Console.WriteLine("\n\nGoodbye, see you again soon.\n");
            Thread.Sleep(1000);
            Environment.Exit(0);
        }
    }
}