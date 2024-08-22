using System;
using System.Collections.Generic;
using System.Linq;
using Jellyfin.Data.Entities;
using Jellyfin.Plugin.SmartPlaylist;
using Jellyfin.Plugin.SmartPlaylist.QueryEngine;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Jellyfin.Plugin.SmartPlaylist.Tests
{
    public class SmartPlaylistTests
    {
        private readonly Mock<ILogger> _mockLogger;
        private readonly Mock<ILibraryManager> _mockLibraryManager;
        private readonly User _mockUser;

        public SmartPlaylistTests()
        {
            _mockLogger = new Mock<ILogger>();
            _mockLibraryManager = new Mock<ILibraryManager>();
            _mockLibraryManager
              .Setup(lm => lm.GetPeople(It.IsAny<BaseItem>()))
              .Returns(new List<PersonInfo>());
            _mockUser = new User("TestUser", "bar", "baz");
        }

        [Fact]
        public void FilterPlaylistItems_TagsContainFooAndBar_ReturnsMatchingItems()
        {
            // Arrange
            var dto = new SmartPlaylistDto
            {
                Id = "1",
                Name = "Test Playlist",
                FileName = "testplaylist",
                User = "TestUser",
                ExpressionSets = new List<ExpressionSet>
                {
                    new ExpressionSet
                    {
                        Expressions = new List<Expression>
                        {
                          new Expression("Tags", "Contains", "difficulty: beginner"),
                          new Expression("Tags", "Contains", "flow type: Slow")
                        }
                    }
                },
                MaxItems = 100,
                Order = new OrderDto { Name = "NoOrder" }
            };

            var smartPlaylist = new SmartPlaylist(dto, _mockLogger.Object);

            var item1 = new Mock<BaseItem>() { Name = "Item 1" };
            item1.Setup(x => x.IsPlayed(_mockUser)).Returns(false);
            item1.Object.Id = Guid.NewGuid();
            item1.Object.Tags = new string[] { "difficulty: beginner", "flow type: Slow" };

            var item2 = new Mock<BaseItem>() { Name = "Item 2" };
            item2.Object.Id = Guid.NewGuid();
            item2.Setup(x => x.IsPlayed(_mockUser)).Returns(false);
            item2.Object.Tags = new string[] { "difficulty: beginner" };

            var items = new List<BaseItem>
            {
                item1.Object,
                item2.Object,
            };

            Console.WriteLine("Items:");
            foreach (var item in items)
            {
              Console.WriteLine($"Item ID: {item.Id}, {string.Join(", ", item.Tags)}");
            }

            // Act
            var result = smartPlaylist.FilterPlaylistItems(items, _mockLibraryManager.Object, _mockUser);

            Console.WriteLine("Filtered Playlist Items:");
            foreach (var id in result)
            {
              Console.WriteLine($"Item ID: {id}");
            }

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal(item1.Object.Id, resultList[0]);
        }
    }
}

