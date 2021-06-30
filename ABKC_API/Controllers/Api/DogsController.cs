using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreDAL.Interfaces;
using CoreDAL.Models;
using CoreDAL.Models.v2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Sieve.Models;
using Sieve.Services;

namespace CoreApp.Controllers.Api
{

    //[Authorize]
    [Route("/api/[controller]")]
    [ApiController]//dotnet 2.1 attribute
    public class DogsController : BaseAuthorizedAPIController
    {
        private readonly IDogService _dogService;
        private readonly ISieveProcessor _sieveProcessor;
        private readonly IMapper _autoMapper;

        public DogsController(IDogService dogService, ISieveProcessor sieveProcessor, IMapper autoMapper)
        {
            _dogService = dogService;
            _sieveProcessor = sieveProcessor;
            _autoMapper = autoMapper;
        }


        /// <summary>
        /// finds all dogs with a name containing the search text
        /// </summary>
        /// <param name="dogName"></param>
        /// <returns></returns>
        [HttpGet("GetDogsByName")]
        [ProducesResponseType(200, Type = typeof(ICollection<CoreDAL.Models.DTOs.ABKCDogDTO>))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ICollection<CoreDAL.Models.DTOs.ABKCDogDTO>>> GetDogsByName(string dogName)
        {
            try
            {
                var foundDogs = await _dogService.GetDogsMatchingName(dogName);
                ICollection<CoreDAL.Models.DTOs.ABKCDogDTO> dtos = _autoMapper.Map<ICollection<CoreDAL.Models.DTOs.ABKCDogDTO>>(foundDogs);
                if (dtos != null && dtos.Any())
                {
                    foreach (CoreDAL.Models.DTOs.ABKCDogDTO d in dtos)
                    {
                        //get the breed, color, and id from new table if it exists
                        BaseDogModel found = await _dogService.GetDogByOldTableId(d.OriginalTableId, false);
                        if (found != null)
                        {
                            d.BreedId = found.Breed != null ? found.Breed.Id : 0;
                            d.ColorId = found.Color != null ? found.Color.Id : 0;
                            d.Id = found.Id;
                        }
                    }
                }
                return Ok(dtos ?? new List<CoreDAL.Models.DTOs.ABKCDogDTO>());
                // return foundDogs;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        /// <summary>
        /// Searches the ABKC System for a dog with a matching ABKC Number
        /// </summary>
        /// <param name="abkcNo">Expecting a 6 digit abkc number or a string formatted NNN,NNN</param>
        /// <returns></returns>
        [HttpGet("GetDogsByABKCNumber")]
        [ProducesResponseType(200, Type = typeof(CoreDAL.Models.DTOs.ABKCDogDTO))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<CoreDAL.Models.DTOs.ABKCDogDTO>> GetDogsByABKCNumber(string abkcNo)
        {
            try
            {
                var foundDog = await _dogService.GetByABKCNo(abkcNo);
                if (foundDog != null)
                {
                    CoreDAL.Models.DTOs.ABKCDogDTO dto = _autoMapper.Map<CoreDAL.Models.DTOs.ABKCDogDTO>(foundDog);
                    //get the breed, color, and id from new table if it exists
                    BaseDogModel found = await _dogService.GetDogByOldTableId(dto.OriginalTableId, false);
                    if (found != null)
                    {
                        dto.BreedId = found.Breed != null ? found.Breed.Id : 0;
                        dto.ColorId = found.Color != null ? found.Color.Id : 0;
                        dto.Id = found.Id;
                    }
                    return Ok(dto);
                }
                return NotFound();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }

        }


        /// <summary>
        /// Retrieves a Dog by the originaltable id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Dogs))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Dogs>> GetDogById(string id)
        {
            try
            {
                var foundDog = await _dogService.GetById(id);
                if (foundDog == null)
                {
                    return NotFound();
                }
                return Ok(foundDog);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        /// <summary>
        /// Retrieves a Dog by the new ABKC Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/fromABKCId/{id}")]
        [ProducesResponseType(200, Type = typeof(CoreDAL.Models.v2.BaseDogModel))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<CoreDAL.Models.v2.BaseDogModel>> GetDogByABKCId(string id)
        {
            return BadRequest("Endpoint is not yet implemented");
            // CoreDAL.Models.v2.BaseDogModel foundDog = await _dogService.GetById(id);
            // if (foundDog == null)
            // {
            //     return NotFound();
            // }
            // return foundDog;
        }


        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Dogs>> CreateDogAsync(Dogs newDog)
        {
            //following is not needed in core 2.1
            // if (!ModelState.IsValid)
            // {
            //     return BadRequest(ModelState);
            // }
            //do we need to validate here since the service does it?
            if (newDog.Id > 0)
            {
                return BadRequest("A post request is idempotent and cannot have an id assigned. Ids are autogenerated");
            }
            if (string.IsNullOrEmpty(newDog.DogName))
            {
                return BadRequest("Dog names must be supplied");
            }

            int newId = await _dogService.AddDog(newDog);

            return CreatedAtAction(nameof(GetDogById),
                new { id = newDog.Id }, newDog);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(202)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Dogs>> UpdateDogAsync(Dogs dogToUpdate)
        {

            Dogs foundDog = await _dogService.GetById(dogToUpdate.Id.ToString());
            if (foundDog == null)
            {
                return NotFound(dogToUpdate.Id);
            }
            //rehydrate navigation properties (owner, coowner, etc?)
            int newId = await _dogService.UpdateDog(dogToUpdate);

            return Ok();
        }

        [HttpGet("GetColorList")]
        [ProducesResponseType(200, Type = typeof(ICollection<string>))]
        public async Task<ICollection<string>> GetColorList() => await _dogService.GetAllColorsAsync();

        [HttpGet("GetMatchingDogs")]
        [ProducesResponseType(200)]
        public async Task<ActionResult> GetMatchingDogs(string searchText)
        {
            if (searchText.Length < 3)
            {
                return BadRequest("Cannot perform dog search with less than 3 characters");
            }
            try
            {
                IQueryable<Dogs> query = _dogService.GetDogsQueryStartsWith(searchText);
                var results = await query.ToListAsync();
                //string formattedResult = JsonConvert.SerializeObject(results, Formatting.Indented);
                ICollection<CoreDAL.Models.DTOs.ABKCDogDTO> dtos = _autoMapper.Map<ICollection<CoreDAL.Models.DTOs.ABKCDogDTO>>(results);
                if (dtos != null && dtos.Any())
                {
                    foreach (CoreDAL.Models.DTOs.ABKCDogDTO d in dtos)
                    {
                        //get the breed, color, and id from new table if it exists
                        BaseDogModel found = await _dogService.GetDogByOldTableId(d.OriginalTableId, false);
                        if (found != null)
                        {
                            d.BreedId = found.Breed != null ? found.Breed.Id : 0;
                            d.ColorId = found.Color != null ? found.Color.Id : 0;
                            d.Id = found.Id;
                        }
                    }
                }
                Response.Headers.Add("x-total-count", results.Count.ToString());
                return Ok(dtos ?? new List<CoreDAL.Models.DTOs.ABKCDogDTO>());
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet("GetAllDogsCount")]
        [ProducesResponseType(200, Type = typeof(int))]
        public async Task<int> GetDogCount() => await _dogService.GetAllDogsCount();

        #region Dog Searching and Lists

        [HttpPost("GetDogsForTable")]
        [ProducesResponseType(200, Type = typeof(ICollection<Dogs>))]
        public async Task<ActionResult<ICollection<Dogs>>> GetDogsForTableAsync(SieveModel searchParams)
        {
            try
            {
                var result = _dogService.GetDogsQuery(false); // Makes read-only queries faster
                result = _sieveProcessor.Apply(searchParams, result); // Returns `result` after applying the sort/filter/page query in `SieveModel` to it
                //get count without paging
                searchParams.Page = null;
                searchParams.PageSize = null;
                int count = await _sieveProcessor.Apply(searchParams, _dogService.GetDogsQuery(false)).CountAsync();
                ICollection<Dogs> tmp = await result.ToListAsync();
                Response.Headers.Add("x-total-count", count.ToString());
                return Ok(tmp);
            }
            catch (Exception x)
            {

                throw;
            }

        }

        [HttpPost("GetDogsCount")]
        [ProducesResponseType(200, Type = typeof(int))]
        public async Task<int> GetDogsCount(SieveModel searchParams)
        {
            //clear out params because we want it all that matches other criteria (filtering)!
            searchParams.Page = null;
            searchParams.PageSize = null;
            searchParams.Sorts = null;

            try
            {
                var result = _dogService.GetDogsQuery(false); // Makes read-only queries faster
                if (string.IsNullOrEmpty(searchParams.Filters))
                {
                    return await result.CountAsync();
                }
                result = _sieveProcessor.Apply(searchParams, result); // Returns `result` after applying the sort/filter/page query in `SieveModel` to it
                return await result.CountAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion


        [HttpGet("GetTrialDogData")]
        [Produces("application/json")]
        public async Task<ActionResult> GetTrialDogData()
        {
            var rtn = await _dogService.GetDogsQuery(false).Select(d => new
            {
                d.DogName,
                d.BullyId,
                d.AbkcNo,
                d.OwnerId,
                d.LastModified,
                d.Birthdate,
                //other show related data?
            }).ToListAsync();
            return Ok(rtn);
        }

    }
}