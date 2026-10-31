using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Academy.Api.Models;

namespace Academy.Api.Services
{
    public static class ChallengeDiffEngine
    {
        public static string MakeVisual(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            var sb = new StringBuilder();
            foreach (char c in text)
            {
                switch (c)
                {
                    case ' ':
                        sb.Append('␠');
                        break;
                    case '\t':
                        sb.Append('⇥');
                        break;
                    case '\r':
                        // Show CR tag if needed or keep for newline marker
                        break;
                    case '\n':
                        sb.Append("↵\n");
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            return sb.ToString();
        }

        public static string Normalize(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            // Normalize line endings to \n
            string s = text.Replace("\r\n", "\n").Replace('\r', '\n');

            // Split and trim trailing whitespace per line
            var lines = s.Split('\n');
            var cleaned = new List<string>();
            foreach (var l in lines)
            {
                cleaned.Add(l.TrimEnd(' ', '\t'));
            }

            // Join and trim trailing/leading blank lines
            string result = string.Join("\n", cleaned).Trim();
            return result;
        }

        public static bool DetectCrLf(string text)
        {
            return text != null && text.Contains("\r\n");
        }

        public static (bool hasDiff, bool lineEndingMismatch, List<DiffLineDto> diffLines, string expectedVisual, string actualVisual)
            CompareAndDiff(string rawExpected, string rawActual)
        {
            string normExpected = Normalize(rawExpected);
            string normActual = Normalize(rawActual);

            bool expHasCrLf = DetectCrLf(rawExpected);
            bool actHasCrLf = DetectCrLf(rawActual);
            bool lineEndingMismatch = (expHasCrLf != actHasCrLf) && (!string.IsNullOrEmpty(rawExpected) && !string.IsNullOrEmpty(rawActual));

            string expectedVisual = MakeVisual(rawExpected) + (expHasCrLf ? " [CRLF]" : " [LF]");
            string actualVisual = MakeVisual(rawActual) + (actHasCrLf ? " [CRLF]" : " [LF]");

            bool exactMatch = string.Equals(normExpected, normActual, StringComparison.Ordinal);
            if (exactMatch)
            {
                return (false, lineEndingMismatch, new List<DiffLineDto>(), expectedVisual, actualVisual);
            }

            var expLines = normExpected.Split('\n');
            var actLines = normActual.Split('\n');
            int maxLines = Math.Max(expLines.Length, actLines.Length);
            var diffLines = new List<DiffLineDto>();

            for (int i = 0; i < maxLines; i++)
            {
                int lineNum = i + 1;
                string? exp = i < expLines.Length ? expLines[i] : null;
                string? act = i < actLines.Length ? actLines[i] : null;

                if (exp != null && act != null)
                {
                    if (string.Equals(exp, act, StringComparison.Ordinal))
                    {
                        diffLines.Add(new DiffLineDto
                        {
                            LineNumber = lineNum,
                            Type = "Equal",
                            Expected = exp,
                            Actual = act,
                            ExpectedVisual = MakeVisual(exp),
                            ActualVisual = MakeVisual(act)
                        });
                    }
                    else
                    {
                        string note = FindFirstCharMismatch(exp, act);
                        diffLines.Add(new DiffLineDto
                        {
                            LineNumber = lineNum,
                            Type = "Modified",
                            Expected = exp,
                            Actual = act,
                            ExpectedVisual = MakeVisual(exp),
                            ActualVisual = MakeVisual(act),
                            Note = note
                        });
                    }
                }
                else if (exp != null)
                {
                    diffLines.Add(new DiffLineDto
                    {
                        LineNumber = lineNum,
                        Type = "Deleted",
                        Expected = exp,
                        Actual = null,
                        ExpectedVisual = MakeVisual(exp),
                        ActualVisual = null,
                        Note = "Baris output diharapkan tetapi tidak ditemukan di output aktual"
                    });
                }
                else if (act != null)
                {
                    diffLines.Add(new DiffLineDto
                    {
                        LineNumber = lineNum,
                        Type = "Added",
                        Expected = null,
                        Actual = act,
                        ExpectedVisual = null,
                        ActualVisual = MakeVisual(act),
                        Note = "Baris tambahan pada output aktual yang tidak diharapkan"
                    });
                }
            }

            return (true, lineEndingMismatch, diffLines, expectedVisual, actualVisual);
        }

        private static string FindFirstCharMismatch(string exp, string act)
        {
            int minLen = Math.Min(exp.Length, act.Length);
            for (int i = 0; i < minLen; i++)
            {
                if (exp[i] != act[i])
                {
                    string expChar = exp[i] == ' ' ? "spasi (␠)" : $"'{exp[i]}'";
                    string actChar = act[i] == ' ' ? "spasi (␠)" : $"'{act[i]}'";
                    return $"Perbedaan karakter pada kolom {i + 1}: diharapkan {expChar}, aktual {actChar}.";
                }
            }
            if (exp.Length > act.Length)
            {
                return $"Output aktual terpotong lebih pendek: kurang {exp.Length - act.Length} karakter.";
            }
            if (act.Length > exp.Length)
            {
                return $"Output aktual memuat tambahan {act.Length - exp.Length} karakter berlebih di ujung baris.";
            }
            return "Perbedaan format tidak terduga.";
        }
    }
}
