
using FreeCourse.Web.Exception;
using FreeCourse.Web.Services.Interfaces;
using System.Net;
using System.Net.Http.Headers;

namespace FreeCourse.Web.Handler
{
    public class ClientCredentialTokenHandler: DelegatingHandler
    {
        IClientCredentialTokenService _clientCredentialTokenService;

        public ClientCredentialTokenHandler(IClientCredentialTokenService clientCredentialTokenService)
        {
            _clientCredentialTokenService = clientCredentialTokenService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await _clientCredentialTokenService.GetToken());
            var response=await base.SendAsync(request, cancellationToken);
            
            if(response.StatusCode==HttpStatusCode.Unauthorized)
            {
                throw new UnAuthorizeException();
            }
            return response;
        }
    }
}
