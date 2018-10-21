using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CorporaTextAnalyzer
{
    public class Analyzer
    {
        public struct CTAReturn
        {
            public readonly ulong Rank;
            public readonly ulong Frequency;
            public CTAReturn(ulong rank, ulong frequency)
            {
                Rank = rank;
                Frequency = frequency;
            }
        }

        readonly Dictionary<string, CTAReturn> Database;
        public bool IsInitalized = false;
        private IProgress<int> _progress;

        public Analyzer()
        {
            Database = new Dictionary<string, CTAReturn>();
        }

        public void Initalize(string databaseFile = @"database\deu_mixed-typical_2011_100K-words.txt")
        {
            foreach (var line in File.ReadLines(databaseFile))
            {
                var split = line.Split('\t');
                var rank = ulong.Parse(split[0]);
                if (rank > 100)
                    Database.Add(split[1], new CTAReturn(rank - 100, ulong.Parse(split[3])));
            }
            IsInitalized = true;
        }



        public void AnalyzeFile(string importFile, string outputFile = null)
        {
            if (String.IsNullOrEmpty(outputFile))
            {
                outputFile = Path.ChangeExtension(importFile, null) + "-output.csv";
            }

            var output = File.Create(outputFile);
            var input = File.OpenRead(importFile);
            output.Write(new byte[] { 0xef, 0xbb, 0xbf }, 0, 3);//UTF8 BOM
            bool firstLine = true;
            long i = 0;
            long fileLinesCount = File.ReadLines(importFile).LongCount();
            foreach (var line in File.ReadLines(importFile))
            {
                i++;
                string writeLine = line;
                if (firstLine)
                {
                    firstLine = false;
                }
                else
                {
                    var split = line.Split(';');
                    if (split[2].Length < 2)
                        continue;
                    var rank = GetRankAndFrequency(split[4]);
                    writeLine = $"{writeLine.Substring(0, writeLine.Length - 1)}{rank.Frequency};{rank.Rank}";


                }

                var bytes = Encoding.UTF8.GetBytes(writeLine + "\r\n");
                output.Write(bytes, 0, bytes.Length);
                _progress?.Report((int)(100 * i / fileLinesCount));


            }
            output.Dispose();

        }

        public void SetProgressFunction(IProgress<int> progress)
        {
            _progress = progress;
        }

        public CTAReturn GetRankAndFrequency(string word)
        {
            if (!IsInitalized)
                Initalize();
            if (Database.TryGetValue(word, out CTAReturn ret))
                return ret;
            else
                return new CTAReturn(0, 0);
        }


    }
}
