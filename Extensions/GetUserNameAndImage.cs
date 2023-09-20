using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using LMS.Models.ViewModel;
using System.Security.Claims;

namespace LMS.Extensions
{
    public class GetUserNameAndImage
    {
        private readonly IHttpClientFactory _clientFactory;

        public GetUserNameAndImage(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<UserDetailsDto> GetUserDetailsAndImageAsync(ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }

            try
            {
                using (var httpClient = _clientFactory.CreateClient())
                {
                    var response = await httpClient.GetAsync($"https://imam.academy/JWTauthentication/GetUserById/{userId}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var userDetails = JsonConvert.DeserializeObject<UserDetailsDto>(content);

                        return userDetails;
                    }
                    else
                    {
                        throw new ApplicationException($"Error fetching user details. StatusCode: {response.StatusCode}");
                    }
                }
            }
            catch (ApplicationException ex)
            {
                // Handle the exception as needed
                return null;
            }
        }
    }
}
