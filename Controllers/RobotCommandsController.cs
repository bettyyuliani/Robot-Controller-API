using Microsoft.AspNetCore.Mvc;
using robot_controller_api.Persistence;
using Microsoft.AspNetCore.Authorization;


namespace robot_controller_api.Controllers;

[ApiController]
[Route("api/robot-commands")]
public class RobotCommandsController : ControllerBase
{
    private readonly IRobotCommandDataAccess _robotCommandsRepo;
    public RobotCommandsController(IRobotCommandDataAccess robotCommandsRepo)
    {
        _robotCommandsRepo = robotCommandsRepo;
    }


    /// <summary>
    /// Gets all robot commands.
    /// </summary>
    /// <returns>All robot commands</returns>
    /// <remarks>
    /// Sample request:
    /// GET /api/robot-commands
    /// </remarks>
    /// <response code="200">Successful request</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [AllowAnonymous]
    [HttpGet()]
    public IEnumerable<RobotCommand> GetAllRobotCommands() => _robotCommandsRepo.GetRobotCommands();


    /// <summary>
    /// Gets all robot commands that move the robot.
    /// </summary>
    /// <returns>All move robot commands</returns>
    /// <remarks>
    /// Sample request:
    /// GET /api/robot-commands/move
    /// </remarks>
    /// <response code="200">Successful request</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Policy = "RegisteredOnly")]
    [HttpGet("move")]
    public IEnumerable<RobotCommand> GetMoveCommandsOnly() => _robotCommandsRepo.GetRobotCommands().Where(i => i.IsMoveCommand == true);

    /// <summary>
    /// Gets robot command based on the specified id.
    /// </summary>
    /// <param name="id">ID of robot command.</param>
    /// <returns>Robot command based on the specified id</returns>
    /// <remarks>
    /// Sample request:
    /// Sample request:
    /// GET /api/robot-commands/1
    /// </remarks>
    /// <response code="200">Successful request</response>
    /// <response code="404">If there is no robot command with the specified id</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = "UserOnly")]
    [HttpGet("{id}", Name = "GetRobotCommand")]
    public IActionResult GetRobotCommandById(int id) => !_robotCommandsRepo.CheckEntry(id) ? NotFound() : Ok(_robotCommandsRepo.GetRobotCommands().Where(i => i.Id == id));

    /// <summary>
    /// Creates a robot command.
    /// </summary>
    /// <param name="newCommand">A new robot command from the HTTP request.</param>
    /// <returns>A newly created robot command</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/robot-commands
    ///     {
    ///        "name": "DANCE",
    ///        "isMoveCommand": true,
    ///        "description": "Salsa on the Moon"
    ///     }
    ///
    /// </remarks>
    /// <response code="201">Returns the newly created robot command</response>
    /// <response code="400">If the robot command is null</response>
    /// <response code="409">If a robot command with the same name already exists.</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    //Authorize(Policy = "AdminOnly")]
    [HttpPost()]
    public IActionResult AddRobotCommand(RobotCommand newCommand)
    {
        if (newCommand == null) return BadRequest();
        else if (_robotCommandsRepo.CheckName(newCommand.Name))return Conflict();

        _robotCommandsRepo.InsertRobotCommands(newCommand);
        return CreatedAtRoute("GetRobotCommand", new { id = newCommand.Id }, newCommand);
    }

    /// <summary>
    /// Updates a robot command.
    /// </summary>
    /// <param name="id">ID of robot command to be updated</param>
    /// <param name="newCommand">A new robot command to replaced the old command.</param>
    /// <returns>Status Code 204</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT /api/robot-commands/1
    ///     {
    ///        "name": "WIGGLE",
    ///        "isMoveCommand": false,
    ///        "description": "Wiggling the robot"
    ///     }
    ///
    /// </remarks>
    /// <response code="204">Request has succeeded</response>
    /// <response code="404">If no robot command correspond to the provided ID</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id}")]
    public IActionResult UpdateRobotCommand(int id, RobotCommand updatedCommand)
    {
        if (!_robotCommandsRepo.CheckEntry(id)) return NotFound();
        _robotCommandsRepo.UpdateRobotCommands(id, updatedCommand);
        return NoContent();
    }

    /// <summary>
    /// Delete a robot command.
    /// </summary>
    /// <param name="id">ID of robot command to be deleted</param>
    /// <returns>Status Code 204</returns>
    /// <remarks>
    /// Sample request:
    /// DELETE /api/robot-commands/1
    /// </remarks>
    /// <response code="204">Request has succeeded</response>
    /// <response code="404">If no robot command correspond to the provided ID</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id}")]
    public IActionResult DeleteRobotCommand(int id)
    {
        if (!_robotCommandsRepo.CheckEntry(id)) return NotFound();
        _robotCommandsRepo.DeleteRobotCommands(id);
        return NoContent();
    }
}
