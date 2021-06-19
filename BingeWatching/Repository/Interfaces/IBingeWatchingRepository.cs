using System;
using System.Collections.Generic;
using BingeWatching.Models;

namespace BingeWatching.Repository.Interfaces
{
    public interface IBingeWatchingRepository
    {
        public int CurrentUserId { get; }
        bool SetOrCreateCurrentUser(int userId);
        void AddContent(Content content);
        bool IsUserContentExists(string contentId);
        List<Content> GetContentHistory();
        void UpdateContentRank(string contentId, int rank);
        void FollowUser(int userId);
        bool IsFollowingUser(int userId);
        void UnFollowUser(int userId);
        List<int> GetCurrentUserFollowers();
        Tuple<int, Content> GetRecommendedMovieFromFollowedUsers(List<int> followedUserIds);
        bool IsUserExists(int userId);
        List<int> GetUsersFollowedByCurrentUser();
    }
}