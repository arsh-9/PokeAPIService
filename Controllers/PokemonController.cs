using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokeAPIService.Services;

namespace PokeAPIService.Controllers;

[ApiController]
[Route("api/pokemon")]
public class PokemonController : ControllerBase
{
    private readonly IPokemonService _service;

    public PokemonController(IPokemonService service)
    {
        _service = service;
    }

    [Authorize]
    [HttpGet("list")]
    public async Task<IActionResult> GetList([FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
    {
        if (pageSize > 10)
            return BadRequest("pageSize cannot be greater than 10");

        if (page < 1)
            return BadRequest("page must be greater than 0");
        var result = await _service.GetPokemonListAsync(page, pageSize);
        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string idOrName)
    {
        if (string.IsNullOrWhiteSpace(idOrName))
            return BadRequest("idOrName is required");

        var result = await _service.SearchPokemonAsync(idOrName);
        return Ok(result);
    }
}

