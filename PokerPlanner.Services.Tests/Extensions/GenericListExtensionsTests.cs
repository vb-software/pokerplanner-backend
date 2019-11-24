using System.Collections.Generic;
using PokerPlanner.Services.Extensions;
using Xunit;

namespace PokerPlanner.Services.Tests.Extensions
{
    public class GenericListExtensionsTests
    {
        [Fact]
        public void ListIsNullOrEmptyListNullTest()
        {
            List<int> listToTest = null;

            var result = GenericListExtensions.IsNullOrEmpty(listToTest);

            Assert.True(result);
        }

        [Fact]
        public void ListIsNullOrEmptyListEmptyTest()
        {
            var listToTest = new List<int>();

            var result = GenericListExtensions.IsNullOrEmpty(listToTest);

            Assert.True(result);
        }

        [Fact]
        public void ListIsNullOrEmptyListValidTest()
        {
            var listToTest = new List<int> { 1, 2, 3};

            var result = GenericListExtensions.IsNullOrEmpty(listToTest);

            Assert.False(result);
        }
    }
}