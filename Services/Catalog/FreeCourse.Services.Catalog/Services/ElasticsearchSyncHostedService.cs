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
                var categoryService = scope.ServiceProvider.GetRequiredService<ICategoryService>();

                try
                {
                    await categoryService.SyncCategoriesToElasticsearchAsync();
                    await courseService.SyncCoursesToElasticsearchAsync();
                    Console.WriteLine("✅ Elasticsearch senkronizasyonu tamamlandı.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Elasticsearch senkronizasyon hatası: {ex.Message}");
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
