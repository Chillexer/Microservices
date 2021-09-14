using CommandsService.Models;
using CommandsService.SyncDataServices.Grpc;

namespace CommandsService.Data
{
    public static class PrepDb
    {
        public async static void PrepPopulation(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();

                if(grpcClient == null)
                    throw new ArgumentNullException(nameof(grpcClient));

                var platforms = await grpcClient.ReturnAllPlatformsAsync();

                var repo = serviceScope.ServiceProvider.GetService<ICommandRepo>();

                if(repo == null)
                    throw new ArgumentNullException(nameof(repo));

                await SeedData(repo, platforms);
            }
        }

        private async static Task SeedData(ICommandRepo repo, IEnumerable<Platform> platforms)
        {
            Console.WriteLine("--> Seeding new platforms....");

            foreach(var plat in platforms)
            {
                if(!await repo.ExternalPlatformExistsAsync(plat.ExternalId))
                {
                    await repo.CreatePlatformAsync(plat);
                }
                await repo.SaveChangesAsync();
            }
        }
    }
}