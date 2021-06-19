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
        private readonly Dictionary<int, List<int>> _followers;
        private readonly Dictionary<int, HashSet<string>> _recommendations;
        
        public BingeWatchingRepository()
        {
            CurrentUserId = 0;

            _users = new Dictionary<int, Dictionary<string, Content>>();
            _followers = new Dictionary<int, List<int>>();
            _recommendations = new Dictionary<int, HashSet<string>>();
        }

        public int CurrentUserId { get; private set; }

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
            return _users[CurrentUserId].Any()
                ? _users[CurrentUserId].Values.ToList()
                : null;
        }

        public void UpdateContentRank(string contentId, int rank)
        {
            _users[CurrentUserId][contentId].Rank = rank;
        }

        public void Follow(int userId)
        {
            if (!_followers.ContainsKey(userId))
            {
                _followers.Add(userId, new List<int>{ CurrentUserId });
            }
            else
            {
                _followers[userId].Add(CurrentUserId);
            }
        }

        public void UnFollow(int userId)
        {
            _followers[userId].Remove(CurrentUserId);
        }

        public bool IsFollowing(int userId)
        {
            return _followers.ContainsKey(userId) && _followers[userId].Contains(CurrentUserId);
        }

        public List<int> GetFollowers()
        {
            return _followers.ContainsKey(CurrentUserId) 
                ? _followers[CurrentUserId] 
                : null;
        }

        public Tuple<int, Content> GetMovieRecommendation(List<int> followedUserIds)
        {
            return GetFollowedUsersMovieRecommendation(followedUserIds);
        }

        public bool UserExists(int userId)
        {
            return _users.ContainsKey(userId);
        }

        public List<int> GetFollowedUsers()
        {
            return _followers
                .Where(x => x.Value.Contains(CurrentUserId))
                .Select(x => x.Key)
                .ToList();
        }

        private Tuple<int, Content> GetFollowedUsersMovieRecommendation(List<int> followedUserIds)
        {
            var recommendedMovie = GetFollowedUsersTopRankedMovie(followedUserIds);

            if (recommendedMovie == null)
            {
                return null;
            }

            if (!_recommendations.ContainsKey(CurrentUserId))
            {
                _recommendations.Add(CurrentUserId, new HashSet<string>());
            }

            _recommendations[CurrentUserId].Add(recommendedMovie.Item2.Id);
            
            // we treat recommendation as if the current user is watching it, so it is added to its already watched content list
            AddContent(recommendedMovie.Item2);

            return recommendedMovie;
        }

        private Tuple<int, Content> GetFollowedUsersTopRankedMovie(List<int> followedUserIds)
        {
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
                // check: 
                // 1. current user has yet to contain this content already
                // 2. it's the first followed user recommendation
                // 3. this content was yet to be recommended to current user
                if (!_users[CurrentUserId].Any() ||
                    !_users[CurrentUserId].ContainsKey(userRecommendedMovie.Item2.Id) &&
                    recommendedMovie == null &&
                    !_recommendations.Any() ||
                    !_recommendations.ContainsKey(CurrentUserId) ||
                    !_recommendations[CurrentUserId].Contains(userRecommendedMovie.Item2.Id))
                {
                    recommendedMovie = userRecommendedMovie;
                    continue;
                }

                // check: 
                // 1. current user has yet to contain this content already
                // 2. it's not the first followed user recommendation
                // 3. it's different and ranked higher than the current top ranked content that was found
                // 3. this content was yet to be recommended to current user
                if (!_users[CurrentUserId].Any() ||
                    !_users[CurrentUserId].ContainsKey(userRecommendedMovie.Item2.Id) &&
                    recommendedMovie != null &&
                    userRecommendedMovie.Item2.Id != recommendedMovie.Item2.Id &&
                    userRecommendedMovie.Item2.Rating > recommendedMovie.Item2.Rating &&
                    !_recommendations.Any() ||
                    !_recommendations.ContainsKey(CurrentUserId) ||
                    !_recommendations[CurrentUserId].Contains(userRecommendedMovie.Item2.Id))
                {
                    recommendedMovie = userRecommendedMovie;
                }
            }

            return recommendedMovie;
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