using System;
using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;

namespace Tests
{
    public class AnimesTests
    {
        [Fact]
        public void AnimeTest() 
        {
            var mockSet = new Mock<DbSet<MangaSurvWebApi.Model.Anime>>();

            var context = new Mock<MangaSurvWebApi.Model.MangaSurvContext>();
            context.Setup(a => a.Animes).Returns(mockSet.Object);
            
        }
    }
}
