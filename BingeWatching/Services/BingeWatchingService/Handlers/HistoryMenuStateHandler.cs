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

        public void Handle()
        {
        }
    }
}