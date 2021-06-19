using System;
using System.Collections.Generic;
using BingeWatching.Models;

namespace BingeWatching.Repository.Interfaces
{
    public interface IBingeWatchingRepository
    {
        bool GetOrCreateUser(int userId);
        void AddContent(Content content);
        bool UserContentExists(string contentId);
        List<Content> GetContentHistory();
        void UpdateContentRank(string contentId, int rank);
        void Follow(int userToFollowId);
        void UnFollow(int userToFollowId);
        HashSet<int> GetFollowers();
        Tuple<int, Content> GetMovieRecommendation();
    }
}