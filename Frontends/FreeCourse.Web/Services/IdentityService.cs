using FreeCourse.Web.Models;
using FreeCourse.Web.Services.Interfaces;
using FreeCourses.Shared.Dtos;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Globalization;
using System.Security.Claims;
using System.Text.Json;

namespace FreeCourse.Web.Services
{
    public class IdentityService : IIdentityService
    {//NET uygulamalarında HTTP istekleri yapmak ve HTTP yanıtlarını almak için
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;//cooka erişebilmek için
        private readonly ClientSettings _clientSettings;//ClientId, ClientSecret alabilmek için
        private readonly ServiceApiSettings _serviceApiSettings;//BaseUrl, PhotoStockUrl alabilmek için

        public IdentityService(HttpClient client, IHttpContextAccessor httpContextAccessor, IOptions<ClientSettings> clientSettings, IOptions<ServiceApiSettings> serviceApiSettings)
        {
            _httpClient = client;
            _httpContextAccessor = httpContextAccessor;
            _clientSettings = clientSettings.Value;
            _serviceApiSettings = serviceApiSettings.Value;
        }

        public async Task<TokenResponse> GetAccessTokenByRefreshToken()
        {
            var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _serviceApiSettings.IdentityBaseUrl,
                Policy = new DiscoveryPolicy { RequireHttps = false }
            });
            if (disco.IsError)
            {
                throw disco.Exception;
            }
            //http contex üzerinden refresh tokeni alıyoruz
            var  refreshToken=await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            RefreshTokenRequest refreshTokenRequest = new()
            {
                ClientId = _clientSettings.WebClientForUser.ClientId,
                ClientSecret = _clientSettings.WebClientForUser.ClientSecret,
                RefreshToken = refreshToken,
                Address = disco.TokenEndpoint//buraya istek yapacak
            };
            var token=await _httpClient.RequestRefreshTokenAsync(refreshTokenRequest);
            if (token.IsError)
            {
                return null;
            }
           
            //tokeni tutmak istiyorsan ben senin için cooki içinde tutucam demek bu store methodu
            var authenticationTokens= new List<AuthenticationToken>()
            {
                new AuthenticationToken{Name=OpenIdConnectParameterNames.AccessToken,Value=token.AccessToken},
             new AuthenticationToken{Name=OpenIdConnectParameterNames.RefreshToken,Value=token.RefreshToken},
              new AuthenticationToken{Name=OpenIdConnectParameterNames.ExpiresIn,Value=
              DateTime.Now.AddSeconds(token.ExpiresIn).ToString("O",CultureInfo.InvariantCulture)}

            };
            //buradan bize prophertiler gelecek
            var authenticationResult = await _httpContextAccessor.HttpContext.AuthenticateAsync();
            var properties = authenticationResult.Properties;
            properties.StoreTokens(authenticationTokens);

            await _httpContextAccessor.HttpContext
                .SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,authenticationResult.Principal,properties);
            return token;

        }

        public async Task RevokeRefreshToken()
        {
            var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _serviceApiSettings.IdentityBaseUrl,
                Policy = new DiscoveryPolicy { RequireHttps = false }
            });
            if (disco.IsError)
            {
                throw disco.Exception;
            }
            var refreshToken= await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            TokenRevocationRequest tokenRevocationRequest = new()
            {
                ClientId=_clientSettings.WebClientForUser.ClientId,
                ClientSecret=_clientSettings.WebClientForUser.ClientSecret,
                Address=disco.RevocationEndpoint,
                Token=refreshToken,
                TokenTypeHint="refresh_token"
            };

            await _httpClient.RevokeTokenAsync(tokenRevocationRequest);
        }

        public async Task<Response<bool>> SignIn(SigninInput signinInput)
        {//tüm endpointler buraya gelecek httpsi engellemek için yaptık 
            var disco= await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address=_serviceApiSettings.IdentityBaseUrl,
                Policy=new DiscoveryPolicy { RequireHttps = false }
            });
            if (disco.IsError)
            {
                throw disco.Exception;
            }
            var passwordTokenRequest = new PasswordTokenRequest
            {
                ClientId = _clientSettings.WebClientForUser.ClientId,
                ClientSecret = _clientSettings.WebClientForUser.ClientSecret,
                UserName = signinInput.Email,
                Password = signinInput.Password,
                Address = disco.TokenEndpoint
            };
            var token=await _httpClient.RequestPasswordTokenAsync(passwordTokenRequest);
            if (token.IsError)
            {
                var responseContent=await token.HttpResponse.Content.ReadAsStringAsync();
                var errorDto=JsonSerializer.Deserialize<ErrorDto>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return Response<bool>.Fail(errorDto.Errors, 400);

            }
            var userInfoRequest = new UserInfoRequest
            {
                Token = token.AccessToken,
                Address = disco.UserInfoEndpoint,
            };
            var userInfo=await _httpClient.GetUserInfoAsync(userInfoRequest);
            if (userInfo.IsError)
            {
                throw userInfo.Exception;
            }

            ClaimsIdentity claimsIdentity = new ClaimsIdentity
                (userInfo.Claims,CookieAuthenticationDefaults.AuthenticationScheme,"name","role");
            //cookie artık bizim verdiğimiz nesne üzerinden oluşacak
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            //accestoken ve refresh tokeni tutuyor olacağız burada
            var authenticationProperties = new AuthenticationProperties();
            //tokeni tutmak istiyorsan ben senin için cooki içinde tutucam demek bu store methodu
            authenticationProperties.StoreTokens(new List<AuthenticationToken>()
            { new AuthenticationToken{Name=OpenIdConnectParameterNames.AccessToken,Value=token.AccessToken},
             new AuthenticationToken{Name=OpenIdConnectParameterNames.RefreshToken,Value=token.RefreshToken},
              new AuthenticationToken{Name=OpenIdConnectParameterNames.ExpiresIn,Value=
              DateTime.Now.AddSeconds(token.ExpiresIn).ToString("O",CultureInfo.InvariantCulture)}

            });
            //ömrü olacakmı diye kalıcı olacağını söyledik
            authenticationProperties.IsPersistent = signinInput.IsRemember;
            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme
                , claimsPrincipal, authenticationProperties);
            return Response<bool>.Success(200);
        }
       
    }
}
