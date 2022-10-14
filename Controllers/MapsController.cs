using Microsoft.AspNetCore.Mvc;
using robot_controller_api.Persistence;
using Microsoft.AspNetCore.Authorization;

namespace robot_controller_api.Controllers;

[ApiController]
[Route("api/maps")]
public class MapsController : ControllerBase
{
    private readonly IMapDataAccess _mapsRepo;
    public MapsController(IMapDataAccess mapsRepo)
    {
        _mapsRepo = mapsRepo;
    }

    /// <summary>
    /// Gets all maps.
    /// </summary>
    /// <returns>All Maps</returns>
    /// <remarks>
    /// Sample request:
    /// GET /api/maps
    /// </remarks>
    /// <response code="200">Successful request</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet()]
    [AllowAnonymous]
    public IEnumerable<Map> GetAllMaps() => _mapsRepo.GetMaps();

    /// <summary>
    /// Gets all maps of square size.
    /// </summary>
    /// <returns>All maps of square size</returns>
    /// <remarks>
    /// Sample request:
    /// GET /api/maps/square
    /// </remarks>
    /// <returns>Status Code 204</returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet("square")]
    [Authorize(Policy = "RegisteredOnly")]
    public IEnumerable<Map> GetAllSquareMaps() => _mapsRepo.GetSquareMaps();

    [HttpGet("{id}", Name = "GetMap")]
    public IActionResult GetMapById(int id) => !_mapsRepo.CheckEntry(id) ? NotFound() : Ok(_mapsRepo.GetMaps().Where(i => i.Id == id));

    /// <summary>
    /// Creates a map.
    /// </summary>
    /// <param name="newMap">A new map from the HTTP request.</param>
    /// <returns>A newly created map</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/maps
    ///     {
    ///        "name": "DANCE",
    ///        "isMoveCommand": true,
    ///        "description": "Salsa on the Moon"
    ///     }
    ///
    /// </remarks>
    /// <response code="201">Returns the newly created map</response>
    /// <response code="400">If the map is null</response>
    /// <response code="409">If a map with the same name already exists.</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost()]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult AddMap(Map newMap)
    {
        if (newMap == null) return BadRequest();
        else if (_mapsRepo.CheckName(newMap.Name)) return Conflict();

        _mapsRepo.InsertMap(newMap);
        return CreatedAtRoute("GetMap", new { id = newMap.Id }, newMap);
    }

    /// <summary>
    /// Updates a map.
    /// </summary>
    /// <param name="id">ID of map to be updated</param>
    /// <param name="newCommand">A new map to replaced the old map.</param>
    /// <returns>Status Code 204</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT /api/map/1
    ///     {
    ///        "name": "UPDATED_ROBOT_MAP",
    ///        "rows": 30,
    ///        "columns":30,
    ///        "description": "Updated robot map"
    ///     }
    ///
    /// </remarks>
    /// <response code="204">Request has succeeded</response>
    /// <response code="404">If no map correspond to the provided ID</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult UpdateMap(int id, Map updatedMap)
    {
        if (!_mapsRepo.CheckEntry(id)) return NotFound();

        _mapsRepo.UpdateMap(id, updatedMap);
        return NoContent();
    }

    /// <summary>
    /// Delete a map.
    /// </summary>
    /// <param name="id">ID of map to be deleted</param>
    /// <returns>Status Code 204</returns>
    /// <remarks>
    /// Sample request:
    /// DELETE /api/mapss/1
    /// </remarks>
    /// <response code="204">Request has succeeded</response>
    /// <response code="404">If no map correspond to the provided ID</response>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult DeleteMap(int id)
    {
        if (!_mapsRepo.CheckEntry(id)) return NotFound();

        _mapsRepo.DeleteMap(id);
        return NoContent();
    }

    /// <summary>
    /// Checks if coordinate exists in the map.
    /// </summary>
    /// <param name="id">ID of the map to check for.</param>
    /// <param name="x">x coordinate whose existence will be checked on the map.</param>
    /// <param name="y">y coordinate whose existence will be checked on the map.</param>
    /// <returns>True if the coordinate exists, false otherwise</returns>
    /// <remarks>
    /// Sample request:
    /// GET /api/maps/1/25-25
    /// </remarks>
    /// <response code="200">Request is successful and response is returned</response>
    /// <response code="400">If the coordinate is on the negative quarters</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Policy = "UserOnly")]
    [HttpGet("{id}/{x}-{y}")]
    public IActionResult CheckCoordinate(int id, int x, int y)
    {
        if (x < 0 || y < 0) return BadRequest();
        bool isOnMap = _mapsRepo.IsOnMap(id, x, y);
        return Ok(isOnMap);
    }
}
