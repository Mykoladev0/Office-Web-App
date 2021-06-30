using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BullsBluffCore.Models;
using CoreDAL.Interfaces;
using CoreDAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoreApp.Controllers.Api
{

    //[Authorize]
    [Route("/api/[controller]")]
    [ApiController]//dotnet 2.1 attribute
    public class ShowsController : BaseAuthorizedAPIController
    {
        private readonly IShowService _showsService;
        private readonly IDogService _dogService;
        private readonly IStyleAndClassService _styleAndClassService;

        public ShowsController(IShowService showsService, IDogService dogService, IStyleAndClassService styleAndClassService)
        {
            _showsService = showsService;
            _dogService = dogService;
            _styleAndClassService = styleAndClassService;
        }


        [HttpGet("GetShowsCount")]
        [ProducesResponseType(200, Type = typeof(int))]
        public async Task<int> GetShowsCount() => await _showsService.GetShowsCount();

        [HttpGet("GetUpcomingShows")]
        [ProducesResponseType(200)]
        public async Task<ActionResult> GetUpcomingShows(DateTime? startDate = null, int? maxCount = 4)
        {
            DateTime fromDate = startDate ?? DateTime.UtcNow;
            if (fromDate.DayOfWeek == DayOfWeek.Friday)
            {
                //do nothing, we've got the weekend
            }
            if (fromDate.DayOfWeek == DayOfWeek.Saturday)
            {
                //go back a day to friday
                fromDate = fromDate.AddDays(-1);
            }
            if (fromDate.DayOfWeek == DayOfWeek.Sunday)
            {
                //go back to friday
                fromDate = fromDate.AddDays(-2);
            }
            if (fromDate.DayOfWeek == DayOfWeek.Monday)
            {
                //on monday probably want to look at the last weekend shows?
                //go back to friday
                fromDate = fromDate.AddDays(-3);
            }
            IQueryable<Shows> query = _showsService.GetUpcomingShows(fromDate);
            //char[] splitChars = new List<char> { '|' }.ToArray();
            query = query.Take(maxCount.Value).OrderBy(s => s.ShowDate).ThenBy(s => s.ShowId);
            var rtn = await query.Select(s => new SimpleShowDTO
            {
                ShowId = s.ShowId,
                ShowName = s.ShowName,
                ShowDate = s.ShowDate.Value,
                BreedList = s.BreedsShown,
                Address = s.Address,
            }).ToListAsync();
            Response.Headers.Add("x-total-count", rtn.Count.ToString());
            DateTime? lastModifiedDate = (await query.OrderByDescending(s => s.ModifyDate).FirstAsync()).ModifyDate;
            Response.Headers.Add("Last-Modified", lastModifiedDate.GetValueOrDefault(DateTime.MinValue).ToString());
            return Ok(rtn);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Shows>> GetShow(int id)
        {
            Shows show = await _showsService.GetShow(id);
            if (show == null)
            {
                return NotFound(id);
            }
            //get events
            ICollection<ShowResults> events = await _showsService.GetResultsForShow(id).ToListAsync();
            //group them
            //note that class has gender (uggh!) in the string
            var result = events.GroupBy(e => new { e.Breed, e.Style, e.Class })
                .Select(b => new
                {
                    Class = b.Key.Class.Replace("Male", "").Replace("Female", "").Replace("()", "").Trim(),
                    b.Key.Breed,
                    b.Key.Style,
                    Gender = b.Key.Class.Contains("Male") ? "Male" : b.Key.Class.Contains("Female") ? "Female" : "",
                    Results = b.Select(bn => bn).ToList()
                });
            ICollection<ShowParticipant> participants = await _showsService.GetParticipantsForShow(id).Include(p => p.Show).Include(p => p.Dog).ToListAsync();
            var rtn = new
            {
                show.ShowId,
                show.ShowName,
                show.ShowJudges,
                show.Abkcrep,
                ShowDate = show.ShowDate.Value,
                BreedList = show.BreedsShown,
                show.Address,
                LastModified = show.ModifyDate,
                Events = result,
                Participants = participants ?? new List<ShowParticipant>(),
            };
            Response.Headers.Add("x-total-count", rtn.Events.Count().ToString());
            DateTime? lastModifiedDate = events.OrderByDescending(s => s.ModifyDate).FirstOrDefault()?.ModifyDate;
            Response.Headers.Add("Last-Modified", lastModifiedDate.GetValueOrDefault(DateTime.MinValue).ToString());

            return Ok(rtn);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ShowResults>> PostShowResult(EventResultDTO result)
        {
            ShowResults rtn = await SaveResult(result);
            return CreatedAtAction(nameof(GetShow), new { id = rtn.Id }, rtn);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(202)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ShowResults>> UpdateShowResult(int id, EventResultDTO result)
        {
            ShowResults rtn = await SaveResult(result, id);
            Response.StatusCode = 202;
            return Ok(rtn);
        }

        [HttpPost("RegisterForShow")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ShowParticipant>> RegisterForShow(ShowParticipantDTO participant)
        {

            ShowParticipant p = await _showsService.AddParticipantToShow(participant.ShowId, participant.DogId, participant.ArmbandNumber);
            return CreatedAtAction(nameof(GetShow), new { id = p.Id }, p);//TODO: Change this response!
        }

        [HttpPut("UpdateRegistration")]
        [ProducesResponseType(202)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ShowParticipant>> UpdateRegistration(ShowParticipantDTO participant)
        {

            ShowParticipant p = await _showsService.UpdateShowParticipant(participant.Id.GetValueOrDefault(-1), participant.ShowId, participant.DogId, participant.ArmbandNumber);
            return Ok(p);
        }

        [HttpGet("GetParticipant")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ShowParticipant>> GetParticipant(int id, int showId)
        {

            IQueryable<ShowParticipant> q = _showsService.GetParticipantsForShow(showId);
            ShowParticipant rtn = await q.Include(p => p.Show).Include(p => p.Dog).FirstOrDefaultAsync();
            if (rtn != null)
            {
                return Ok(rtn);
            }
            return NotFound();
        }


        [HttpGet("GetParticipantsForShow")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ShowParticipant>> GetParticipantsForShow(int showId)
        {

            IQueryable<ShowParticipant> q = _showsService.GetParticipantsForShow(showId);
            var rtn = await q.Select(p => new
            {
                p.Id,
                p.ArmbandNumber,
                p.Show.ShowId,
                p.DateRegistered,
                Dog = new
                {
                    p.Dog.Id,
                    p.Dog.DogName,
                    p.Dog.BullyId,
                    p.Dog.AbkcNo,
                    p.Dog.OwnerId,
                    p.Dog.Breed,
                    p.Dog.Birthdate,
                    p.Dog.Gender
                }
            }).ToListAsync();
            if (rtn != null)
            {
                return Ok(rtn);
            }
            return NotFound();
        }

        private async Task<ShowResults> SaveResult(EventResultDTO result, int resultId = 0)
        {
            ShowResults toSave = new ShowResults()
            {
                Id = resultId,
                ArmbandNumber = result.ArmbandNumber,
                DogId = result.DogId,
                ModifiedBy = "INTERNET USER",
                NoComp = result.NoComp,
                Points = result.Points,
                ShowId = result.ShowId,
                //ChampPoints
                //Winning_ABKC
            };
            //get things
            //don't need showId, that's handled in service
            try
            {
                toSave.ClassTemplate = await _styleAndClassService.GetClassById(result.ClassId);

            }
            catch (Exception)
            {

                throw;
            }
            if (result.StyleId.HasValue)
            {
                toSave.StyleRef = await _styleAndClassService.GetStyleById(result.StyleId.Value);
            }
            Dogs dog = await _dogService.GetById(result.DogId);
            if (dog != null)
            {
                toSave.Winning_ABKC = dog.AbkcNo;
                toSave.Breed = dog.Breed;//could cause some problems because ~500 dogs don't have breed assigned
            }
            return await _showsService.SaveShowResult(toSave);
        }
    }
}