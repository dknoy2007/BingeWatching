using System;
using System.Collections.Generic;
using BingeWatching.Models;

namespace BingeWatching.Repository.Interfaces
{
    public interface IBingeWatchingRepository
    {
        public int CurrentUserId { get; }
        bool GetOrCreateUser(int userId);
        void AddContent(Content content);
        bool UserContentExists(string contentId);
        List<Content> GetContentHistory();
        void UpdateContentRank(string contentId, int rank);
        void Follow(int userId);
        bool IsFollowing(int userId);
        void UnFollow(int userId);
        List<int> GetFollowers();
        Tuple<int, Content> GetMovieRecommendation(List<int> followedUserIds);
        bool UserExists(int userId);
        List<int> GetFollowedUsers();
    }
}