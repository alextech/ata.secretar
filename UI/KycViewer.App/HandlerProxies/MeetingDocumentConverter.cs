using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ata.Investment.Profile.Domain;
using Ata.Investment.Profile.Domain.KYC;

namespace KycViewer.App.HandlerProxies;

public class MeetingDocumentConverter : JsonConverter<Meeting>
{
    public override void WriteJson(JsonWriter writer, Meeting? meeting, JsonSerializer serializer)
    {
        KycDocument doc = meeting.KycDocument;

        meeting.Purpose = doc.Purpose;
        meeting.Date = doc.Date;
        meeting.CreatedFor =
            $"{doc.PrimaryClient.Name}{(doc.IsJoint ? " and " + doc.JointClient.Name : "")}";
        meeting.AllocationVersion =
            doc.AllocationVersion;

        JToken jToken = JToken.FromObject(meeting);
        JObject jObject = (JObject)jToken;

        jObject.WriteTo(writer);
    }

    public override Meeting ReadJson(JsonReader reader, Type objectType, Meeting existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        return serializer.Deserialize<Meeting>(reader);
    }
}