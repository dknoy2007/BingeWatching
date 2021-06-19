using System;
using BingeWatching.Models.Enums;
using BingeWatching.Services.BingeWatchingService.Handlers.Interfaces;

namespace BingeWatching.Services.BingeWatchingService.Handlers
{
    public class SwitchUserMenuStateHandler : IMenuStateHandler
    {
        private readonly IUserHandler _userHandler;

        public SwitchUserMenuStateHandler(IUserHandler userHandler)
        {
            _userHandler = userHandler;
        }

        public bool CanHandle(MenuState menuState)
        {
            return menuState == MenuState.SwitchUser;
        }

        public MenuState MenuState { get; set; }

        public void Handle()
        {
            Console.WriteLine("\n\nSwitching user.\n");
            _userHandler.Handle();
        }
    }
}