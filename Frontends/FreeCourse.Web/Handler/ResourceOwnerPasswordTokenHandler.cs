using FreeCourse.Web.Exception;
using FreeCourse.Web.Services.Interfaces;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using static IdentityModel.OidcConstants;

namespace FreeCourse.Web.Handler
{
    public class ResourceOwnerPasswordTokenHandler:DelegatingHandler
    {//burda login ve refresh durumunda giriş yapamazsak diye oluşturuyoruz otomatik olarak logine geçmesi için
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IIdentityService _ıdentityService;//refresh tokeni alıyoruz
        private readonly ILogger<ResourceOwnerPasswordTokenHandler> _logger;//buraya logluyoruz

        public ResourceOwnerPasswordTokenHandler(IHttpContextAccessor httpContextAccessor, IIdentityService ıdentityService
            , ILogger<ResourceOwnerPasswordTokenHandler> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _ıdentityService = ıdentityService;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {//accessTokeni cookiden okuduk
            var accessToken =await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken); 
            //requestine header ekledik
            request.Headers.Authorization=new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",accessToken);
            //gönderdik 401 gelirse elimizdeki refersh tokekn ile almaya çalışıyoruz alamzsak devamı
            var response=await base.SendAsync(request, cancellationToken);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var tokenResponse = await _ıdentityService.GetAccessTokenByRefreshToken();
                if (tokenResponse!=null)
                {
                    request.Headers.Authorization = new System.Net.Http.Headers
                        .AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
                    response = await base.SendAsync(request, cancellationToken);
                }
               
            }
            if(response.StatusCode==System.Net.HttpStatusCode.Unauthorized)
            {
                //hata tekrar login sayfasına gitmsi lazım ama hata yolluycaz
                throw new UnAuthorizeException();
            }
            return response;
            
        }

    }
}
