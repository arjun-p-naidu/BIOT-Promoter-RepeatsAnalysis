using RyhaanSeq.Model.GPUParallel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RyhaanSeq.Model.Factory
{
    internal class SimpleRepeat
    {
        public List<string> Name = new List<string>();
        public List<int> Start = new List<int>();
        public List<int> End = new List<int>();
        public List<string> Genome = new List<string>();
        public List<int> Length = new List<int>();
        public List<float> Percent_identity = new List<float>();
        public List<int> Copy_number = new List<int>();
        public List<int> PercentATCG = new List<int>();
        public List<int> Mismatches = new List<int>();
        public List<int> Alignment = new List<int>();
        public List<int> score7 = new List<int>();
        public List<int> score8 = new List<int>();
        public List<int> score9 = new List<int>();
        public List<int> score10 = new List<int>();
        public List<float> FinalScore = new List<float>();

        public void addSReapeatEntry(
            string name,
            int start,
            int end,
            string Genome,
            int Length,
            float Percent_identity,
            int Copy_number,
            int PercentATCG,
            int Mismatches,
            int Alignment,
            int score7,
            int score8,
            int score9,
            int score10,
            float FinalScore
            )
        {

            this.Name.Add(name);
            this.Start.Add(start);
            this.End.Add(end);
            this.Genome.Add(Genome);
            this.Length.Add(Length);
            this.Percent_identity.Add(Percent_identity);
            this.Copy_number.Add(Copy_number);
            this.PercentATCG.Add(PercentATCG);
            this.Mismatches.Add(Mismatches);
            this.Alignment.Add(Alignment);
            this.score7.Add(score7);
            this.score8.Add(score8);
            this.score9.Add(score9);
            this.score10.Add(score10);
            this.FinalScore.Add(FinalScore);
        }

        public SimpleRepeat(SimpleRepeat sRepeats, int index)
        {
            this.Name.Add(sRepeats.Name[index]);
            this.Start.Add(sRepeats.Start[index]);
            this.End.Add(sRepeats.End[index]);
            this.Genome.Add(sRepeats.Genome[index]);
            this.Length.Add(sRepeats.Length[index]);
            this.Percent_identity.Add(sRepeats.Percent_identity[index]);
            this.Copy_number.Add(sRepeats.Copy_number[index]);
            this.PercentATCG.Add(sRepeats.PercentATCG[index]);
            this.Mismatches.Add(sRepeats.Mismatches[index]);
            this.Alignment.Add(sRepeats.Alignment[index]);
            this.score7.Add(sRepeats.score7[index]);
            this.score8.Add(sRepeats.score8[index]);
            this.score9.Add(sRepeats.score9[index]);
            this.score10.Add(sRepeats.score10[index]);
            this.FinalScore.Add(sRepeats.FinalScore[index]);
        }
        public void addSReapeatEntry(SimpleRepeat sRepeats, int index)
        {
            this.Name.Add(sRepeats.Name[index]);
            this.Start.Add(sRepeats.Start[index]);
            this.End.Add(sRepeats.End[index]);
            this.Genome.Add(sRepeats.Genome[index]);
            this.Length.Add(sRepeats.Length[index]);
            this.Percent_identity.Add(sRepeats.Percent_identity[index]);
            this.Copy_number.Add(sRepeats.Copy_number[index]);
            this.PercentATCG.Add(sRepeats.PercentATCG[index]);
            this.Mismatches.Add(sRepeats.Mismatches[index]);
            this.Alignment.Add(sRepeats.Alignment[index]);
            this.score7.Add(sRepeats.score7[index]);
            this.score8.Add(sRepeats.score8[index]);
            this.score9.Add(sRepeats.score9[index]);
            this.score10.Add(sRepeats.score10[index]);
            this.FinalScore.Add(sRepeats.FinalScore[index]);
        }
        public SimpleRepeat()
        {

        }
        public void doGPUmatch(Promoters promoter, SRepeatsFilter result)
        {
            int[,] output = GPUSReapeatFilter.SReapeatFilter(promoter.Start, promoter.End, this.End);

            for (int i = 0; i < output.GetLength(0); i++)
            {
                result.index.Add(promoter.Promoter_id[i]);
                result.resultPromoter.addPromoterEntry(promoter, i);
                result.output.Add(new ResultHash(this.Genome[output[i, 1]], promoter.Promoter_id[i]));
                result.resultSRepeats.Add(new SimpleRepeat(this, output[i, 1]));
                for (int j = 2; j < output[i, 0]; j++)
                {
                    result.output[i].addPromoter(this.Genome[output[i, j]]);
                    result.resultSRepeats[i].addSReapeatEntry(this, output[i, j]);
                }
            }

            output = null;
        }
        public void read()
        {

            string simpleRepeatPath = Path.GetFullPath(@"Data\simpleRepeat.txt");
            string[] Lines = File.ReadAllLines(simpleRepeatPath);

            foreach (string line in Lines)
            {
                char[] delimiters = { '	' };

                string[] column = line.Split(delimiters);
                int end = int.Parse(column[3]);
                int start = int.Parse(column[2]);
                if (column[16].Length > 5)
                {
                    this.addSReapeatEntry(
                        column[1],
                        start,
                        end,
                        column[16],
                        int.Parse(column[5]),
                        float.Parse(column[6]),
                        int.Parse(column[7]),
                        int.Parse(column[8]),
                        int.Parse(column[9]),
                        int.Parse(column[10]),
                        int.Parse(column[11]),
                        int.Parse(column[12]),
                        int.Parse(column[13]),
                        int.Parse(column[14]),
                        float.Parse(column[15])
                        );
                }
            }
        }
    }
}
