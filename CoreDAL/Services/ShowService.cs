using CoreDAL.Interfaces;
using CoreDAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreDAL.Services
{
    public class ShowService : IShowService
    {
        private readonly ABKCOnlineContext _context;
        private readonly IJudgeService _judgeService;

        public ShowService(ABKCOnlineContext context, IJudgeService judgeService)
        {
            _context = context;
            _judgeService = judgeService;
        }

        public IQueryable<ShowResults> GetResultsForShow(int showId)
        {
            return _context.ShowResults.Include(r => r.StyleRef).Include(r => r.ClassTemplate).Where(r => r.ShowId == showId);
        }

        public async Task<int> GetShowsCount()
        {
            return await _context.Shows.CountAsync();
        }

        public IQueryable<Shows> GetUpcomingShows(DateTime? dateFrom = null)
        {
            try
            {
                dateFrom = dateFrom ?? DateTime.UtcNow;
                IQueryable<Shows> query = _context.Shows.Where(s => s.ShowDate.HasValue && s.ShowDate.Value.Date >= dateFrom.Value.Date);
                return query;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<Shows> GetShow(int showId)
        {
            Shows found = await _context.Shows.FirstOrDefaultAsync(s => s.ShowId == showId);
            if (found == null)
            {
                return null;
            }
            //hydrate judges
            if (found.JudgeId.HasValue)
            {
                //go fetch
                //var judge = await _context.Judges.FirstOrDefaultAsync(j => j.Id == found.JudgeId.Value);
                Judges judge = await _judgeService.GetById(found.JudgeId.Value);
                if (judge != null)
                {
                    found.ShowJudges.Add(judge);
                }
            }
            //check free text judges
            if (!string.IsNullOrEmpty(found.Judge1))
            {
                Judges judge = await _judgeService.FindByName(found.Judge1);
                if (judge != null && found.ShowJudges.FirstOrDefault(j => j.Id == judge.Id) == null)
                {
                    found.ShowJudges.Add(judge);
                }
            }
            if (!string.IsNullOrEmpty(found.Judge2))
            {
                Judges judge = await _judgeService.FindByName(found.Judge2);
                if (judge != null && found.ShowJudges.FirstOrDefault(j => j.Id == judge.Id) == null)
                {
                    found.ShowJudges.Add(judge);
                }
            }
            //there will be one named judges, that should be reported!
            return found;
        }

        /// <summary>
        /// handles create and updates
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public async Task<ShowResults> SaveShowResult(ShowResults showResult)
        {
            //get show, see if it is locked
            Shows show = await _context.Shows.FindAsync(showResult.ShowId);
            if (show == null)
            {
                throw new System.Data.RowNotInTableException($"Show with an id of {showResult.ShowId} is not in the system, cannot save result.");
            }
            if (show.FinalizedDate.HasValue)
            {
                throw new System.Data.ConstraintException($"Show {show.ShowName} has been marked complete. An admin needs to re-open the show prior to saving the result");
            }
            if (showResult.ClassTemplate == null)
            {
                throw new System.Data.ConstraintException($"Result does not have a class defined with the ClassTemplate property, it cannot be saved.");
            }
            //see if it already exists based on id
            if (showResult.Id > 0)
            {
                //considered an update IF we can find it in the system
                //if not in the system, throw exception
                return await UpdateShowResult(showResult);

            }
            //TODO:Switch to ClassTemplate reference!

            //no id, see if it already exists in the show based on breed/style/class/gender combination WITH armband #
            IQueryable<ShowResults> q = _context.ShowResults.Where(r => r.Breed == showResult.Breed && r.ClassTemplate.ClassId == showResult.ClassTemplate.ClassId);
            if (showResult.StyleRef != null)
            {
                q = q.Where(r => r.StyleRef.Id == showResult.StyleRef.Id);
            }
            ShowResults existing = await q.FirstOrDefaultAsync();
            if (existing != null)
            {
                //TODO: think about points/champ points/ duplicates
                //check armband and points
                if (existing.ArmbandNumber == showResult.ArmbandNumber)
                {
                    //collision, return result to user with found one for resolution
                    throw new DataCollisionException<ShowResults>("Result for armband and event already exists for show")
                    {
                        OriginalData = existing,
                        IncomingData = showResult,
                    };
                }
            }
            //cleanup class name, use link instead
            showResult.Class = showResult.ClassTemplate.Name;
            //classes are required, but styles are optional
            showResult.Style = showResult.StyleRef != null ? showResult.StyleRef.StyleName : "-----------";

            //we are in the clear, add it to the show
            DateTime insertDate = DateTime.UtcNow;
            showResult.CreateDate = insertDate;
            showResult.ModifyDate = insertDate;

            var a = await _context.ShowResults.AddAsync(showResult);
            _context.SaveChanges();
            return showResult;

        }

        /// <summary>
        /// attempts to update an existing result
        /// throws exception if existing result doesn't exist
        /// </summary>
        /// <param name="showResult"></param>
        /// <returns></returns>
        public async Task<ShowResults> UpdateShowResult(ShowResults showResult)
        {
            try
            {
                ShowResults originalResult = await _context.ShowResults.FirstOrDefaultAsync(d => d.Id == showResult.Id);
                if (originalResult == null)
                {
                    throw new System.Data.RowNotInTableException($"Cannot update a result that does not exist in the system with ID: {showResult.Id}");
                }
                //cleanup class name, use link instead
                showResult.Class = showResult.ClassTemplate.Name;
                showResult.Style = showResult.StyleRef != null ? showResult.StyleRef.StyleName : "-----------";
                showResult.CreateDate = originalResult.CreateDate;
                showResult.ModifyDate = DateTime.UtcNow;
                //TODO: research what happens with navigation properties once those are added
                _context.Entry(originalResult).CurrentValues.SetValues(showResult);
                await _context.SaveChangesAsync();
                return originalResult;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Show Participants

        public IQueryable<ShowParticipant> GetParticipantsForShow(int showId)
        {
            IQueryable<ShowParticipant> q = _context.ShowParticipants.Where(p => p.Show.ShowId == showId);
            return q;
        }
        public async Task<ShowParticipant> AddParticipantToShow(int showId, int dogId, int? armbandNumber)
        {
            Shows show = await _context.Shows.FindAsync(showId);
            if (show == null)
            {
                throw new System.Data.RowNotInTableException($"Cannot add a participant to a show that does not exist in the system with ID: {showId}");
            }
            Dogs dog = await _context.Dogs.FindAsync(dogId);
            if (dog == null)
            {
                throw new System.Data.RowNotInTableException($"Cannot add a participant for a dog that does not exist in the system with ID: {dogId}");
            }
            if (armbandNumber.HasValue)
            {
                //check to see participant with armband # already exists for show
                IQueryable<ShowParticipant> matchingParticipants = _context.ShowParticipants.Where(p => p.Show.ShowId == showId && p.ArmbandNumber == armbandNumber.Value);
                bool armbandExists = await matchingParticipants.AnyAsync();
                if (armbandExists)
                {
                    throw new System.Data.InvalidConstraintException($"Armband # {armbandNumber} already exists for the show");
                }
            }
            //we've passed our tests, let's go!
            ShowParticipant participant = new ShowParticipant()
            {
                ArmbandNumber = armbandNumber,
                Show = show,
                Dog = dog,
                DateRegistered = DateTime.UtcNow,
            };
            _context.ShowParticipants.Add(participant);
            await _context.SaveChangesAsync();

            return participant;
        }

        /// <summary>
        /// primarily working off of participant id, but including all incase that isn't set so we can still find them
        /// </summary>
        /// <param name="participantId"></param>
        /// <param name="showId"></param>
        /// <param name="dogId"></param>
        /// <param name="armbandNumber"></param>
        /// <returns></returns>
        public async Task<ShowParticipant> UpdateShowParticipant(int participantId, int showId, int dogId, int? armbandNumber)
        {
            ShowParticipant found = await _context.ShowParticipants.Include(p => p.Show).Include(p => p.Dog).FirstOrDefaultAsync(p => p.Id == participantId);
            Shows show = null;
            Dogs dog = null;
            if (found == null)
            {
                //backup!
                show = await _context.Shows.FindAsync(showId);
                if (show == null)
                {
                    throw new System.Data.RowNotInTableException($"Cannot update a participant without an id to a show that does not exist in the system with ID: {showId}");
                }
                dog = await _context.Dogs.FindAsync(dogId);
                if (dog == null)
                {
                    throw new System.Data.RowNotInTableException($"Cannot update a participant without an id for a dog that does not exist in the system with ID: {dogId}");
                }
                found = await _context.ShowParticipants.Where(p => p.Show.ShowId == showId && p.Dog.Id == dogId).FirstOrDefaultAsync();
                if (found == null)
                {
                    throw new System.Data.RowNotInTableException($"Cannot update a participant that does not exist for the show.");
                }
            }
            else
            {
                //backwards setting incase dogid/showid weren't passed and we were relying on participant Id
                //NOTE, probably not needed!
                showId = found.ShowId.Value;
                dogId = found.DogId.Value;
            }
            if (armbandNumber.HasValue)
            {
                //check to see participant with armband # already exists for show
                IQueryable<ShowParticipant> matchingParticipants = _context.ShowParticipants.Where(p => p.Show.ShowId == showId && p.ArmbandNumber == armbandNumber.Value && p.Dog.Id != found.Dog.Id);
                bool armbandExists = await matchingParticipants.AnyAsync();
                if (armbandExists)
                {
                    throw new System.Data.InvalidConstraintException($"Armband # {armbandNumber} already exists for the show");
                }
            }
            found.ArmbandNumber = armbandNumber;
            await _context.SaveChangesAsync();
            return found;
        }
        #endregion
    }
}
