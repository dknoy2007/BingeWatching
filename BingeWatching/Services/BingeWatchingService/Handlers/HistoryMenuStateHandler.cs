using System;
using System.Linq;
using BingeWatching.Models.Enums;
using BingeWatching.Repository.Interfaces;
using BingeWatching.Services.BingeWatchingService.Handlers.Interfaces;

namespace BingeWatching.Services.BingeWatchingService.Handlers
{
    public class HistoryMenuStateHandler : IMenuStateHandler
    {
        private readonly IBingeWatchingRepository _repository;

        public HistoryMenuStateHandler(IBingeWatchingRepository repository)
        {
            _repository = repository;
        }

        public bool CanHandle(MenuState menuState)
        {
            return menuState == MenuState.History;
        }

        public MenuState MenuState { get; set; }

        public void Handle()
        {
            Console.WriteLine("\n");

            var contentHistory = _repository.GetContentHistory();

            if (contentHistory == null || !contentHistory.Any())
            {
                Console.WriteLine("Sorry, you do not have any content history");
                return;
            }

            Console.WriteLine($"Current user: {_repository.CurrentUserId}\n");

            for (var i = 0; i < contentHistory.Count; i++)
            {
                var content = $"{contentHistory[i].ToString(false)}";

                if (i == contentHistory.Count - 1)
                {
                    Console.Write(content);
                }
                else
                {
                    Console.WriteLine(content);
                }
            }
        }
    }
}