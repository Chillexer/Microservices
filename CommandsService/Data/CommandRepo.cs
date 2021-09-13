using CommandsService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandsService.Data
{
    public class CommandRepo : ICommandRepo
    {
        private readonly AppDbContext _context;

        public CommandRepo(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task CreateCommandAsync(int platformId, Command command)
        {
            if(command == null){
                throw new ArgumentNullException(nameof(command));
            }
            if(!await PlatformExistsAsync(platformId)){
                throw new ArgumentException(nameof(platformId));
            }

            command.PlatformId = platformId;
            await _context.Commands.AddAsync(command);
        }

        public async Task CreatePlatformAsync(Platform plat)
        {
            if(plat == null){
                throw new ArgumentNullException(nameof(plat));
            }
            await _context.Platforms.AddAsync(plat);
        }

        public async Task<bool> ExternalPlatformExistsAsync(int externalPlatformId)
        {
            return await _context.Platforms.AnyAsync(p => p.ExternalId == externalPlatformId);
        }

        public async Task<IEnumerable<Platform>> GetAllPlatformsAsync()
        {
            return await _context.Platforms.ToListAsync();
        }

        public async Task<Command> GetCommandAsync(int platformId, int commandId)
        {
            return await _context.Commands
                .Where(c => c.PlatformId == platformId && c.Id == commandId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Command>> GetCommandsForPlatformAsync(int platformId)
        {
            return await _context.Commands
                .Where(c => c.PlatformId == platformId)
                .OrderBy(c => c.Platform!.Name).ToListAsync();
        }

        public async Task<bool> PlatformExistsAsync(int platformId)
        {
            return await _context.Platforms.AnyAsync(p => p.Id == platformId);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
    }
}