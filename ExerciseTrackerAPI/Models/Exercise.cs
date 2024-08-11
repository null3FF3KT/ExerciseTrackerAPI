using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExerciseTrackerAPI.Models
{
    public class Exercise
    {
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public TimeSpan Duration { get; set; }

        [Required]
        public int ExerciseTypeId { get; set; }

        [ForeignKey("ExerciseTypeId")]
        public ExerciseType? ExerciseType { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Distance { get; set; }
    }
}