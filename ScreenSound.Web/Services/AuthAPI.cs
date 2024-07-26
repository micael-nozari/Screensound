using Microsoft.AspNetCore.Components.Authorization;
using ScreenSound.Web.Responses;
using System.Net.Http.Json;
using System.Security.Claims;

namespace ScreenSound.Web.Services
{
    public class AuthAPI(IHttpClientFactory factory) : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient = factory.CreateClient("API");
        private bool autenticado = false;

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            autenticado = false;
            var pessoa = new ClaimsPrincipal();
            var response = await _httpClient.GetAsync("auth/manage/info");
            if(response.IsSuccessStatusCode)
            {
                var info = await response.Content.ReadFromJsonAsync<InfoPessoaResponse>();
                if (info is not null)
                {
                    Claim[] dados =
                    [
                        new Claim(ClaimTypes.Name, info.email),
                        new Claim(ClaimTypes.Email, info.email),
                    ];

                    var identity = new ClaimsIdentity(dados, "Cookies");
                    pessoa = new ClaimsPrincipal(identity);
                    autenticado = true;
                }
            }

            return new AuthenticationState(pessoa);
        }

        public async Task<AuthResponse> LoginAsync(string email, string senha)
        {
            var response = await _httpClient.PostAsJsonAsync("auth/login?useCookies=true", new
            {
                email,
                password = senha
            });

            if (response.IsSuccessStatusCode)
            {
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                return new AuthResponse { Sucesso = true };
            }
            else
            {
                return new AuthResponse
                {
                    Sucesso = false,
                    Erros = ["Login ou senha inválidos"]
                };
            }
        }

        public async Task LogoutAsync()
        {
            await _httpClient.PostAsync("auth/logout", null);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task<bool> VerificaAutenticado()
        {
            await GetAuthenticationStateAsync();
            return autenticado;
        }

    }
}
