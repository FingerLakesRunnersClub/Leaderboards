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
            var date = new Date(new DateTimeOffset(new DateTime(2012, 3, 4, 5, 6, 7, DateTimeKind.Utc)));
            
            //act
            var display = date.Display;
            
            //assert
            Assert.Equal("3/4/2012 12:06am", display);
        }
        
        [Fact]
        public void CanHandleDST()
        {
            //arrange
            var date = new Date(new DateTimeOffset(new DateTime(2012, 3, 14, 5, 6, 7, DateTimeKind.Utc)));
            
            //act
            var display = date.Display;
            
            //assert
            Assert.Equal("3/14/2012 1:06am", display);
        }
    }
}