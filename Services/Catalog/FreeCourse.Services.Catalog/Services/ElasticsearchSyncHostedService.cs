namespace FreeCourse.Services.Catalog.Services
{
    public class ElasticsearchSyncHostedService:IHostedService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ElasticsearchSyncHostedService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var courseService = scope.ServiceProvider.GetRequiredService<ICourseService>();
                await courseService.SyncCoursesToElasticsearchAsync();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
