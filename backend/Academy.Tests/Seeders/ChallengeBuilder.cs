using System;
using System.Collections.Generic;
using System.Linq;
using Academy.Api.Models;

namespace Academy.Tests.Seeders
{
    public static class ChallengeBuilder
    {
        public static ChallengeDto Create(
            string id,
            string title,
            string difficulty,
            string problemStatement,
            string inputFormat,
            string outputFormat,
            string constraints,
            (string Input, string Output, string Explanation)[] examples,
            string starterCode,
            string solutionCode,
            (string Name, string Input, string Expected, bool IsHidden, string Category)[] testCases,
            string[] hints,
            string explanation)
        {
            return new ChallengeDto
            {
                Id = id,
                Title = title,
                Difficulty = difficulty,
                ProblemStatement = problemStatement,
                InputFormat = inputFormat,
                OutputFormat = outputFormat,
                Constraints = constraints,
                Examples = examples.Select(e => new ChallengeExampleDto
                {
                    Input = e.Input,
                    Output = e.Output,
                    Explanation = e.Explanation
                }).ToList(),
                StarterCode = starterCode,
                SolutionCode = solutionCode,
                TestCases = testCases.Select(t => new ChallengeTestCase
                {
                    Name = t.Name,
                    Description = $"{t.Name}: input '{t.Input}' -> expected '{t.Expected}'",
                    Input = t.Input,
                    ExpectedOutput = t.Expected,
                    IsHidden = t.IsHidden,
                    Category = t.Category
                }).ToList(),
                Hints = hints.ToList(),
                Explanation = explanation
            };
        }
    }
}
