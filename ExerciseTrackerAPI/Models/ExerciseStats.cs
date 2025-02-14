namespace ExerciseTrackerAPI.Models
{
    public class ExerciseStats
    {
        public int DaysSinceBeginning { get; set; }
        public TimeSpan TotalDuration { get; set; }
        public TimeSpan AverageDuration { get; set; }
        public int TotalExercises { get; set; }
        public int NumberOfDays { get; set; }
        public decimal TotalDistance { get; set; }
        public double AverageMph { get; set; }
    }
}