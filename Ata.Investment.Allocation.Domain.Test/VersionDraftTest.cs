using System;
using Xunit;

namespace Ata.Investment.Allocation.Domain.Test
{
    public class VersionDraftTest
    {
        [Fact]
        public void VersionNumberDecodedToDate_YearMonthTest()
        {
            VersionDraft versionDraft = new VersionDraft(1803);
            Assert.Equal(new DateTime(2018,03, 01), versionDraft.Date);
        }
        
        [Fact]
        public void VersionNumberDecodedToDate_YearMonthDayTest()
        {
            VersionDraft versionDraft = new VersionDraft(180305);
            Assert.Equal(new DateTime(2018,03, 05), versionDraft.Date);
        }
        
        [Fact]
        public void VersionNumberDecodedToDate_YearMonthDayHourTest()
        {
            VersionDraft versionDraft = new VersionDraft(18030513);
            Assert.Equal(new DateTime(2018,03, 05, 13, 0, 0), versionDraft.Date);
        }
        
        [Fact]
        public void VersionNumberDecodedToDate_YearMonthDayHourMinuteTest()
        {
            VersionDraft versionDraft = new VersionDraft(1803051305);
            Assert.Equal(new DateTime(2018,03, 05, 13, 5, 0), versionDraft.Date);
        }
    }
}