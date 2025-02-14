namespace ExerciseTrackerAPI.Models
{
    public static class PredefinedExerciseTypes
    {
        public static readonly (int Id, string Name)[] Types = new[]
        {
            (1, "Walking"),
            (2, "Hiking"),
            (3, "Jogging"),
            (4, "Running"),
            (5, "Weight Lifting"),
            (6, "Gardening")
        };

        public static IEnumerable<ExerciseType> AsExerciseTypes()
        {
            return Types.Select(t => new ExerciseType { Id = t.Id, TypeName = t.Name });
        }
    }
}