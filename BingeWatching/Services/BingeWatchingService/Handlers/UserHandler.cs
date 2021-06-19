using System;
using BingeWatching.Repository.Interfaces;
using BingeWatching.Services.BingeWatchingService.Handlers.Interfaces;

namespace BingeWatching.Services.BingeWatchingService.Handlers
{
    public class UserHandler : IUserHandler
    {
        private readonly IBingeWatchingRepository _repository;

        public UserHandler(IBingeWatchingRepository repository)
        {
            _repository = repository;
        }

        public void Handle()
        {
            while(true)
            {
                Console.WriteLine("Please enter user id:");

                if (!int.TryParse(Console.ReadLine(), out var userId) || userId <= 0)
                {
                    Console.WriteLine("\nInvalid userId: must be a whole number greater than 0\n");
                    continue;
                }

                var status = _repository.SetOrCreateCurrentUser(userId);

                if (status)
                {
                    break;
                }
            }
        }
    }
}