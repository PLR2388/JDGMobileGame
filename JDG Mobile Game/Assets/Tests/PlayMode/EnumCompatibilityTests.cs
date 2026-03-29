using System;
using System.Linq;
using NUnit.Framework;
using LegacyCardOwner = Cards.CardOwner;
using LegacyCardFamily = Cards.CardFamily;
using LegacyCardType = Cards.CardType;
using DomainCardOwner = JDG.Domain.CardOwner;
using DomainCardFamily = JDG.Domain.Enums.CardFamily;
using DomainCardType = JDG.Domain.Enums.CardType;

namespace JDG.PlayMode.Tests
{
    /// <summary>
    /// Tests that verify enum values are compatible between legacy and domain enums.
    /// Phase 145: Added to catch potential issues when casting between enum types.
    ///
    /// These tests ensure that (DomainEnum)(int)legacyEnum produces the correct result.
    /// If these tests fail, it means the enum values have diverged and casts in the
    /// codebase (CardFactory, CardSyncService, InGameInvocationCard) will be broken.
    /// </summary>
    [TestFixture]
    public class EnumCompatibilityTests
    {
        [Test]
        public void CardOwner_LegacyAndDomain_HaveMatchingValues()
        {
            // Get all values from both enums
            var legacyValues = Enum.GetValues(typeof(LegacyCardOwner)).Cast<LegacyCardOwner>().ToList();
            var domainValues = Enum.GetValues(typeof(DomainCardOwner)).Cast<DomainCardOwner>().ToList();

            // Verify same number of values
            Assert.AreEqual(legacyValues.Count, domainValues.Count,
                "CardOwner enums have different number of values");

            // Verify each value matches
            foreach (var legacyValue in legacyValues)
            {
                var expectedDomainValue = (DomainCardOwner)(int)legacyValue;
                Assert.IsTrue(Enum.IsDefined(typeof(DomainCardOwner), expectedDomainValue),
                    $"Legacy CardOwner.{legacyValue} ({(int)legacyValue}) has no matching domain value");

                // Verify the name matches
                var legacyName = legacyValue.ToString();
                var domainName = expectedDomainValue.ToString();
                Assert.AreEqual(legacyName, domainName,
                    $"CardOwner names don't match: Legacy={legacyName}, Domain={domainName}");
            }
        }

        [Test]
        public void CardFamily_LegacyAndDomain_HaveMatchingValues()
        {
            var legacyValues = Enum.GetValues(typeof(LegacyCardFamily)).Cast<LegacyCardFamily>().ToList();
            var domainValues = Enum.GetValues(typeof(DomainCardFamily)).Cast<DomainCardFamily>().ToList();

            Assert.AreEqual(legacyValues.Count, domainValues.Count,
                "CardFamily enums have different number of values");

            foreach (var legacyValue in legacyValues)
            {
                var expectedDomainValue = (DomainCardFamily)(int)legacyValue;
                Assert.IsTrue(Enum.IsDefined(typeof(DomainCardFamily), expectedDomainValue),
                    $"Legacy CardFamily.{legacyValue} ({(int)legacyValue}) has no matching domain value");

                var legacyName = legacyValue.ToString();
                var domainName = expectedDomainValue.ToString();
                Assert.AreEqual(legacyName, domainName,
                    $"CardFamily names don't match: Legacy={legacyName}, Domain={domainName}");
            }
        }

        [Test]
        public void CardType_LegacyAndDomain_HaveMatchingValues()
        {
            var legacyValues = Enum.GetValues(typeof(LegacyCardType)).Cast<LegacyCardType>().ToList();
            var domainValues = Enum.GetValues(typeof(DomainCardType)).Cast<DomainCardType>().ToList();

            Assert.AreEqual(legacyValues.Count, domainValues.Count,
                "CardType enums have different number of values");

            foreach (var legacyValue in legacyValues)
            {
                var expectedDomainValue = (DomainCardType)(int)legacyValue;
                Assert.IsTrue(Enum.IsDefined(typeof(DomainCardType), expectedDomainValue),
                    $"Legacy CardType.{legacyValue} ({(int)legacyValue}) has no matching domain value");

                var legacyName = legacyValue.ToString();
                var domainName = expectedDomainValue.ToString();
                Assert.AreEqual(legacyName, domainName,
                    $"CardType names don't match: Legacy={legacyName}, Domain={domainName}");
            }
        }

        [Test]
        public void CardOwner_RoundTripConversion_PreservesValue()
        {
            // Test that converting legacy -> domain -> legacy preserves the value
            foreach (LegacyCardOwner legacyValue in Enum.GetValues(typeof(LegacyCardOwner)))
            {
                var domainValue = (DomainCardOwner)(int)legacyValue;
                var backToLegacy = (LegacyCardOwner)(int)domainValue;
                Assert.AreEqual(legacyValue, backToLegacy,
                    $"Round-trip conversion failed for CardOwner.{legacyValue}");
            }
        }

        [Test]
        public void CardFamily_RoundTripConversion_PreservesValue()
        {
            foreach (LegacyCardFamily legacyValue in Enum.GetValues(typeof(LegacyCardFamily)))
            {
                var domainValue = (DomainCardFamily)(int)legacyValue;
                var backToLegacy = (LegacyCardFamily)(int)domainValue;
                Assert.AreEqual(legacyValue, backToLegacy,
                    $"Round-trip conversion failed for CardFamily.{legacyValue}");
            }
        }

        [Test]
        public void CardType_RoundTripConversion_PreservesValue()
        {
            foreach (LegacyCardType legacyValue in Enum.GetValues(typeof(LegacyCardType)))
            {
                var domainValue = (DomainCardType)(int)legacyValue;
                var backToLegacy = (LegacyCardType)(int)domainValue;
                Assert.AreEqual(legacyValue, backToLegacy,
                    $"Round-trip conversion failed for CardType.{legacyValue}");
            }
        }
    }
}
