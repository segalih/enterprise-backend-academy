using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Academy.Api.Models;

namespace Academy.Tests.Seeders
{
    public static class ChallengeMasterCatalog
    {
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        public static Dictionary<string, ChallengeDto> GetAllChallenges()
        {
            var dict = new Dictionary<string, ChallengeDto>();

            var p1 = ChallengeDefinitionsLevel00To03.GetAll();
            foreach (var kv in p1) dict[kv.Key] = kv.Value;

            var p2 = ChallengeDefinitionsLevel04To09.GetAll();
            foreach (var kv in p2) dict[kv.Key] = kv.Value;

            var p3 = ChallengeDefinitionsLevel10To18.GetAll();
            foreach (var kv in p3) dict[kv.Key] = kv.Value;

            return dict;
        }

        public static void SyncToLessonJsonFiles(string lessonsDir)
        {
            if (!Directory.Exists(lessonsDir)) return;

            var challenges = GetAllChallenges();

            foreach (var (lessonId, challenge) in challenges)
            {
                var filePath = Path.Combine(lessonsDir, $"{lessonId}.json");
                if (File.Exists(filePath))
                {
                    var rawJson = File.ReadAllText(filePath);
                    using var doc = JsonDocument.Parse(rawJson);
                    var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(rawJson, _jsonOptions);
                    if (dict != null)
                    {
                        dict["hasExercise"] = true;
                        dict["codingChallenge"] = challenge;
                        var updatedJson = JsonSerializer.Serialize(dict, _jsonOptions);
                        File.WriteAllText(filePath, updatedJson);
                    }
                }
            }
        }
    }
}
