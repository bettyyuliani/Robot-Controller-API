using Microsoft.AspNetCore.Mvc;
using robot_controller_api.Persistence;

namespace robot_controller_api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserDataAccess _userRepo;
    public UsersController(IUserDataAccess userRepo)
    {
        _userRepo = userRepo;
    }

    /// <summary>
    /// Gets all user.
    /// </summary>
    /// <returns>All user</returns>
    /// <remarks>
    /// Sample request:
    /// GET /api/user
    /// </remarks>
    /// <response code="200">Successful request</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet()]
    public IEnumerable<UserModel> GetAllUsers() => _userRepo.GetUsers();

    /// <summary>
    /// Gets all admin users
    /// </summary>
    /// <returns>All users whose role is an admin</returns>
    /// <remarks>
    /// Sample request:
    /// GET /api/user/admin
    /// </remarks>
    /// <returns>Status Code 204</returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet("admin")]
    public IEnumerable<UserModel> GetAllAdminUser() => _userRepo.GetAdminUsers();

    /// <summary>
    /// Gets user by ID
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// GET /api/user/id
    /// </remarks>
    /// <returns>Status Code 204</returns>
    [HttpGet("{id}", Name = "GetUser")]
    public IActionResult GetUserById(int id) => !_userRepo.CheckEntry(id) ? NotFound() : Ok(_userRepo.GetUsers().Where(i => i.Id == id));

    /// <summary>
    /// Creates a user.
    /// </summary>
    /// <param name="newUser">A new user from the HTTP request.</param>
    /// <returns>A newly created user</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/user
    ///     {
    ///        "firstname": "Betty",
    ///        "lastname": "Yuliani
    ///     }
    ///
    /// </remarks>
    /// <response code="201">Returns the newly created user</response>
    /// <response code="400">If the user is null</response>
    /// <response code="409">If a user with the same name already exists.</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost()]
    public IActionResult AddUser(UserModel newUser)
    {
        if (newUser == null) return BadRequest();
        else if (_userRepo.CheckEmail(newUser.Email)) return Conflict();
        _userRepo.InsertUser(newUser);
        return CreatedAtRoute("GetUser", new { id = newUser.Id }, newUser);
    }

    /// <summary>
    /// Updates a user.
    /// </summary>
    /// <param name="id">ID of user to be updated</param>
    /// <param name="newUser">A new user to replaced the old user.</param>
    /// <returns>Status Code 204</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT /api/user/1
    ///     {
    ///        "firstname": "updated",
    ///        "lasttname": "user",
    ///        "description": "Updated user"
    ///     }
    ///
    /// </remarks>
    /// <response code="204">Request has succeeded</response>
    /// <response code="404">If no user correspond to the provided ID</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("{id}")]
    public IActionResult UpdateUser(int id, UserModel updatedUser)
    {
        if (!_userRepo.CheckEntry(id)) return NotFound();

        _userRepo.UpdateUser(id, updatedUser);
        return NoContent();
    }

    /// <summary>
    /// Delete a user.
    /// </summary>
    /// <param name="id">ID of user to be deleted</param>
    /// <returns>Status Code 204</returns>
    /// <remarks>
    /// Sample request:
    /// DELETE /api/users/1
    /// </remarks>
    /// <response code="204">Request has succeeded</response>
    /// <response code="404">If no user correspond to the provided ID</response>
    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        if (!_userRepo.CheckEntry(id)) return NotFound();

        _userRepo.DeleteUser(id);
        return NoContent();
    }

    /// <summary>
    /// Updates/patches a user's email and password.
    /// </summary>
    /// <param name="id">ID of user to be updated</param>
    /// <param name="email">A new email to replace the old email.</param>
    /// <param name="password">A new password hash to replace the old password hash.</param>
    /// <returns>Status Code 204</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     PATCH /api/user/1
    ///     {
    ///        "email": blablabla@hotmail.com,
    ///        "password": "12345",
    ///     }
    ///
    /// </remarks>
    /// <response code="204">Request has succeeded</response>
    /// <response code="404">If no user correspond to the provided ID</response>
    /// <response code="409">If a user with the same email already exists.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPatch("{id}")]
    public IActionResult PatchUser(int id, LoginModel user)
    {
        if (!_userRepo.CheckEntry(id)) return NotFound();
        else if (_userRepo.CheckEmail(user.Email)) return Conflict();

        _userRepo.PatchUser(id, user);
        return NoContent();
    }
}
