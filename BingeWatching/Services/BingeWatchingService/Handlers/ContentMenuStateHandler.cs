using System;
using System.Linq;
using BingeWatching.Models.Enums;
using BingeWatching.Repository.Interfaces;
using BingeWatching.Services.BingeWatchingService.Handlers.Interfaces;

namespace BingeWatching.Services.BingeWatchingService.Handlers
{
    public class ContentMenuStateHandler : IMenuStateHandler
    {
        private readonly ContentKindHandler _handler;
        
        public ContentMenuStateHandler(IBingeWatchingRepository repository)
        {
            _handler = new ContentKindHandler(repository);
        }

        public bool CanHandle(MenuState menuState)
        {
            return menuState == MenuState.Content;
        }

        public MenuState MenuState { get; set; }

        public void Handle()
        {
            var contentKind = GetContentKind();
            _handler.Handle(contentKind);
        }

        private static ContentKind GetContentKind()
        {
            Console.WriteLine("\n");

            while (true)
            {
                var allContentKinds = GetAllPossibleContentKinds();

                foreach (var contentKind in allContentKinds)
                {
                    Console.WriteLine($"Press {(int)contentKind} for {contentKind}");
                }

                Console.WriteLine("\nEnter your choice:");

                if (int.TryParse(Console.ReadLine(), out var choice))
                {
                    var contentKindsChoice = (ContentKind)Enum.ToObject(typeof(ContentKind), choice);

                    if (allContentKinds.Contains(contentKindsChoice))
                    {
                        return contentKindsChoice;
                    }
                }

                Console.WriteLine("\nInvalid Choice! - Please choose again\n");
            }
        }

        private static ContentKind[] GetAllPossibleContentKinds() =>
            (ContentKind[])Enum.GetValues(typeof(ContentKind));
    }
}