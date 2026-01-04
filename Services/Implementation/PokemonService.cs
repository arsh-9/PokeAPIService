using Microsoft.Extensions.Caching.Memory;
using PokeAPIService.Clients;
using PokeAPIService.Models;

namespace PokeAPIService.Services;

public class PokemonService : IPokemonService
{
    private readonly IPokeApiClient _client;
    private readonly IMemoryCache _cache;

    public PokemonService(IPokeApiClient client, IMemoryCache cache)
    {
        _client = client;
        _cache = cache;
    }

    public async Task<List<PokemonDto>> GetPokemonListAsync(int page, int pageSize)
    {
        var cacheKey = $"pokemon:list:page:{page}:size:{pageSize}";

        if (_cache.TryGetValue(cacheKey, out List<PokemonDto>? cached))
        {
            return cached;
        }
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

                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                    SlidingExpiration = TimeSpan.FromMinutes(2)
                };

                _cache.Set(cacheKey, pokemonListDto, cacheOptions);
            }
        }

        return pokemonListDto;
    }

    public async Task<PokemonSearchDto?> SearchPokemonAsync(string idOrName)
    {
        var details = await _client.GetPokemonDetailsAsync(idOrName);
        PokemonSearchDto? pokemonSearchDto = null;
        if (details is not null)
        {
            pokemonSearchDto = new PokemonSearchDto
            {
                Name = details.Name,
                Img_Url = details.Sprites.Front_default
            };
        }
        return pokemonSearchDto;
    }
}
