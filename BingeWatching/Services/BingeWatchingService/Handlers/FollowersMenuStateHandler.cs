using BingeWatching.Models.Enums;
using BingeWatching.Repository.Interfaces;
using BingeWatching.Services.BingeWatchingService.Handlers.Interfaces;

namespace BingeWatching.Services.BingeWatchingService.Handlers
{
    public class FollowersMenuStateHandler : IMenuStateHandler
    {
        private readonly IBingeWatchingRepository _repository;

        public FollowersMenuStateHandler(IBingeWatchingRepository repository)
        {
            _repository = repository;
        }

        public bool CanHandle(MenuState menuState)
        {
            return menuState == MenuState.Followers;
        }

        public void Handle()
        {
        }
    }
}