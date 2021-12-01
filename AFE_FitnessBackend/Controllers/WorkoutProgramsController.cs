using AFE_FitnessBackend.Data;
using AFE_FitnessBackend.Models.Entities;
using AFE_FitnessBackend.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
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
        /// <summary>
        /// Returns all programs posted by the personalTrainer that is sending 
        /// the request.
        /// </summary>
        /// <returns>An array of WorkoutPrograms</returns>
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
        /// <summary>
        /// Returns the workout programs for the specified client.
        /// </summary>
        /// <param name="id">Client id</param>
        /// <returns>An array of WorkoutPrograms</returns>
        [HttpGet("client/{id}")]
        public async Task<ActionResult<IEnumerable<WorkoutProgram>>> GetWorkoutProgramsForClient(long id)
        {
            return await _context.WorkoutPrograms.Where(w => w.ClientId == id).Include(w => w.Exercises).ToListAsync();
        }

        // GET: api/WorkoutPrograms
        /// <summary>
        /// Gets all workoutPrograms associated with the current user. For the manager
        /// all workoutPrograms will be returned.
        /// </summary>
        /// <returns>An array of WorkoutPrograms</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkoutProgram>>> GetWorkoutPrograms()
        {
            var role = GetClaim("Role");
            if (role == Role.Manager.ToString())
            {
                return await _context.WorkoutPrograms.Include(w => w.Exercises).ToListAsync();
            }
            if (role == Role.PersonalTrainer.ToString())
            {
                long trainerId = long.Parse(GetClaim("UserId"));
                return await _context.WorkoutPrograms.Where(w => w.PersonalTrainerId == trainerId).Include(w => w.Exercises).ToListAsync();
            }
            if (role == Role.Client.ToString())
            {
                long userId = long.Parse(GetClaim("UserId"));
                return await _context.WorkoutPrograms.Where(w => w.ClientId == userId).Include(w => w.Exercises).ToListAsync();
            }
            return BadRequest("Role unknown");
        }

        // GET: api/WorkoutPrograms/5
        /// <summary>
        /// Returns the specified program with exercises.
        /// </summary>
        /// <param name="id">WorkoutProgramId</param>
        /// <returns>Workout Program with exercises</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<WorkoutProgram>> GetWorkoutProgram(long id)
        {
            var workoutProgram = await _context.WorkoutPrograms.Where(w => w.WorkoutProgramId == id).Include(w => w.Exercises).FirstOrDefaultAsync();

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
            var userId = long.Parse(GetClaim("UserId"));
            workoutProgram.PersonalTrainerId = userId;
            foreach (var ex in workoutProgram.Exercises)
            {
                ex.PersonalTrainerId = userId;
            }
            _context.WorkoutPrograms.Add(workoutProgram);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWorkoutProgram", new { id = workoutProgram.WorkoutProgramId }, workoutProgram);
        }

        // DELETE: api/WorkoutPrograms/5
        /// <summary>
        /// Deletes a workout program and all it's associated exercises.
        /// </summary>
        /// <param name="id">WorkoutProgramId</param>
        /// <returns>No content</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkoutProgram(long id)
        {
            var role = GetClaim("Role");
            if (role != Role.PersonalTrainer.ToString())
            {
                ModelState.AddModelError(string.Empty, "Only personal trainers can call this endpoint.");
                return BadRequest(ModelState);
            }
            var workoutProgram = await _context.WorkoutPrograms.Include(w => w.Exercises).Where(w => w.WorkoutProgramId == id).FirstOrDefaultAsync();
            if (workoutProgram == null)
            {
                return NotFound();
            }
            try { 
            _context.WorkoutPrograms.Remove(workoutProgram);
            await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return NoContent();
        }

        //[HttpPost("exercise/{id}")]
        //public async Task<IActionResult> PostWorkoutProgramExercise(long id, Exercise exercise)
        //{
        //    if (!WorkoutProgramExists(id)
        //    {
        //        return BadRequest("WorkoutProgramId");
        //    }

        //    _context.Entry(workoutProgram).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!WorkoutProgramExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}
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
