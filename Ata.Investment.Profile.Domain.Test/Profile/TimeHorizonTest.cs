using System;
using System.Collections.Generic;
using Ata.Investment.Profile.Domain.Profile;
using Xunit;
using Range = SharedKernel.Range;

namespace Ata.Investment.Profile.Domain.Test.Profile
{
    public class TimeHorizonTest
    {
        [Theory]
        [MemberData(nameof(TimeHorizonRanges))]
        public void TimeHorizonDefaultsToThirdOfGoalTest(int expectedTime, int from, int to, int origin)
        {
            if (expectedTime == -1)
            {
                Assert.Throws<ArgumentException>(() =>
                    new TimeHorizon(origin) {Range = new Range(from, to)}.WithdrawTime);
            }

            if (expectedTime == -2)
            {
                Assert.Throws<ArgumentException>(() =>
                    new TimeHorizon(origin) {Range = new Range(from, to)}.WithdrawTime);
            }

            Assert.Equal(expectedTime,
                new TimeHorizon(origin) {Range = new Range(from, to)}.WithdrawTime);
        }

        [Fact]
        public void TimeHorizonCanUpdateThirdWithdrawTimeIndependently()
        {
            // ============ SETUP =============
            bool called = false;
            TimeHorizon timeHorizon = new TimeHorizon(2020)
            {
                Range = new Range(2021, 2024)
            };
            timeHorizon.TimeHorizonChanged += () => called = true;

            // ============ ACT ===============
            timeHorizon.WithdrawTime = 4;


            // ============= VERIFY ==========
            Assert.Equal(4, timeHorizon.WithdrawTime);
            Assert.Equal(2021, timeHorizon.Range.Min);
            Assert.Equal(2024, timeHorizon.Range.Max);
            Assert.True(called, "Timehorizon did not trigger event on property change.");
        }

        [Fact]
        public void WithdrawTimeUpdateWhenRangeChangesTest()
        {
            // ============ SETUP =============
            bool called = false;

            TimeHorizon timeHorizon = new TimeHorizon(2020);
            timeHorizon.TimeHorizonChanged += () => called = true;


            // ============ ACT ===============
            timeHorizon.Range = new Range()
            {
                Min = 2025,
                Max = 2030
            };


            // ============= VERIFY ==========
            Assert.Equal(6, timeHorizon.WithdrawTime);
            Assert.Equal(2026, timeHorizon.WithdrawYear);
            Assert.True(called, "Timehorizon did not trigger event on property change.");
        }

        [Theory]
        [MemberData(nameof(WithdrawTimeUpdates))]
        public void UpdateWithdrawTimeTest(
                int originalWithdrawTime,
                int originalFrom,
                int originalTo,
                int newWithdrawTime,
                int newFrom,
                int newTo
            )
        {
            // =========== SETUP ==========

            bool called = false;
            TimeHorizon timeHorizon = new TimeHorizon(2020);
            timeHorizon.TimeHorizonChanged += () => called = true;
            timeHorizon.Range = new Range()
            {
                Min = originalFrom,
                Max = originalTo
            };
            Assert.Equal(originalWithdrawTime, timeHorizon.WithdrawTime);

            // =========== ACT ===========
            timeHorizon.WithdrawTime = newWithdrawTime;

            // =========== VERIFY ========
            Assert.Equal(newFrom, timeHorizon.Range.Min);
            Assert.Equal(newTo, timeHorizon.Range.Max);
            Assert.Equal(newWithdrawTime, timeHorizon.WithdrawTime);
            Assert.True(called, "Timehorizon did not trigger event on property change.");
        }

        [Fact]
        public void TimeHorizonRangeIntersectionTest()
        {
            TimeHorizon timeHorizon = new TimeHorizon(2020)
            {
                Range = new Range()
                {
                    Min = 2022,
                    Max = 2025
                }
            };

            Assert.True(timeHorizon.Range.Intersects(new Range(2021, 2023)));
            Assert.True(timeHorizon.Range.Intersects(new Range(2025, 2027)));
            Assert.True(timeHorizon.Range.Intersects(new Range(2023, 2024)));
            Assert.False(timeHorizon.Range.Intersects(new Range(2020, 2020)));
            Assert.False(timeHorizon.Range.Intersects(new Range(2026, 2027)));
        }

        [Fact]
        public void TimeHorizonRangeIntersectionSelectWithdrawalTimeTest()
        {
            TimeHorizon timeHorizon = new TimeHorizon(2020)
            {
                Range = new Range()
                {
                    Min = 2022,
                    Max = 2025
                }
            };

            Range selectedRange = new Range(2021, 2023);
            Assert.Equal(2022, timeHorizon.Range.LowestIntersectionWith(selectedRange));
            selectedRange = new Range(2024, 2030);
            Assert.Equal(2024, timeHorizon.Range.LowestIntersectionWith(selectedRange));
        }

        public static IEnumerable<object[]> TimeHorizonRanges()
        {
            // yield return new object[] {-1, 2, 1, 0};
            // yield return new object[] {-2, 5, 5, 2020};
            yield return new object[] {0, 2020, 2020, 2020};
            yield return new object[] {4, 2024, 2024, 2020};
            yield return new object[] {7, 2022, 2037, 2020};
            yield return new object[] {1, 2021, 2022, 2020};
        }

        public static IEnumerable<object[]> WithdrawTimeUpdates()
        {
            // origin = 2020
            // original wTime | original FROM | original TO || new wTime | new FROM | new TO
            // yield return new object[] { 3, 2025, 2030, 4, 2023, 2028 };
            yield return new object[] { 4, 2020, 2032, 1, 2020, 2032 };
            yield return new object[] { 7, 2025, 2031, 8, 2025, 2031 };
            // yield return new object[] { 4, 2020, 2032, 1, 2020, 2020 };
        }
    }
}