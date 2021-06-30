using CoreDAL.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoreDAL.Interfaces
{
    public interface IShowService
    {
        Task<int> GetShowsCount();

        IQueryable<Shows> GetUpcomingShows(DateTime? dateFrom=null);

        IQueryable<ShowResults> GetResultsForShow(int showId);
        Task<Shows> GetShow(int showId);

        /// <summary>
        /// handles create and updates
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        Task<ShowResults> SaveShowResult(ShowResults showResult);
        /// <summary>
        /// attempts to update an existing result
        /// throws exception if existing result doesn't exist
        /// </summary>
        /// <param name="showResult"></param>
        /// <returns></returns>
        Task<ShowResults> UpdateShowResult(ShowResults showResult);

        IQueryable<ShowParticipant> GetParticipantsForShow(int showId);
        Task<ShowParticipant> AddParticipantToShow(int showId, int dogId, int? armbandNumber);

        /// <summary>
        /// primarily working off of participant id, but including all incase that isn't set so we can still find them
        /// </summary>
        /// <param name="participantId"></param>
        /// <param name="showId"></param>
        /// <param name="dogId"></param>
        /// <param name="armbandNumber"></param>
        /// <returns></returns>
        Task<ShowParticipant> UpdateShowParticipant(int participantId, int showId, int dogId, int? armbandNumber);
    }

}