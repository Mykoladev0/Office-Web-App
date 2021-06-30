using System.Threading.Tasks;
using CoreDAL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CoreApp.Controllers.Api
{


    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json", "application/json-patch+json", "multipart/form-data")]
    [ApiController]
    public class MaintenanceController : BaseAuthorizedAPIControllerWithUser
    {
        private readonly IDogRegistrationService _dogRegService;
        private readonly IOwnerService _ownerService;
        private readonly IGeneralRegistrationService _genRegService;

        public MaintenanceController(IABKCUserService userService, IDogRegistrationService dogRegService, IGeneralRegistrationService genRegService, IOwnerService ownerService) : base(userService)
        {
            _dogRegService = dogRegService;
            _ownerService = ownerService;
            _genRegService = genRegService;
        }


        [HttpPost("SeedData")]
        public async Task<ActionResult<int>> SeedData()
        {
            //to seed data
            CoreDAL.SeedData.DogRegistrationSeed seeding = new CoreDAL.SeedData.DogRegistrationSeed();
            int count = await seeding.SeedRegistrations(_genRegService, _ownerService, _userService, _dogRegService);
            return Ok(count);
        }
    }
}