using AFE_FitnessBackend.Data;
using AFE_FitnessBackend.Models.Entities;
using AFE_FitnessBackend.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AFE_FitnessBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WorkoutProgramsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WorkoutProgramsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/WorkoutPrograms
        [HttpGet("trainer")]
        public async Task<ActionResult<IEnumerable<WorkoutProgram>>> GetWorkoutProgramsForTrainer()
        {
            var role = GetClaim("Role");
            if (role != Role.PersonalTrainer.ToString())
            {
                ModelState.AddModelError(string.Empty, "Only personal trainers can call this endpoint.");
                return BadRequest(ModelState);
            }
            long trainerId = long.Parse(GetClaim("UserId"));
            return await _context.WorkoutPrograms.Where(w => w.PersonalTrainerId == trainerId).Include(w => w.Exercises).ToListAsync();
        }

        [HttpGet("client/{id}")]
        public async Task<ActionResult<IEnumerable<WorkoutProgram>>> GetWorkoutProgramsForClient(long id)
        {
            return await _context.WorkoutPrograms.Where(w => w.ClientId == id).Include(w => w.Exercises).ToListAsync();
        }

        // GET: api/WorkoutPrograms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkoutProgram>>> GetWorkoutPrograms()
        {
            var role = GetClaim("Role");
            if (role != Role.PersonalTrainer.ToString())
            {
                ModelState.AddModelError(string.Empty, "Only personal trainers can call this endpoint.");
                return BadRequest(ModelState);
            }
            long trainerId = long.Parse(GetClaim("UserId"));
            return await _context.WorkoutPrograms.Where(w => w.PersonalTrainerId == trainerId).Include(w => w.Exercises).ToListAsync();
        }

        // GET: api/WorkoutPrograms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WorkoutProgram>> GetWorkoutProgram(long id)
        {
            var workoutProgram = await _context.WorkoutPrograms.FindAsync(id);

            if (workoutProgram == null)
            {
                return NotFound();
            }

            return workoutProgram;
        }

        // PUT: api/WorkoutPrograms/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWorkoutProgram(long id, WorkoutProgram workoutProgram)
        {
            if (id != workoutProgram.WorkoutProgramId)
            {
                return BadRequest();
            }

            _context.Entry(workoutProgram).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkoutProgramExists(id))
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

        // POST: api/WorkoutPrograms
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<WorkoutProgram>> PostWorkoutProgram(WorkoutProgram workoutProgram)
        {
            var role = GetClaim("Role");
            if (role != Role.PersonalTrainer.ToString())
            {
                ModelState.AddModelError(string.Empty, "Only personal trainers can call this endpoint.");
                return BadRequest(ModelState);
            }
            _context.WorkoutPrograms.Add(workoutProgram);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWorkoutProgram", new { id = workoutProgram.WorkoutProgramId }, workoutProgram);
        }

        // DELETE: api/WorkoutPrograms/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkoutProgram(long id)
        {
            var role = GetClaim("Role");
            if (role != Role.PersonalTrainer.ToString())
            {
                ModelState.AddModelError(string.Empty, "Only personal trainers can call this endpoint.");
                return BadRequest(ModelState);
            }
            var workoutProgram = await _context.WorkoutPrograms.FindAsync(id);
            if (workoutProgram == null)
            {
                return NotFound();
            }

            _context.WorkoutPrograms.Remove(workoutProgram);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool WorkoutProgramExists(long id)
        {
            return _context.WorkoutPrograms.Any(e => e.WorkoutProgramId == id);
        }

        private string GetClaim(string claimType)
        {
            return User.Claims.Where(c => c.Type == claimType)
                   .Select(c => c.Value).SingleOrDefault();
        }
    }
}
