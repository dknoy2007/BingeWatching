using BingeWatching.Models.Enums;

namespace BingeWatching.Services.BingeWatchingService.Handlers.Interfaces
{
    public interface IMenuStateHandler
    {
        bool CanHandle(MenuState menuState);
        void Handle();
        MenuState MenuState { get; set; }
    }
}