using Xunit;

namespace Ata.Investment.Api.Test
{
    public class MeetingVmTest
    {
        [Fact]
        public void GetNamesFromDraft_JoinedTest()
        {
//            Meeting meeting = new Meeting(
//                DateTimeOffset.Now, 
//                new Advisor("Meeting VM Test", "test_a@meetingVm.test", DateTimeOffset.Now)
//            );
//
//            meeting.Draft = @"{""clients"": [{""name"": ""resumed""}, {""name"": ""joined""}]}";
//
//            var draft = JsonConvert.DeserializeObject(meeting.Draft);
//
//            MeetingVM meetingVm = JsonConvert.DeserializeObject<MeetingVM>(meeting.Draft);
//
//            Assert.Equal("resumed", meetingVm.PrimaryName);
//            Assert.Equal("joined", meetingVm.JointName);
        }

        [Fact]
        public void GetNamesFromDraft_SingleTest()
        {
//            Meeting meeting = new Meeting(
//                DateTimeOffset.Now, 
//                new Advisor("Meeting VM Test", "test_a@meetingVm.test", DateTimeOffset.Now)
//            );
//
//            meeting.Draft = @"{""clients"": [{""name"": ""resumed""}]}";
//
//            var draft = JsonConvert.DeserializeObject(meeting.Draft);
//
//            MeetingVM meetingVm = JsonConvert.DeserializeObject<MeetingVM>(meeting.Draft);
//
//            Assert.Equal("resumed", meetingVm.PrimaryName);
//            Assert.Equal("", meetingVm.JointName);
        }

        [Fact]
        public void GetNamesFromDraft_PrimaryNotYetEnteredTest()
        {
//            Meeting meeting = new Meeting(
//                DateTimeOffset.Now, 
//                new Advisor("Meeting VM Test", "test_a@meetingVm.test", DateTimeOffset.Now)
//            );
//
//            meeting.Draft = @"{""clients"": []}";
//
//            var draft = JsonConvert.DeserializeObject(meeting.Draft);
//
//            MeetingVM meetingVm = JsonConvert.DeserializeObject<MeetingVM>(meeting.Draft);
//
//            Assert.Equal("", meetingVm.PrimaryName);
//            Assert.Equal("", meetingVm.JointName);
        }
    }
}
