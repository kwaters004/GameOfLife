using System;
using Xunit;
using GameOfLifeKataWPF;

namespace GameOfLifeKataWPFTests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            Assert.True(Life.LiveOrDie(new Point(1,1)));
        }
    }
}
