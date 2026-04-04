using System;
using System.Linq;
using NUnit.Framework;
using JDG.Domain;
using JDG.Domain.Enums;

namespace JDG.PlayMode.Tests
{
    /// <summary>
    /// Tests that verify domain enum values are well-defined and consistent.
    /// Phase 145: Originally tested legacy-to-domain enum compatibility.
    /// Phase 166: Legacy enums removed - now validates domain enums have expected values.
    /// </summary>
    [TestFixture]
    public class EnumCompatibilityTests
    {
        [Test]
        public void CardOwner_HasExpectedValues()
        {
            var values = Enum.GetValues(typeof(CardOwner)).Cast<CardOwner>().ToList();

            Assert.AreEqual(3, values.Count, "CardOwner should have 3 values");
            Assert.AreEqual(0, (int)CardOwner.NotDefined);
            Assert.AreEqual(1, (int)CardOwner.Player1);
            Assert.AreEqual(2, (int)CardOwner.Player2);
        }

        [Test]
        public void CardFamily_HasExpectedValues()
        {
            var values = Enum.GetValues(typeof(CardFamily)).Cast<CardFamily>().ToList();

            Assert.AreEqual(14, values.Count, "CardFamily should have 14 values");
            Assert.AreEqual(0, (int)CardFamily.None);
            Assert.AreEqual(1, (int)CardFamily.Comics);
            Assert.AreEqual(13, (int)CardFamily.Any);
        }

        [Test]
        public void CardType_HasExpectedValues()
        {
            var values = Enum.GetValues(typeof(CardType)).Cast<CardType>().ToList();

            Assert.AreEqual(5, values.Count, "CardType should have 5 values");
            Assert.AreEqual(0, (int)CardType.Contre);
            Assert.AreEqual(1, (int)CardType.Effect);
            Assert.AreEqual(2, (int)CardType.Equipment);
            Assert.AreEqual(3, (int)CardType.Field);
            Assert.AreEqual(4, (int)CardType.Invocation);
        }
    }
}
