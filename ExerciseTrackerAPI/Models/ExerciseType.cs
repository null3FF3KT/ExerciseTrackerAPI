using System.ComponentModel.DataAnnotations;

namespace ExerciseTrackerAPI.Models
{
    public class ExerciseType
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(15)]
        public string TypeName { get; set; } = string.Empty;
    }
}