using PokeAPIService.Clients;
using PokeAPIService.Models;

namespace PokeAPIService.Services;

public class PokemonService : IPokemonService
{
    private readonly PokeApiClient _client;

    public PokemonService(PokeApiClient client)
    {
        _client = client;
    }

    public async Task<List<PokemonDto>> GetPokemonListAsync(int page, int pageSize)
    {
        var offset = (page - 1) * pageSize;
        var pokemonListResponse = await _client.GetPokemonListAsync(pageSize, offset);
        List<PokemonDto> pokemonListDto = [];
        if (pokemonListResponse is not null)
        {
            pokemonListDto = new List<PokemonDto>();
            foreach (var pokemon in pokemonListResponse.Results)
            {
                var details = await _client.GetPokemonDetailsByUrlAsync(pokemon.Url);
                if (details is not null)
                {
                    pokemonListDto.Add(new PokemonDto
                    {
                        Name = details.Name,
                        Order = details.Order,
                        Abilities = details.Abilities
                            .Select(a => a.ability.Name)
                            .ToList(),
                        Type = details.Types.First().type.Name
                    });
                }
            }
        }

        return pokemonListDto;
    }
}
