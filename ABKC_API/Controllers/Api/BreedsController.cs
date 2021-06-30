using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreDAL.Interfaces;
using CoreDAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreApp.Controllers.Api
{

    //[Authorize]
    [Route("/api/[controller]")]
    [ApiController]//dotnet 2.1 attribute
    public class BreedsController : BaseAuthorizedAPIController
    {
        private readonly IBreedService _breedService;
        private readonly IStyleAndClassService _styleAndClassService;

        public BreedsController(IBreedService breedService, IStyleAndClassService styleAndClassService)
        {
            _breedService = breedService;
            _styleAndClassService = styleAndClassService;
        }


        [HttpGet("GetBreeds")]
        [ProducesResponseType(200, Type = typeof(ICollection<Breeds>))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ICollection<Breeds>>> GetBreeds()
        {
            ICollection<Breeds> breeds = await _breedService.GetBreedsAsync();
            Response.Headers.Add("x-total-count", breeds.Count().ToString());
            return Ok(breeds);

        }

        [HttpGet("GetStyles")]
        [ProducesResponseType(200, Type = typeof(ICollection<Styles>))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ICollection<Styles>>> GetStyles()
        {
            ICollection<Styles> styles = await _styleAndClassService.GetStyles();
            Response.Headers.Add("x-total-count", styles.Count().ToString());
            return Ok(styles);
        }
        [HttpGet("GetClassTemplates")]
        [ProducesResponseType(200, Type = typeof(ICollection<ClassTemplates>))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ICollection<ClassTemplates>>> GetClassTemplates()
        {
            ICollection<ClassTemplates> classTemplates = await _styleAndClassService.GetClassTemplates();
            Response.Headers.Add("x-total-count", classTemplates.Count().ToString());
            return Ok(classTemplates);
        }
    }
}