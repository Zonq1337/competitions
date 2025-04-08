using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TournamentResults
{
    class Program
    {
        static void Main(string[] args)
        {
            var inputFile = "vxod.txt";
            var outputFile = "result.csv";

            ProcessParticipants(inputFile, outputFile);
        }

        static void ProcessParticipants(string inputFilePath, string outputFilePath)
        {
            List<Participant> participants = new List<Participant>();

            using (var reader = new StreamReader(inputFilePath, Encoding.UTF8))
            {
                string line;
                Participant current = null;

                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();

                    if (line.EndsWith(":"))
                    {
                        if (current != null)
                            participants.Add(current);

                        current = new Participant();
                        current.Name = line.Substring(0, line.Length - 1).Trim();
                    }
                    else if (current != null && line.Length > 0)
                    {
                        string[] parts = line.Split(' ');
                        for (int i = 0; i < parts.Length; i += 2)
                        {
                            int reps = int.Parse(parts[i]);
                            string[] timeParts = parts[i + 1].Split(':');
                            int time = int.Parse(timeParts[0]) * 60 + int.Parse(timeParts[1]);

                            current.TotalReps += reps;
                            current.TotalTime += time;
                        }
                    }
                }

                if (current != null)
                    participants.Add(current);
            }

            participants.Sort((a, b) =>
            {
                if (a.TotalReps != b.TotalReps)
                    return b.TotalReps.CompareTo(a.TotalReps);
                else
                    return a.TotalTime.CompareTo(b.TotalTime);
            });

            if (participants.Count > 0)
            {
                participants[0].Place = 1;
                for (int i = 1; i < participants.Count; i++)
                {
                    if (participants[i].TotalReps == participants[i - 1].TotalReps &&
                        participants[i].TotalTime == participants[i - 1].TotalTime)
                    {
                        participants[i].Place = participants[i - 1].Place;
                    }
                    else
                    {
                        participants[i].Place = i + 1;
                    }
                }
            }

            using (var writer = new StreamWriter(outputFilePath, false, new UTF8Encoding(true)))
            {
                foreach (Participant p in participants)
                {
                    int minutes = p.TotalTime / 60;
                    int seconds = p.TotalTime % 60;
                    string time = $"{minutes}:{seconds:D2}";

                    writer.WriteLine($"{p.Name} {p.TotalReps} {time} {p.Place}");
                }
            }
        }
    }

    class Participant
    {
        public string Name { get; set; }
        public int TotalReps { get; set; }
        public int TotalTime { get; set; }
        public int Place { get; set; }
    }
}