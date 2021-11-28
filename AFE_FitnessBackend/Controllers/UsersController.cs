using AFE_FitnessBackend.Data;
using AFE_FitnessBackend.Models.Dtos;
using AFE_FitnessBackend.Models.Enums;
using AFE_FitnessBackend.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static BCrypt.Net.BCrypt;

namespace AFE_FitnessBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly AppSettings _appSettings;
        const int BcryptWorkfactor = 10;

        public UsersController(ApplicationDbContext context, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _appSettings = appSettings.Value;
        }

        /// <summary>
        /// You must login before you can use any other api call.
        /// </summary>
        /// <param name="login"></param>
        /// <returns>JWT token</returns>
        [HttpPost("login"), AllowAnonymous]
        public async Task<ActionResult<Token>> Login(Login login)
        {
            if (login != null)
            {
                if (login.Password == string.Empty)
                    return BadRequest(new { error = "Password is required" });
                if (login.Email == string.Empty)
                    return BadRequest(new { error = "Email address is required" });
                login.Email = login.Email.ToLowerInvariant();
                var user = await _context.Users.Where(u => u.Email == login.Email)
                    .FirstOrDefaultAsync().ConfigureAwait(false);

                if (user != null)
                {
                    var validPwd = Verify(login.Password, user.PwHash);
                    if (validPwd)
                    {
                        var jwt = GenerateToken(user.FirstName, user.UserId, user.AccountType);
                        var token = new Token() { Jwt = jwt };
                        return token;
                    }
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid login");
            return BadRequest(ModelState);
        }

        /// <summary>
        /// Use to change the password.
        /// </summary>
        /// <param name="newPassword"></param>
        /// <returns>Token</returns>
        [HttpPut("Password")]
        public async Task<ActionResult<Token>> ChangePassword(NewPassword newPassword)
        {
            if (newPassword == null)
            {
                ModelState.AddModelError(string.Empty, "Data missing");
                return BadRequest(ModelState);
            }
            newPassword.Email = newPassword.Email.ToLowerInvariant();
            var account = await _context.Users.Where(u => u.Email == newPassword.Email)
                .FirstOrDefaultAsync().ConfigureAwait(false);

            if (account == null)
            {
                ModelState.AddModelError("email", "Not found!");
                return BadRequest(ModelState);
            }
            if (account.AccountType == Role.Manager)
                return BadRequest("Cannot change password for managers");
            if (account.UserId <= 3)
                return BadRequest("Cannot change password for Superman or Superwoman");
            var validPwd = Verify(newPassword.OldPassword, account.PwHash);
            if (validPwd)
            {
                account.PwHash = HashPassword(newPassword.Password, BcryptWorkfactor);
                await _context.SaveChangesAsync().ConfigureAwait(false);
                return Ok();
            }
            else
            {
                ModelState.AddModelError("oldPassword", "No match");
                return BadRequest(ModelState);
            }
        }
        // GET: api/Users
        /// <summary>
        /// Get a let of all users
        /// </summary>
        /// <returns>All users</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var eUsers = await _context.Users.ToListAsync();
            var users = new List<User>();
            foreach (var eUser in eUsers)
                users.Add(Models.Dtos.User.FromEntityUser(eUser));
            return users;
        }

        // GET: api/Users/Clients
        /// <summary>
        /// Gets the list of clients for a personal trainer.
        /// </summary>
        /// <returns>A list of users</returns>
        [HttpGet("Clients")]
        public async Task<ActionResult<IEnumerable<User>>> GetClientsForTrainer()
        {
            var role = GetClaim("Role");
            if (role != Role.PersonalTrainer.ToString())
            {
                return BadRequest(new { error = "Only personal trainers can call this endpoint" });
            }
            var userId = long.Parse(GetClaim("UserId"));
            var eUsers = await _context.Users.Where(u => u.PersonalTrainerId == userId).ToListAsync();
            var users = new List<User>();
            foreach (var eUser in eUsers)
                users.Add(Models.Dtos.User.FromEntityUser(eUser));
            return users;
        }

        // GET: api/Users/Trainer
        /// <summary>
        /// Returns the personal trainer for the logged in user.
        /// </summary>
        /// <returns>Personal Trainer</returns>
        [HttpGet("Trainer")]
        public async Task<ActionResult<User>> GetTrainer()
        {
            var userId = long.Parse(GetClaim("UserId"));
            var eUser = await _context.Users.FindAsync(userId);

            if (eUser == null)
            {
                return NotFound("User not found");
            }

            if (eUser.PersonalTrainerId == null)
            {
                return BadRequest("Personal Trainer not set");
            }
            var trainer = await _context.Users.FindAsync(eUser.PersonalTrainerId);
            if (trainer == null)
            {
                return NotFound("Personal trainer not found");
            }
            var user = Models.Dtos.User.FromEntityUser(trainer);
            return user;
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(long id)
        {
            var eUser = await _context.Users.FindAsync(id);

            if (eUser == null)
            {
                return NotFound();
            }

            var user = Models.Dtos.User.FromEntityUser(eUser);
            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(long id, User user)
        {
            if (id != user.UserId)
            {
                return BadRequest(new { error = "id and UserId must match" });
            }

            var role = GetClaim("Role");
            var userId = long.Parse(GetClaim("UserId"));
            if (role == Role.Client.ToString())
            {
                if (userId != user.UserId)
                    return Unauthorized(new { error = "Clients are not allowed to change other users" });
                if (user.AccountType != Role.Client.ToString())
                    return Unauthorized(new { error = "Clients are not allowed to change their AccoutType" });
            }
            if (user.AccountType == Role.Manager.ToString())
                return BadRequest(new { error = "You are not allowed to create managers" });
            if (user.FirstName == string.Empty)
                return BadRequest(new { error = "First name is required" });
            if (user.LastName == string.Empty)
                return BadRequest(new { error = "Last name is required" });
            if (user.Email == string.Empty)
                return BadRequest(new { error = "Email address is required" });
            user.Email = user.Email.ToLowerInvariant();
            if (!IsValidEmail(user.Email))
                return BadRequest(new { error = "Not a valid email address" });
            if (user.AccountType == string.Empty)
                return BadRequest(new { error = "AccountType is required" });
            Object clientRole;
            var validRole = Enum.TryParse(typeof(Role), user.AccountType, out clientRole);
            if (!validRole)
                return BadRequest(new { error = "AccountType not valid" });

            var eUser = user.ToEntityUser();
            try
            {
                var old = await _context.Users.FindAsync(user.UserId);
                eUser.PwHash = old.PwHash;
                _context.Entry(eUser).State = EntityState.Modified;            
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Valid accountTypes are: Client and PersonalTrainer".
        /// The userId is set by the backend.
        /// The personalTrainerId is only used for clients.
        /// Valid accountTypes are PersonalTrainer and Client.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>user with id</returns>
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            var role = GetClaim("Role");
            if ( role == Role.Client.ToString())
            {
                return Unauthorized(new { error = "Clients are not allowed to create users" });
            }
            if (user.AccountType == Role.Manager.ToString())
                return BadRequest(new { error = "You are not allowed to create managers" });
            user.UserId = 0;
            if (user.FirstName == string.Empty)
                return BadRequest(new { error = "First name is required" });
            if (user.LastName == string.Empty)
                return BadRequest(new { error = "Last name is required" });
            if (user.Password == string.Empty)
                return BadRequest(new { error = "Password is required" });
            if (user.Email == string.Empty)
                return BadRequest(new { error = "Email address is required" });
            user.Email = user.Email.ToLowerInvariant();
            if (!IsValidEmail(user.Email))
                return BadRequest(new { error = "Not a valid email address" });
            if (user.AccountType == string.Empty)
                return BadRequest(new { error = "AccountType is required" });
            Object clientRole;
            var validRole = Enum.TryParse(typeof(Role), user.AccountType, out clientRole);
            if (!validRole)
                return BadRequest(new { error = "AccountType not valid" });
            if (EmailExists(user.Email))
                return BadRequest(new { error = "Email address all ready registered" });
            var eUser = user.ToEntityUser();
            if (user.PersonalTrainerId == 0)
                eUser.PersonalTrainerId = null;
            eUser.PwHash = HashPassword(user.Password, BcryptWorkfactor);
            _context.Users.Add(eUser);
            await _context.SaveChangesAsync();
            user.UserId = eUser.UserId;
            user.Password = string.Empty;

            return CreatedAtAction("GetUser", new { id = eUser.UserId }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(long id)
        {
            var role = GetClaim("Role");
            var callerId = long.Parse(GetClaim("UserId"));
            if (role == Role.Client.ToString())
            {
                return Unauthorized(new { error = "Clients are not allowed to delete users" });
            }
            var eUser = await _context.Users.FindAsync(id);
            if (eUser == null)
            {
                return NotFound();
            }
            if (role == Role.PersonalTrainer.ToString())
            {
                if (eUser.PersonalTrainerId == callerId)
                {
                    _context.Users.Remove(eUser);
                    await _context.SaveChangesAsync();
                    return NoContent();
                }
                else return Unauthorized("Personal trainers are only allowed to delete their own clients");
                
            }
            if (role == Role.Manager.ToString())
            {
                _context.Users.Remove(eUser);
                await _context.SaveChangesAsync();

                return NoContent();
            }

            return BadRequest("Role unknown");
        }

        private bool UserExists(long id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }

        private bool EmailExists(string email)
        {
            return _context.Users.Any(e => e.Email == email);
        }

        private string GenerateToken(string name, long userId, Role role)
        {
            var claims = new Claim[]
            {
                new Claim("Name", name),
                new Claim("Role", role.ToString()),
                new Claim("UserId", userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddDays(1)).ToUnixTimeSeconds().ToString()),
            };

            var key = Encoding.ASCII.GetBytes(_appSettings.SecretKey);
            var token = new JwtSecurityToken(
                 new JwtHeader(new SigningCredentials(
                      new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)),
                      new JwtPayload(claims));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GetClaim(string claimType)
        {
            return User.Claims.Where(c => c.Type == claimType)
                   .Select(c => c.Value).SingleOrDefault();
        }

        internal static bool IsValidEmail(string email)
        {
            if (email.Trim().EndsWith("."))
            {
                return false; // suggested by @TK-421
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
