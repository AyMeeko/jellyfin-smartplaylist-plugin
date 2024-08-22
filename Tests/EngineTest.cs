using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Jellyfin.Plugin.SmartPlaylist.QueryEngine.Tests
{
    [TestFixture]
    public class EngineTests
    {
        [Test]
        public void CompileRule_EqualityOperator_ReturnsValidFunc()
        {
            // Arrange
            var rule = new Expression("Name", "Equal", "Some Movie");

            // Act
            var func = Engine.CompileRule<Operand>(rule);

            // Assert
            var operand = new Operand("TestOperand") { Name = "Some Movie" };
            Assert.IsTrue(func(operand));

            operand.Name = "Another Movie";
            Assert.IsFalse(func(operand));
        }

        [Test]
        public void CompileRule_GreaterThanOperator_ReturnsValidFunc()
        {
            // Arrange
            var rule = new Expression("CommunityRating", "GreaterThan", "4.5");

            // Act
            var func = Engine.CompileRule<Operand>(rule);

            // Assert
            var operand = new Operand("TestOperand") { CommunityRating = 5 };
            Assert.IsTrue(func(operand));

            operand.CommunityRating = 4;
            Assert.IsFalse(func(operand));
        }

        [Test]
        public void CompileRule_MatchRegexOperator_ReturnsValidFunc()
        {
            // Arrange
            var rule = new Expression("Name", "MatchRegex", "^Some.*");

            // Act
            var func = Engine.CompileRule<Operand>(rule);

            // Assert
            var operand = new Operand("TestOperand") { Name = "Some Movie" };
            Assert.IsTrue(func(operand));

            operand.Name = "Another Movie";
            Assert.IsFalse(func(operand));
        }

        [Test]
        public void CompileRule_ContainsOperator_ReturnsValidFunc()
        {
            // Arrange
            var rule = new Expression("Tags", "Contains", "flow type: slow");

            // Act
            var func = Engine.CompileRule<Operand>(rule);

            // Assert
            var operand = new Operand("TestOperand") { Tags = new List<string> { "flow type: slow", "difficulty: beginner" } };
            Assert.IsTrue(func(operand));

            operand.Tags = new List<string> { "flow type: focus" };
            Assert.IsFalse(func(operand));
        }

        [Test]
        public void FixRuleSets_FixesPremiereDate()
        {
            // Arrange
            var ruleSets = new List<ExpressionSet>
            {
                new ExpressionSet
                {
                    Expressions = new List<Expression>
                    {
                        new Expression("PremiereDate", "GreaterThan", "2022-01-01")
                    }
                }
            };

            // Act
            var fixedRuleSets = Engine.FixRuleSets(ruleSets);

            // Assert
            var premiereDate = Engine.ConvertToUnixTimestamp(new DateTime(2022, 1, 1));
            Assert.AreEqual(premiereDate.ToString(), fixedRuleSets[0].Expressions[0].TargetValue);
        }
    }
}
