using CommandsService.Models;

namespace CommandsService.SyncDataServices.Grpc
{
    public interface IPlatformDataClient
    {
        Task<IEnumerable<Platform>> ReturnAllPlatformsAsync();
    }
}