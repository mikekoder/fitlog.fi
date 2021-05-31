using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Fitlog.Migration
{
    class EverKinetic
    {
        internal static void ImportData(string connStr)
        {
            var muscleMap = new Dictionary<string, Guid>
            {
                { "abdominals", Guid.Parse("F68C417F-CF57-4907-AB88-A2982449A224") },
                { "back", Guid.Parse("E0C23767-C6F7-451F-9141-58E87C133B57") },
                { "biceps brachii", Guid.Parse("385CD0FD-592F-4C42-A9E8-924B45CA2382") },
                { "bicpes", Guid.Parse("385CD0FD-592F-4C42-A9E8-924B45CA2382") },
                //{ "core", Guid.Parse("") },
                { "deltoid", Guid.Parse("86B6ECFC-C6CE-4787-9FA2-5C9FD9134C94") },
                { "deltoideus (clavicula)", Guid.Parse("86B6ECFC-C6CE-4787-9FA2-5C9FD9134C94") },
                { "erector spinae", Guid.Parse("E2F8F814-DF3D-4883-9C85-FAF5B95468B0") },
                { "forearm", Guid.Parse("CA975C3D-720F-4F54-BF4C-05C5279FC3AD") },
                { "forerm", Guid.Parse("CA975C3D-720F-4F54-BF4C-05C5279FC3AD") },
                { "gastrocnemius", Guid.Parse("91E25D36-A751-4786-923E-6B7FB09DB2FC") },
                { "glutaeus maximus", Guid.Parse("E7E5FBB6-F6AA-439E-BD99-8369C09C1F13") },
                //{ "hip abductors", Guid.Parse("" },
                { "ischiocrural muscles", Guid.Parse("760B826D-DD74-4908-BED2-3E89CCEE5DFF") },
                { "latissimus dorsi", Guid.Parse("E0C23767-C6F7-451F-9141-58E87C133B57") },
                { "obliques", Guid.Parse("F68C417F-CF57-4907-AB88-A2982449A224") },
                { "obliquus", Guid.Parse("F68C417F-CF57-4907-AB88-A2982449A224") },
                { "pectoralis major", Guid.Parse("A7B81E6B-2488-475B-9B9A-F93C465F3585") },
                { "quadriceps", Guid.Parse("AB922BE1-B171-413E-855F-72118F7A3A33") },
                { "should", Guid.Parse("86B6ECFC-C6CE-4787-9FA2-5C9FD9134C94") },
                { "soleus", Guid.Parse("91E25D36-A751-4786-923E-6B7FB09DB2FC") },
                { "trapezius", Guid.Parse("95E7266F-CA37-4730-A824-8324C1E864D4") },
                { "triceps brachii", Guid.Parse("41023B3F-511E-48EE-B772-0C0100CECEDE") },
                { "upper back", Guid.Parse("E0C23767-C6F7-451F-9141-58E87C133B57") }
            };
            var directory = @"C:\code\github.com\everkinetic-data\dist";
            var filename = "exercises.json";
            var json = File.ReadAllText(Path.Combine(directory, filename));
            var exercises = JsonSerializer.Deserialize<Exercise[]>(json);
            foreach(var exercise in exercises)
            {
                exercise.Equipment = exercise.Equipment.Select(e => e.ToLower()).Distinct().ToArray();
            }
            var equipments = exercises.SelectMany(e => e.Equipment).Distinct().ToArray();
            var equipmentMap = new Dictionary<string, Guid>();

            using (var conn = OpenConnection(connStr))
            {
                foreach(var equipment in equipments)
                {
                    var equipmentId = Guid.NewGuid();
                    try
                    {
                        conn.Execute("INSERT INTO Equipment(Id,Name) VALUES(@equipmentId,@equipment)",
                            new
                            {
                                equipmentId,
                                equipment
                            });

                        equipmentMap.Add(equipment, equipmentId);
                    }
                    catch
                    {
                    }
                }
                
            }

            foreach(var exercise in exercises)
            {
                var exerciseId = Guid.NewGuid();
                var primaryIds = exercise.Primary.Where(m => muscleMap.ContainsKey(m))
                    .Select(m => muscleMap[m])
                    .Distinct()
                    .ToArray();
                var secondaryIds = exercise.Secondary.Where(m => muscleMap.ContainsKey(m))
                    .Select(m => muscleMap[m])
                    .Distinct()
                    .Except(primaryIds)
                    .ToArray();

                using var conn = OpenConnection(connStr);
                using var tran = conn.BeginTransaction();
                try
                {
                    conn.Execute("INSERT INTO Exercise(Id,Name,Type,Created,Deleted) VALUES(@exerciseId,@Name,@Type,@Now,@Now)", new
                    {
                        exerciseId,
                        exercise.Name,
                        Type = exercise.Type.Contains("compound") ? "Compound" : exercise.Type.Contains("isolation") ? "Isolation" : "Other",
                        DateTimeOffset.Now
                    }, tran);

                    conn.Execute("INSERT INTO ExerciseTarget(ExerciseId,MuscleGroupId,Type) VALUES(@exerciseId,@m,'Primary')", primaryIds.Select(m => new
                    {
                        exerciseId,
                        m
                    }), tran);

                    conn.Execute("INSERT INTO ExerciseTarget(ExerciseId,MuscleGroupId,Type) VALUES(@exerciseId,@m,'Secondary')", secondaryIds.Select(m => new
                    {
                        exerciseId,
                        m
                    }), tran);

                    conn.Execute("INSERT INTO ExerciseEquipment(ExerciseId,EquipmentId) VALUES(@exerciseId, @id)", exercise.Equipment.Select(e => new
                    {
                        exerciseId,
                        id = equipmentMap[e]
                    }), tran);

                    foreach (var svg in exercise.Svg)
                    {
                        var imageId = Guid.NewGuid();
                        var data = File.ReadAllBytes(Path.Combine(directory, svg));

                        conn.Execute("INSERT INTO ExerciseImage(Id,ExerciseId,Data,Type) VALUES(@imageId,@exerciseId,@data,@Type)", new
                        {
                            imageId,
                            exerciseId,
                            data,
                            Type = svg.Contains("relaxation") ? "Relaxation" : svg.Contains("tension") ? "Tension" : "Other"
                        }, tran);
                    }

                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                }
            }
        }
        static SqlConnection OpenConnection(string connStr)
        {
            var conn = new SqlConnection(connStr);
            conn.Open();
            return conn;
        }

        class Exercise
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Title { get; set; }
            public string Type { get; set; }
            public string[] Primary { get; set; }
            public string[] Secondary { get; set; }
            public string[] Equipment { get; set; }
            public string[] Svg { get; set; }
        }
    }
}
