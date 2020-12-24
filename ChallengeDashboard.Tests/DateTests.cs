using System;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class DateTests
    {
        [Fact]
        public void CanFormatDate()
        {
            //arrange
            var date = new Date(new DateTime(2000, 1, 1));
            
            //act
            var display = date.Display;
            
            //assert
            Assert.Equal("1/1/2000 12:00:00 AM", display);
        }
    }
}