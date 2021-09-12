using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers{


    [Route("api/platforms")]
    [ApiController]
    public class PlatformsController : ControllerBase{
        private readonly IPlatformRepo _repository;
        public IMapper _mapper { get; }
        private readonly ICommandDataClient _commandDataClient;

        public PlatformsController(IPlatformRepo repository, IMapper mapper, ICommandDataClient commandDataClient)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _commandDataClient = commandDataClient ?? throw new ArgumentNullException(nameof(commandDataClient));
        }


        [HttpGet(Name = nameof(GetPlatformsAsync))]
        public async Task<ActionResult<IEnumerable<PlatformReadDto>>> GetPlatformsAsync(){
            Console.WriteLine("--> Getting Platforms....");

            var platformItems = await _repository.GetAllPlatformsAsync();

            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
        }

        [HttpGet("{id:int}", Name = nameof(GetPlatformByIdAsync))]
        public async Task<ActionResult<PlatformReadDto>> GetPlatformByIdAsync(int id){
            Console.WriteLine($"--> Getting Platform {id}....");

            var platformItem = await _repository.GetPlatformByIdAsync(id);

            if(platformItem != null)
                return Ok(_mapper.Map<PlatformReadDto>(platformItem));
            
            return NotFound();
        }

        [HttpPost(Name = nameof(CreatePlatformAsync))]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatformAsync(PlatformCreateDto platformCreateDto){
            if(!ModelState.IsValid)
                return BadRequest();

            var platformModel = _mapper.Map<Platform>(platformCreateDto);

            await _repository.CreatePlatformAsync(platformModel);
            await _repository.SaveChangesAsync();

            var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

            try
            {
                 await _commandDataClient.SendPlatformToCommandAsync(platformReadDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not send synchronously: {ex.Message}");
            }

            return CreatedAtRoute(nameof(GetPlatformByIdAsync), new {Id = platformReadDto.Id}, platformReadDto);
        }
    }
}