namespace BoardgameReportsService.Clients
{
    class AuthenticationClient
    {
        private readonly HttpClient _httpClient;

        public AuthenticationClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<bool> Register(RegisterInput input)
        {
            var responseMessage = await _httpClient.PostAsJsonAsync<RegisterInput>("/register", input);
            return responseMessage.IsSuccessStatusCode;
        }
        public async Task<string> Login(LoginCredentials loginCredentials)
        {
            var responseMessage = await _httpClient.PostAsJsonAsync<LoginCredentials>("/login", loginCredentials);
            if (responseMessage.IsSuccessStatusCode)
            {
                return await responseMessage.Content.ReadFromJsonAsync<string>();
            }
            return null;
        }
    }
}