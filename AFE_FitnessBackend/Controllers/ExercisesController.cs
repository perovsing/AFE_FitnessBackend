using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AFE_FitnessBackend.Data;
using AFE_FitnessBackend.Models.Entities;
using AFE_FitnessBackend.Models.Enums;
using Microsoft.AspNetCore.Authorization;

namespace AFE_FitnessBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExercisesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExercisesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/Exercises/Program/{ProgramId}
        /// <summary>
        /// Use this endpoint to add an exercise to an existing workout program.
        /// </summary>
        /// <param name="exerciseDto">exercise</param>
        /// <param name="programId">programId for the program the exercise is added to.</param>
        /// <returns>The created exercise</returns>
        [HttpPost("Program/{programId}")]
        public async Task<ActionResult<Exercise>> PostExercise(Models.Dtos.ExerciseDto exerciseDto, long programId)
        {
            var role = GetClaim("Role");
            if (role != Role.PersonalTrainer.ToString())
            {
                return BadRequest(new { error = "Only personal trainers can call this endpoint" });
            }
            var trainerId = long.Parse(GetClaim("UserId"));
            var eExercise = exerciseDto.ToEntityExercise();
            eExercise.PersonalTrainerId = trainerId;
            eExercise.WorkoutProgramId = programId;
            _context.Exercises.Add(eExercise);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetExercise", new { id = eExercise.ExerciseId }, eExercise);
        }

        // GET: api/Exercises
        /// <summary>
        /// Gets all exercises a trainer has in the system.
        /// </summary>
        /// <returns>A list of exercises.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Exercise>>> GetExercises()
        {
            var role = GetClaim("Role");
            if (role != Role.PersonalTrainer.ToString())
            {
                return BadRequest(new { error = "Only personal trainers can call this endpoint" });
            }
            var userId = long.Parse(GetClaim("UserId"));
            return await _context.Exercises.Where(e => e.PersonalTrainerId == userId).ToListAsync();
        }

        // GET: api/Exercises/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Exercise>> GetExercise(long id)
        {
            var exercise = await _context.Exercises.FindAsync(id);

            if (exercise == null)
            {
                return NotFound();
            }

            return exercise;
        }

        // PUT: api/Exercises/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExercise(long id, Exercise exercise)
        {
            var role = GetClaim("Role");
            if (role != Role.PersonalTrainer.ToString())
            {
                return BadRequest(new { error = "Only personal trainers can call this endpoint" });
            }
            if (id != exercise.ExerciseId)
            {
                return BadRequest();
            }

            _context.Entry(exercise).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExerciseExists(id))
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

        // POST: api/Exercises
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Exercise>> PostExercise(Exercise exercise)
        {
            var role = GetClaim("Role");
            if (role != Role.PersonalTrainer.ToString())
            {
                return BadRequest(new { error = "Only personal trainers can call this endpoint" });
            }
            _context.Exercises.Add(exercise);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetExercise", new { id = exercise.ExerciseId }, exercise);
        }

        // DELETE: api/Exercises/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExercise(long id)
        {
            var role = GetClaim("Role");
            if (role != Role.PersonalTrainer.ToString())
            {
                return BadRequest(new { error = "Only personal trainers can call this endpoint" });
            }
            var exercise = await _context.Exercises.FindAsync(id);
            if (exercise == null)
            {
                return NotFound();
            }

            _context.Exercises.Remove(exercise);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ExerciseExists(long id)
        {
            return _context.Exercises.Any(e => e.ExerciseId == id);
        }

        private string GetClaim(string claimType)
        {
            return User.Claims.Where(c => c.Type == claimType)
                   .Select(c => c.Value).SingleOrDefault();
        }
    }
}
