using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FLRC.ChallengeDashboard.Controllers;
using NSubstitute;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class AthleteControllerTests
    {
        [Fact]
        public async Task CanGetAthleteInfo()
        {
            //arrange
            var dataService = Substitute.For<IDataService>();
            dataService.GetAllResults().Returns(new List<Course>
            {
                new Course
                {
                    Results = new List<Result>
                    {
                        new Result
                        {
                            Athlete = new Athlete { ID = 123 },
                            Duration = new Time(new TimeSpan(1, 2, 3))
                        }
                    }
                }
            });
            var controller = new AthleteController(dataService);

            //act
            var response = await controller.Index(123);

            //assert
            Assert.Equal((uint)123, ((AthleteViewModel)(response.Model)).Athlete.ID);
        }
    }
}