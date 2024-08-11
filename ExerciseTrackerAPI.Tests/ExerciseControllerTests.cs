using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using ExerciseTrackerAPI.Controllers;
using ExerciseTrackerAPI.Data;
using ExerciseTrackerAPI.Models;

namespace ExerciseTrackerAPI.Tests
{
	public class ExerciseControllerTests
	{
		private static DbContextOptions<AppDbContext> CreateNewContextOptions()
		{
			return new DbContextOptionsBuilder<AppDbContext>()
					.UseInMemoryDatabase(databaseName: "TestDatabase")
					.Options;
		}

		private async Task SeedTestData(AppDbContext context)
		{
			context.Exercises.RemoveRange(context.Exercises);
			await context.SaveChangesAsync();

			for (int i = 1; i <= 10; i++)
			{
				context.Exercises.Add(new Exercise()
				{
					Id = i,
					Date = DateTime.Now.AddDays(-i),
					Duration = TimeSpan.FromMinutes(30),
					ExerciseTypeId = 1,
					Distance = i * 0.5m
				});
			}
			await context.SaveChangesAsync();
		}

		[Fact]
		public async Task PostExercise_ReturnsCreatedAtActionResult()
		{
			var options = CreateNewContextOptions();
			using (var context = new AppDbContext(options))
			{
				var controller = new ExercisesController(context);
				var newExercise = new Exercise
				{
					Date = DateTime.Today,
					Duration = TimeSpan.FromMinutes(30),
					ExerciseTypeId = 1,
					Distance = 5.0m
				};

				var result = await controller.PostExercise(newExercise);

				var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
				var returnValue = Assert.IsType<Exercise>(createdAtActionResult.Value);
				Assert.Equal(newExercise.Date, returnValue.Date);
				Assert.Equal(newExercise.Duration, returnValue.Duration);
				Assert.Equal(newExercise.ExerciseTypeId, returnValue.ExerciseTypeId);
				Assert.Equal(newExercise.Distance, returnValue.Distance);
			}
		}

		[Fact]
		public async Task PutExercise_ReturnsNoContent()
		{
			var options = CreateNewContextOptions();
			using (var context = new AppDbContext(options))
			{

				await SeedTestData(context);
			}

			using (var context = new AppDbContext(options))
			{
				var controller = new ExercisesController(context);
				var updatedExercise = new Exercise
				{
					Id = 1,
					Date = DateTime.Today.AddDays(1),
					Duration = TimeSpan.FromMinutes(45),
					ExerciseTypeId = 2,
					Distance = 7.5m
				};

				var result = await controller.PutExercise(1, updatedExercise);

				Assert.IsType<NoContentResult>(result);
			}

			using (var context = new AppDbContext(options))
			{
				var dbExercise = await context.Exercises.FindAsync(1);
				Assert.NotNull(dbExercise);
				Assert.Equal(DateTime.Today.AddDays(1).Date, dbExercise.Date.Date);
				Assert.Equal(TimeSpan.FromMinutes(45), dbExercise.Duration);
				Assert.Equal(2, dbExercise.ExerciseTypeId);
				Assert.Equal(7.5m, dbExercise.Distance);
			}
		}

		[Fact]
		public async Task DeleteExercise_ReturnsNoContent()
		{
			var options = CreateNewContextOptions();
			using (var context = new AppDbContext(options))
			{
				await SeedTestData(context);
			}

			using (var context = new AppDbContext(options))
			{
				var controller = new ExercisesController(context);

				var result = await controller.DeleteExercise(1);

				Assert.IsType<NoContentResult>(result);
			}

			using (var context = new AppDbContext(options))
			{
				Assert.Null(await context.Exercises.FindAsync(1));
			}
		}

		[Fact]
		public async Task GetExercise_ReturnsExercise()
		{
			var options = CreateNewContextOptions();

			using (var context = new AppDbContext(options))
			{
				await SeedTestData(context);
				var exerciseCount = await context.Exercises.CountAsync();
			}

			using (var context = new AppDbContext(options))
			{
				var controller = new ExercisesController(context);
				var actionResult = await controller.GetExercise(1);

				Assert.NotNull(actionResult);
				Assert.IsType<ActionResult<Exercise>>(actionResult);

				var returnValue = Assert.IsType<Exercise>(actionResult.Value);
				Assert.NotNull(returnValue);
				Assert.Equal(DateTime.Now.AddDays(-1).Date, returnValue.Date.Date);
				Assert.Equal(TimeSpan.FromMinutes(30), returnValue.Duration);
				Assert.Equal(1, returnValue.ExerciseTypeId);
				Assert.Equal(0.5m, returnValue.Distance);
			}
		}

		[Fact]
		public async Task GetExerciseStats_ReturnsCorrectStats()
		{
			var options = CreateNewContextOptions();

			using (var context = new AppDbContext(options))
			{
				await SeedTestData(context);
				var exerciseCount = await context.Exercises.CountAsync();
			}

			using (var context = new AppDbContext(options))
			{
				var controller = new ExercisesController(context);
				var actionResult = await controller.GetExerciseStats();

				Assert.NotNull(actionResult);
				Assert.IsType<ActionResult<ExerciseStats>>(actionResult);

				var result = actionResult.Result;

				Assert.IsType<OkObjectResult>(result);
				var okObjectResult = result as OkObjectResult;

				Assert.NotNull(okObjectResult.Value);
				var stats = Assert.IsType<ExerciseStats>(okObjectResult.Value);

				Assert.Equal(10, stats.TotalExercises);
				Assert.Equal(10, stats.NumberOfDays);
				Assert.Equal(TimeSpan.FromMinutes(300), stats.TotalDuration);
				Assert.Equal(27.5m, stats.TotalDistance);
			}
		}

		[Fact]
		public async Task GetFastestExercises_ReturnsCorrectFastestExercises()
		{
			var options = CreateNewContextOptions();

			using (var context = new AppDbContext(options))
			{
				await SeedTestData(context);
				var exerciseCount = await context.Exercises.CountAsync();
			}

			using (var context = new AppDbContext(options))
			{
				var controller = new ExercisesController(context);
				var actionResult = await controller.GetFastestExercises();

				Assert.NotNull(actionResult);
				Assert.IsType<ActionResult<IEnumerable<FastestExercise>>>(actionResult);

				var result = actionResult.Result;

				Assert.IsType<OkObjectResult>(result);
				var okObjectResult = result as OkObjectResult;

				Assert.NotNull(okObjectResult.Value);
				var fastestExercises = Assert.IsType<List<FastestExercise>>(okObjectResult.Value);

				Assert.Equal(10, fastestExercises.Count);
				Assert.Equal(0.5m, fastestExercises[0].DistanceGroup);
				Assert.Equal(5.0m, fastestExercises[9].DistanceGroup);
			}
		}
	}
}