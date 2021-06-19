using System;
using System.Collections.Generic;
using System.Linq;
using BingeWatching.Models.Enums;
using BingeWatching.Repository.Interfaces;
using BingeWatching.Services.BingeWatchingService.Handlers.Interfaces;

namespace BingeWatching.Services.BingeWatchingService.Handlers
{
    public class FollowersMenuStateHandler : IMenuStateHandler
    {
        private readonly HashSet<MenuState> _menuStates = new HashSet<MenuState>
        {
            MenuState.Follow,
            MenuState.UnFollow,
            MenuState.Followers
        };

        private readonly IBingeWatchingRepository _repository;

        public FollowersMenuStateHandler(IBingeWatchingRepository repository)
        {
            _repository = repository;
        }

        public bool CanHandle(MenuState menuState)
        {
            return _menuStates.Contains(menuState);
        }

        public MenuState MenuState { get; set; }

        public void Handle()
        {
            switch (MenuState)
            {
                case MenuState.Follow:
                case MenuState.UnFollow:
                    FollowOrUnFollowUser();
                    break;
                case MenuState.Followers:
                    GetFollowers();
                    break;
                default:
                    Console.WriteLine($"\n\nFailed to handle {MenuState} menu state");
                    break;
            }
        }

        private void GetFollowers()
        {
            Console.WriteLine("\n");

            var followers = _repository.GetCurrentUserFollowers();

            if (followers == null || !followers.Any())
            {
                Console.WriteLine("You do not have any followers");
                return;
            }

            PrintFollowers(followers);
        }

        private static void PrintFollowers(IReadOnlyList<int> followers)
        {
            if (followers.Count > 1)
            {
                Console.WriteLine($"You are followed by userIds: {string.Join(",", followers)}");
                return;
            }

            Console.WriteLine($"userId={followers[0]} is following you");
        }

        private void FollowOrUnFollowUser()
        {
            Console.WriteLine("\n");

            if (!AskForUserIdToFollowOrUnFollow(out var userId))
            {
                return;
            }

            var isFollowing = _repository.IsFollowingUser(userId);

            switch (isFollowing)
            {
                case false when MenuState == MenuState.UnFollow:
                    Console.WriteLine($"\nYou are not following userId={userId}");
                    return;

                case true when MenuState == MenuState.Follow:
                    Console.WriteLine($"\nYou are already following userId={userId}");
                    return;
            }

            if (MenuState == MenuState.Follow)
            {
                _repository.FollowUser(userId);
                Console.WriteLine($"\nYou are now following userId={userId}");
                return;
            }

            _repository.UnFollowUser(userId);
            Console.WriteLine($"\nYou're not following user id={userId} anymore");
        }

        private bool AskForUserIdToFollowOrUnFollow(out int userIdTo)
        {
            var action = MenuState == MenuState.Follow ? "follow" : "un-follow";
            
            while (true)
            {
                Console.WriteLine($"Please enter user id you want to {action}:");

                if (!int.TryParse(Console.ReadLine(), out userIdTo) || userIdTo <= 0)
                {
                    Console.WriteLine("\nInvalid userId: must be a whole number greater than 0\n");
                    continue;
                }

                if (userIdTo == _repository.CurrentUserId)
                {
                    Console.WriteLine($"\nInvalid userId: you cannot {action} yourself\n");
                    continue;
                }

                var userToFollowExist = _repository.IsUserExists(userIdTo);

                if (!userToFollowExist)
                {
                    Console.WriteLine($"\nInvalid userId to {action}: userId={userIdTo} not exist");
                    return false;
                }

                break;
            }

            return true;
        }
    }
}