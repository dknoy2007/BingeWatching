using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BingeWatching.Models.Enums;
using BingeWatching.Repository;
using BingeWatching.Repository.Interfaces;
using BingeWatching.Services.BingeWatchingService.Handlers;
using BingeWatching.Services.BingeWatchingService.Handlers.Interfaces;

namespace BingeWatching.Services.BingeWatchingService.Menu
{
    public sealed class Menu
    {
        private readonly IUserHandler _userHandler;
        private readonly IList<IMenuStateHandler> _menuStateHandlers;

        public Menu()
        {
            IBingeWatchingRepository repository = new BingeWatchingRepository();

            _userHandler = new UserHandler(repository);

            _menuStateHandlers = new List<IMenuStateHandler>
            {
                new ContentMenuStateHandler(repository),
                new SwitchUserMenuStateHandler(_userHandler),
                new HistoryMenuStateHandler(repository),
                new ExitMenuStateHandler(),
                new FollowersMenuStateHandler(repository)
            };
        }

        public void Run()
        {
            _userHandler.Handle();

            while (true)
            {
                var menuState = GetMenuState();

                var handler = _menuStateHandlers.FirstOrDefault(h => h.CanHandle(menuState));

                if (handler == null)
                {
                    Console.WriteLine($"\n\nFailed to locate handler for handling {menuState} menu state");
                    Console.WriteLine("Goodbye, see you again soon.\n");
                    Thread.Sleep(1000);
                    Environment.Exit(0);
                }

                handler.MenuState = menuState;

                handler.Handle();
            }
        }

        private static MenuState GetMenuState()
        {
            while (true)
            {
                Console.WriteLine("\nBinge Watching menu");
                Console.WriteLine("-------------------");

                var allMenuStates = GetAllPossibleMenuStates();
                
                foreach (var menuState in allMenuStates)
                {
                    Console.WriteLine($"Press {(char) menuState} for {menuState}");
                }

                Console.WriteLine("\nEnter your choice:");

                var choice = char.ToUpperInvariant(Console.ReadKey().KeyChar);
                
                var menuStateChoice = (MenuState) Enum.ToObject(typeof(MenuState), choice);
                
                if (allMenuStates.Contains(menuStateChoice))
                {
                    return menuStateChoice;
                }

                Console.WriteLine("\n\nInvalid Choice! - Please choose again");
            }
        }

        private static MenuState[] GetAllPossibleMenuStates() =>
            (MenuState[]) Enum.GetValues(typeof(MenuState));
    }
}