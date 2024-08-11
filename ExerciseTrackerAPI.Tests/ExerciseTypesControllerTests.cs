using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExerciseTrackerAPI.Controllers;
using ExerciseTrackerAPI.Data;
using ExerciseTrackerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ExerciseTrackerAPI.Tests
{
    public class ExerciseTypesControllerTests
    {
        [Fact]
        public async Task GetExerciseTypes_ReturnsAllTypes()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_GetAll")
                .Options;

            using (var context = new AppDbContext(options))
            {
                context.ExerciseTypes.AddRange(PredefinedExerciseTypes.AsExerciseTypes());
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new AppDbContext(options))
            {
                var controller = new ExerciseTypesController(context);
                var result = await controller.GetExerciseTypes();

                // Assert
                var actionResult = Assert.IsType<ActionResult<IEnumerable<ExerciseType>>>(result);
                var returnValue = Assert.IsType<List<ExerciseType>>(actionResult.Value);
                Assert.Equal(PredefinedExerciseTypes.Types.Length, returnValue.Count);
                Assert.Equal("Walking", returnValue.First(t => t.Id == 1).TypeName);
                Assert.Equal("Gardening", returnValue.Last().TypeName);
            }
        }
    }
}