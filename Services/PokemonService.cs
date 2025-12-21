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
            var detailTasks = pokemonListResponse.Results
            .Select(p => _client.GetPokemonDetailsByUrlAsync(p.Url));
            var detailsList = await Task.WhenAll(detailTasks);
            if (detailsList is not null)
            {
                pokemonListDto = detailsList.Select(detail => new PokemonDto
                {
                    Name = detail.Name,
                    Order = detail.Order,
                    Abilities = detail.Abilities
                .Select(a => a.ability.Name)
                .ToList(),
                    Type = detail.Types.First().type.Name
                }).ToList();
            }
        }

        return pokemonListDto;
    }
}
