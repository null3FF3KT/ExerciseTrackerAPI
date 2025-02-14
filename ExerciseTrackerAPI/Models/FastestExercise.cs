namespace ExerciseTrackerAPI.Models
{
    public class FastestExercise
    {
        public decimal DistanceGroup { get; set; }
        public TimeSpan FastestTime { get; set; }
        public DateTime DateAchieved { get; set; }
        public decimal ActualDistance { get; set; }
    }
}