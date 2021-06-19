using System;
using System.Threading;
using BingeWatching.Models;
using BingeWatching.Models.Enums;
using BingeWatching.Repository.Interfaces;
using System.Linq;

namespace BingeWatching.Services.BingeWatchingService.Handlers
{
    public class ContentKindHandler
    {
        private const int SleepInMillis = 10000;
        private const int MinUserContentRank = 0;
        private const int MaxUserContentRank = 10;

        private readonly NetflixService.NetflixService _netflix;
        private readonly IBingeWatchingRepository _repository;

        public ContentKindHandler(IBingeWatchingRepository repository)
        {
            _repository = repository;
            _netflix = new NetflixService.NetflixService();
        }

        public void Handle(ContentKind contentKind)
        {
            switch (contentKind)
            {
                case ContentKind.TvShow:
                case ContentKind.Movie:
                case ContentKind.Any:
                    HandleRandomContent(contentKind);
                    break;
                case ContentKind.Recommendation:
                    HandleMovieRecommendation();
                    break;
                default:
                    Console.WriteLine($"\nInvalid content kind {contentKind}"); 
                    break;
            }
        }

        private void HandleRandomContent(ContentKind kind)
        {
            var randomContent = GetRandomContentFromNetflix(kind);

            _repository.AddContent(randomContent);

            Console.WriteLine($"\nYou are now watching '{randomContent.Title}'");

            WaitForFinishWatching(randomContent.Id, randomContent.Title, randomContent.Type);
        }

        private Content GetRandomContentFromNetflix(ContentKind kind)
        {
            while (true)
            {
                var randomContent = _netflix.GetRandomContent(kind);

                if (randomContent != null && !_repository.UserContentExists(randomContent.Id))
                {
                    return randomContent;
                }
            }
        }

        private void WaitForFinishWatching(string contentId, string title, ContentType type)
        {
            Thread.Sleep(SleepInMillis);

            Console.WriteLine();

            while (true)
            {
                var allUserReplies = GetAllUserReplies();

                Console.WriteLine($"Did you finish watching '{title}'?");

                foreach (var replay in allUserReplies)
                {
                    Console.WriteLine($"Press {(char)replay} for {replay}");
                }

                var choice = char.ToUpperInvariant(Console.ReadKey().KeyChar);

                var replayChoice = (UserReplay)Enum.ToObject(typeof(UserReplay), choice);

                Console.WriteLine();

                if (allUserReplies.Contains(replayChoice))
                {
                    if (replayChoice == UserReplay.No)
                    {
                        Console.WriteLine($"\nOK, enjoy watching the {type}\n");
                        Thread.Sleep(SleepInMillis);
                        continue;
                    }

                    AskUserToSetContentRank(contentId);

                    break;
                }

                Console.WriteLine("\nInvalid replay - Please replay again\n");
            }
        }

        private void AskUserToSetContentRank(string contentId)
        {
            while (true)
            {
                Console.WriteLine($"\nPlease enter content rank between {MinUserContentRank}-{MaxUserContentRank}:");

                if (!int.TryParse(Console.ReadLine(), out var rank) || 
                    rank < MinUserContentRank || 
                    rank > MaxUserContentRank)
                {
                    Console.WriteLine($"\nInvalid user rank: user rank must be between {MinUserContentRank}-{MaxUserContentRank}");
                    continue;
                }

                _repository.UpdateContentRank(contentId, rank);

                break;
            }
        }

        private static UserReplay[] GetAllUserReplies() =>
            (UserReplay[])Enum.GetValues(typeof(UserReplay));

        private void HandleMovieRecommendation()
        {
            var followedUserIds = _repository.GetFollowedUsers();

            if (!followedUserIds.Any())
            {
                Console.WriteLine("\nYou are not following anybody");
                return;
            }

            var recommendedMovie = _repository.GetMovieRecommendation(followedUserIds);

            if (recommendedMovie == null)
            {
                Console.WriteLine("\nSorry, there are no movie recommendations, or you've probably already watched all recommended movies");
                return;
            }

            Console.WriteLine($"\nMovie recommended by userId={recommendedMovie.Item1}:");
            Console.WriteLine("------------------------------");

            Console.WriteLine($"{recommendedMovie.Item2.ToString(false)}");
        }
    }
}