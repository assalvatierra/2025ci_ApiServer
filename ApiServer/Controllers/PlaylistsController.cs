using ApiServer.Mongo;
using Microsoft.AspNetCore.Mvc;

namespace ApiServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlaylistsController : ControllerBase
{
    private readonly MongodbService _mongoService;

    public PlaylistsController(MongodbService mongoService)
    {
        _mongoService = mongoService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var playlists = await _mongoService.GetAsync();
        return Ok(playlists);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var playlist = await _mongoService.GetByIdAsync(id);
        if (playlist == null) return NotFound();
        return Ok(playlist);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Playlist playlist)
    {
        await _mongoService.CreateAsync(playlist);
        return CreatedAtAction(nameof(GetById), new { id = playlist.Id }, playlist);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] Playlist playlist)
    {
        var existing = await _mongoService.GetByIdAsync(id);
        if (existing == null) return NotFound();
        playlist.Id = existing.Id; // ensure id remains the same
        await _mongoService.UpdateAsync(id, playlist);
        return NoContent();
    }

    [HttpPost("{id}/add")]
    public async Task<IActionResult> AddToPlaylist(string id, [FromBody] string movieId)
    {
        var existing = await _mongoService.GetByIdAsync(id);
        if (existing == null) return NotFound();
        await _mongoService.AddToPlaylistAsync(id, movieId);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var existing = await _mongoService.GetByIdAsync(id);
        if (existing == null) return NotFound();
        await _mongoService.DeleteAsync(id);
        return NoContent();
    }
}
