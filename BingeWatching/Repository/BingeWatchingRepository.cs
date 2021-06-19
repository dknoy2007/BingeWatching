using System;
using System.Collections.Generic;
using System.Linq;
using BingeWatching.Models;
using BingeWatching.Models.Enums;
using BingeWatching.Repository.Interfaces;

namespace BingeWatching.Repository
{
    public class BingeWatchingRepository : IBingeWatchingRepository
    {
        private readonly Dictionary<int, Dictionary<string, Content>> _users;
        private readonly Dictionary<int, HashSet<int>> _followers;
        private readonly Dictionary<int, HashSet<string>> _recommendations;
        
        public BingeWatchingRepository()
        {
            CurrentUserId = 0;

            _users = new Dictionary<int, Dictionary<string, Content>>();
            _followers = new Dictionary<int, HashSet<int>>();
            _recommendations = new Dictionary<int, HashSet<string>>();
        }

        public bool GetOrCreateUser(int userId)
        {
            if (!UserExists(userId))
            {
                _users.Add(userId, new Dictionary<string, Content>());
            }

            CurrentUserId = userId;

            return true;
        }

        public void AddContent(Content content)
        {
            _users[CurrentUserId].Add(content.Id, content);
        }

        public bool UserContentExists(string contentId)
        {
            return _users[CurrentUserId].ContainsKey(contentId);
        }

        public List<Content> GetContentHistory()
        {
            if (_users[CurrentUserId].Any())
            {
                return _users[CurrentUserId].Values.ToList();
            }
                
            Console.WriteLine($"User id={CurrentUserId} has no content history");

            return null;
        }

        public void UpdateContentRank(string contentId, int rank)
        {
            _users[CurrentUserId][contentId].Rank = rank;
        }

        public void Follow(int userToFollowId)
        {
            if (!UserExists(userToFollowId))
            {
                Console.WriteLine($"User id={userToFollowId} to follow doesn't exist");
                return;
            }

            if (_followers.ContainsKey(userToFollowId) && _followers[userToFollowId].Contains(CurrentUserId))
            {
                Console.WriteLine($"You're already following user id={userToFollowId}");
                return;
            }

            if (!_followers.ContainsKey(userToFollowId))
            {
                _followers.Add(userToFollowId, new HashSet<int>{ CurrentUserId });
            }
            else
            {
                _followers[userToFollowId].Add(CurrentUserId);
            }

            Console.WriteLine($"You're now following user id={userToFollowId}");
        }

        public void UnFollow(int userToFollowId)
        {
            if (!UserExists(userToFollowId))
            {
                Console.WriteLine($"User id={userToFollowId} to follow doesn't exist");
                return;
            }

            if (!_followers.ContainsKey(userToFollowId) || !_followers[userToFollowId].Contains(CurrentUserId))
            {
                Console.WriteLine($"You weren't following user id={userToFollowId}");
                return;
            }

            _followers[userToFollowId].Remove(CurrentUserId);

            Console.WriteLine($"You're not following user id={userToFollowId} anymore");
        }

        public HashSet<int> GetFollowers()
        {
            if (_followers.ContainsKey(CurrentUserId))
            {
                return _followers[CurrentUserId];
            }

            Console.WriteLine("You do not have any followers");

            return null;
        }

        public Tuple<int, Content> GetMovieRecommendation()
        {
            return GetFollowedUsersMovieRecommendation();
        }

        private int CurrentUserId { get; set; }

        private bool UserExists(int userId)
        {
            return _users.ContainsKey(userId);
        }

        private Tuple<int, Content> GetFollowedUsersMovieRecommendation()
        {
            var followedUserIds = GetFollowedUsers();

            if (!followedUserIds.Any())
            {
                Console.WriteLine($"Sorry, you're not following anybody");
                return null;
            }

            Tuple<int, Content> recommendedMovie = null;

            foreach (
                var userRecommendedMovie 
                in from followedUserId 
                    in followedUserIds 
                where _users[followedUserId].Any() 
                select GetFollowedUserMovieRecommendation(followedUserId) 
                into userRecommendedMovie 
                where userRecommendedMovie.Item2 != null 
                select userRecommendedMovie)
            {
                if (recommendedMovie == null && 
                    !_recommendations.ContainsKey(CurrentUserId) ||
                    !_recommendations[CurrentUserId].Contains(userRecommendedMovie.Item2.Id))
                {
                    recommendedMovie = userRecommendedMovie;
                    continue;
                }

                if (recommendedMovie != null &&
                    userRecommendedMovie.Item2.Id != recommendedMovie.Item2.Id &&
                    userRecommendedMovie.Item2.Rating > recommendedMovie.Item2.Rating &&
                    !_recommendations.ContainsKey(CurrentUserId) ||
                    !_recommendations[CurrentUserId].Contains(userRecommendedMovie.Item2.Id))
                {
                    recommendedMovie = userRecommendedMovie;
                }
            }

            if (recommendedMovie == null)
            {
                Console.WriteLine("Sorry, you've already watched all recommended movies");
                return null;
            }

            if (!_recommendations.ContainsKey(CurrentUserId))
            {
                _recommendations.Add(CurrentUserId, new HashSet<string>());
            }

            _recommendations[CurrentUserId].Add(recommendedMovie.Item2.Id);

            return recommendedMovie;
        }

        private List<int> GetFollowedUsers()
        {
            return _followers
                    .Where(x => x.Value.Contains(CurrentUserId))
                    .Select(x => x.Key)
                    .ToList();
        }

        private Tuple<int, Content> GetFollowedUserMovieRecommendation(int followedUserId)
        {
            var recommendedMovie = 
                _users[followedUserId]
                    .Where(x => x.Value.Type == ContentType.Movie)
                    .OrderByDescending(x => x.Value.Rating)
                    .Select(x => x.Value)
                    .FirstOrDefault();

            return new Tuple<int, Content>(followedUserId, recommendedMovie);
        }
    }
}