using Microsoft.Extensions.DependencyInjection;

namespace BsiPlaywrightPoc.Factory.Api
{
    public static class SingletonHttpClientFactory
    {
        private static IHttpClientFactory _instance;
        private static readonly object _lock = new object();

        public static IHttpClientFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            var serviceCollection = new ServiceCollection();
                            serviceCollection.AddHttpClient();
                            var serviceProvider = serviceCollection.BuildServiceProvider();
                            _instance = serviceProvider.GetRequiredService<IHttpClientFactory>();
                        }
                    }
                }
                return _instance;
            }
        }
    }
}
