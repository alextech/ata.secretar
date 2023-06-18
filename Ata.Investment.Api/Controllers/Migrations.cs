using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharedKernel;
using Ata.Investment.Profile.Data;
using Ata.Investment.Profile.Domain.Points;
using Ata.Investment.Profile.Domain.Profile;

namespace Ata.Investment.Api.Controllers
{
    [ApiController]
    public class Migrations : ControllerBase
    {
        private readonly ProfileContext _profileContext;

        public Migrations(ProfileContext profileContext)
        {
            _profileContext = profileContext;
        }

        [HttpGet("/api/migrations")]
        public async Task Migrate()
        {
            DbCommand meetingsSelectCmd = _profileContext.Database.GetDbConnection().CreateCommand();
            meetingsSelectCmd.CommandText = "SELECT id, KycDocument FROM Investments.[Meetings]";
            _profileContext.Database.OpenConnection();

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Include,
                TypeNameHandling = TypeNameHandling.All
            };

            Dictionary<int, string> docs = new Dictionary<int, string>();

            DbDataReader dbDataReader = meetingsSelectCmd.ExecuteReader();
            while (dbDataReader.Read())
            {
                int id = dbDataReader.GetInt32(0);
                string doc = dbDataReader.GetString(1);

                JObject docObj = JObject.Parse(doc);
                string date = docObj["Date"].Value<string>();
                DateTime dt =
                    DateTime.ParseExact(date, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                int year = dt.Year;

                JToken? pClient = docObj["PrimaryClient"];
                JToken? profiles = pClient["Profiles"];
                if (profiles == null)
                {
                    continue;
                }
                migrateProfiles(profiles, year);

                JToken? jClient = docObj["JointClient"];
                if (jClient != null)
                {
                    profiles = jClient["Profiles"];
                    if (profiles == null)
                    {
                        continue;
                    }

                    migrateProfiles(profiles, year);
                }

                doc = JsonConvert.SerializeObject(docObj, settings);

                docs.Add(id, doc);
            }

            dbDataReader.Close();

            foreach (KeyValuePair<int,string> docPair in docs)
            {
                DbCommand updateMeetingCmd = _profileContext.Database.GetDbConnection().CreateCommand();
                updateMeetingCmd.CommandText = "UPDATE Investments.[Meetings] SET KycDocument = @doc WHERE id = @id";

                SqlParameter idParam = new SqlParameter("@id", SqlDbType.Int);
                idParam.Value = docPair.Key;
                SqlParameter docParam = new SqlParameter("@doc", SqlDbType.NVarChar);
                docParam.Value = docPair.Value;

                updateMeetingCmd.Parameters.Add(docParam);
                updateMeetingCmd.Parameters.Add(idParam);

                int queryResult = updateMeetingCmd.ExecuteNonQuery();
                Console.WriteLine(queryResult);
            }
        }

        private static void migrateProfiles(JToken? profiles, int originYear)
        {
            foreach (JToken profileObj in profiles)
            {
                JToken? timeObj = profileObj["TimeHorizon"];
                TimeHorizon newTimeHorizon = new TimeHorizon(originYear);


                if (timeObj is JValue)
                {
                    int oldTime = timeObj.Value<int>();
                    newTimeHorizon.Range = ProfilePoints.MatchRiskRange(oldTime) + originYear;
                }

                // object is simple enough without typical settings
                // and option that generates ID conflicts with final object it is merged with
                string serializeObject = JsonConvert.SerializeObject(newTimeHorizon);
                profileObj["TimeHorizon"] = JObject.Parse(serializeObject);
            }
        }
    }
}
