
using PokeAPIService.Models;

namespace PokeAPIService.Clients;
public class PokeApiClient
{
    private readonly HttpClient _httpClient;

    public PokeApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PokemonListResponse?> GetPokemonListAsync(int limit, int offset)
    {
        return await _httpClient.GetFromJsonAsync<PokemonListResponse>(
            $"pokemon?limit={limit}&offset={offset}"
        );
    }

    public async Task<PokemonDetails?> GetPokemonDetailsByUrlAsync(string url)
    {
        return await _httpClient.GetFromJsonAsync<PokemonDetails>(url);
    }

    public async Task<PokemonDetails?> GetPokemonDetailsAsync(string idOrName)
    {
        return await _httpClient.GetFromJsonAsync<PokemonDetails>(
            $"pokemon/{idOrName}"
        );
    }
}
