using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; // Tilføj denne linje
using CatalogServiceAPI.Models;

namespace EnvironmentAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserInterface _userService;
        private readonly ILogger<UserController> _logger; // Tilføjet ILogger

        public UserController(IUserInterface userService, ILogger<UserController> logger) // Ændret her for at inkludere ILogger
        {
            _userService = userService;
            _logger = logger; // Tilføjet her for at gemme loggeren
        }

        [HttpGet("{bruger_id}")]
        public async Task<ActionResult<User>> GetUser(Guid bruger_id)
        {
            var user = await _userService.GetUser(bruger_id);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUserList()
        {
            var userList = await _userService.GetUserList();
            if (userList == null) { throw new ApplicationException("listen er null"); };
            return Ok(userList);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> AddUser(User user)
        {
            var userId = await _userService.AddUser(user);
            return CreatedAtAction(nameof(GetUser), new { user_id = userId }, userId);
        }

        [HttpPut("{user_id}")]
        public async Task<IActionResult> UpdateUser(Guid _id, User user)
        {
            if (_id != user._id)
            {
                return BadRequest();
            }

            var result = await _userService.UpdateUser(user);
            if (result == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{user_id}")]
        public async Task<IActionResult> DeleteUser(Guid user_id)
        {
            var result = await _userService.DeleteUser(user_id);
            if (result == 0)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
