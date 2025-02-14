using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExerciseTrackerAPI.Data;
using ExerciseTrackerAPI.Models;
using Microsoft.AspNetCore.Cors;

namespace ExerciseTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigin")]
    public class ExercisesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ExercisesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Exercises
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Exercise>>> GetExercises()
        {
            return await _context.Exercises.Include(e => e.ExerciseType).ToListAsync();
        }

        // GET: api/Exercises/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Exercise>> GetExercise(int id)
        {
            var exercise = await _context.Exercises.FindAsync(id);

            if (exercise == null)
            {
                return NotFound();
            }

            return exercise;
        }

        // POST: api/Exercises
        [HttpPost]
        public async Task<ActionResult<Exercise>> PostExercise(Exercise exercise)
        {
            _context.Exercises.Add(exercise);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetExercise), new { id = exercise.Id }, exercise);
        }

        // PUT: api/Exercises/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExercise(int id, Exercise exercise)
        {
            if (id != exercise.Id)
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

        // DELETE: api/Exercises/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExercise(int id)
        {
            var exercise = await _context.Exercises.FindAsync(id);
            if (exercise == null)
            {
                return NotFound();
            }

            _context.Exercises.Remove(exercise);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ExerciseExists(int id)
        {
            return _context.Exercises.Any(e => e.Id == id);
        }

        [HttpGet("stats")]
        public async Task<ActionResult<ExerciseStats>> GetExerciseStats()
        {
            var exercises = await _context.Exercises.ToListAsync();

            if (!exercises.Any())
            {
                return NotFound("No exercises found in the database.");
            }

            try
            {
                var stats = new ExerciseStats
                {
                    DaysSinceBeginning = (DateTime.Now - exercises.Min(e => e.Date)).Days + 1,
                    TotalDuration = TimeSpan.FromSeconds(exercises.Sum(e => e.Duration.TotalSeconds)),
                    AverageDuration = TimeSpan.FromSeconds(exercises.Average(e => e.Duration.TotalSeconds)),
                    TotalExercises = exercises.Count,
                    NumberOfDays = exercises.Select(e => e.Date.Date).Distinct().Count(),
                    TotalDistance = exercises.Sum(e => e.Distance ?? 0),
                    AverageMph = exercises.Where(e => e.Distance.HasValue && e.Duration.TotalHours > 0)
                                          .Average(e => (double)(e.Distance.Value / (decimal)e.Duration.TotalHours))
                };


                var result = Ok(stats);
                return result;
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while calculating exercise statistics.");
            }
        }

        [HttpGet("fastest")]
        public async Task<ActionResult<IEnumerable<FastestExercise>>> GetFastestExercises()
        {
            var exercises = await _context.Exercises.Where(e => e.Distance.HasValue).ToListAsync();

            if (!exercises.Any())
            {
                return NotFound("No exercises with distance found in the database.");
            }

            try
            {
                var fastestExercises = exercises
                    .GroupBy(e => decimal.Round(e.Distance.Value / 0.05m) * 0.05m)
                    .Select(g => new FastestExercise
                    {
                        DistanceGroup = g.Key,
                        FastestTime = g.OrderBy(e => e.Duration).First().Duration,
                        DateAchieved = g.OrderBy(e => e.Duration).First().Date,
                        ActualDistance = g.OrderBy(e => e.Duration).First().Distance.Value
                    })
                    .OrderBy(f => f.DistanceGroup)
                    .ToList();

                

                var result = Ok(fastestExercises);
                return result;
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while calculating fastest exercises.");
            }
        }
    }
}