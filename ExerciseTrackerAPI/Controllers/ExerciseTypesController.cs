using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExerciseTrackerAPI.Data;
using ExerciseTrackerAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExerciseTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExerciseTypesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ExerciseTypesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ExerciseTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExerciseType>>> GetExerciseTypes()
        {
            return await _context.ExerciseTypes.ToListAsync();
        }

        // GET: api/ExerciseTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExerciseType>> GetExerciseType(int id)
        {
            var exerciseType = await _context.ExerciseTypes.FindAsync(id);

            if (exerciseType == null)
            {
                return NotFound();
            }

            return exerciseType;
        }

        // POST: api/ExerciseTypes
        [HttpPost]
        public async Task<ActionResult<ExerciseType>> PostExerciseType(ExerciseType exerciseType)
        {
            _context.ExerciseTypes.Add(exerciseType);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetExerciseType), new { id = exerciseType.Id }, exerciseType);
        }

        // PUT: api/ExerciseTypes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExerciseType(int id, ExerciseType exerciseType)
        {
            if (id != exerciseType.Id)
            {
                return BadRequest();
            }

            _context.Entry(exerciseType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExerciseTypeExists(id))
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

        // DELETE: api/ExerciseTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExerciseType(int id)
        {
            var exerciseType = await _context.ExerciseTypes.FindAsync(id);
            if (exerciseType == null)
            {
                return NotFound();
            }

            _context.ExerciseTypes.Remove(exerciseType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ExerciseTypeExists(int id)
        {
            return _context.ExerciseTypes.Any(e => e.Id == id);
        }
    }
}