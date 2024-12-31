using FreeCourse.Web.Models;
using FreeCourse.Web.Services.Interfaces;
using IdentityModel.Client;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Net.Http;

namespace FreeCourse.Web.Services
{
    public class ClientCredentialTokenService : IClientCredentialTokenService
    {

        private readonly ServiceApiSettings _serviceApiSettings;//url leri almak için
        private readonly ClientSettings _clientSettings;//clientId-seccert almak için
        private readonly IMemoryCache _memoryCache;//memoride tutracak olay
        private readonly HttpClient _httpClient;//http işlemlerş yapmak için

        //ıOption interface üzeridne alacağız appsettingsdeki
        public ClientCredentialTokenService(IOptions<ServiceApiSettings> serviceApiSettings
            , IOptions<ClientSettings> clientSettings, IMemoryCache memoryCache, HttpClient httpClient)
        {
            _serviceApiSettings = serviceApiSettings.Value;
            _clientSettings = clientSettings.Value;
            _memoryCache = memoryCache;
            _httpClient = httpClient;
        }

        public async Task<string> GetToken()//tokeni memoryde tutacak ve delege de bundan tokeni alıp tıokenrequeste verecek
        {
            //böyle bir token varmı tutmuşmuyuz diye kontrol ediyoruz
            if (_memoryCache.TryGetValue("WebClientToken", out string currentToken))
            {
                return currentToken;
            }
            var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _serviceApiSettings.IdentityBaseUrl,
                Policy = new DiscoveryPolicy { RequireHttps = false }
            });

            if (disco.IsError)
            {
                throw disco.Exception;
            }

            // Token talep et
            var clientCredentialTokenRequest = new ClientCredentialsTokenRequest
            {
                ClientId = _clientSettings.WebClient.ClientId,
                ClientSecret = _clientSettings.WebClient.ClientSecret,
                Address = disco.TokenEndpoint
            };

            var newToken = await _httpClient.RequestClientCredentialsTokenAsync(clientCredentialTokenRequest);

            if (newToken.IsError)
            {
                throw newToken.Exception;
            }

            // Token'ı önbelleğe ekle
            _memoryCache.Set("WebClientToken", newToken.AccessToken, TimeSpan.FromSeconds(newToken.ExpiresIn));

            return newToken.AccessToken;
        }
    }
}
