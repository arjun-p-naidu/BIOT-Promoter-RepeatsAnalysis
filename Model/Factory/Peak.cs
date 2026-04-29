using RyhaanSeq.Model.GPUParallel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RyhaanSeq.Model.Factory
{
    internal class Peak
    {
        public int Citrome_id;
        public List<string> Name = new List<string>();
        public List<int> Start = new List<int>();
        public List<int> End = new List<int>();
        public List<string> Peak_name = new List<string>();
        public List<string> Peak_seq = new List<string>();
        public List<string> Factor = new List<string>();

        public Peak(int fileNo) { this.Citrome_id = fileNo; }
        public Peak() { }
        public Peak(Peak peak, int index)
        {
            this.Name.Add(peak.Name[index]);
            this.Start.Add(peak.Start[index]);
            this.End.Add(peak.End[index]);
            this.Peak_name.Add(peak.Peak_name[index]);
            this.Peak_seq.Add(peak.Peak_seq[index]);
            this.Peak_seq.Add(peak.Factor[index]);

        }
        public void addPeakEntry(Peak peak, int index)
        {
            this.Name.Add(peak.Name[index]);
            this.Start.Add(peak.Start[index]);
            this.End.Add(peak.End[index]);
            this.Peak_name.Add(peak.Peak_name[index]);
            this.Peak_seq.Add(peak.Peak_seq[index]);
            this.Peak_seq.Add(peak.Factor[index]);
        }
        public void addPeakEntry(string name, int start, int end, string Peak_name,string factor)
        {
            this.Name.Add(name);
            this.Start.Add(start);
            this.End.Add(end);
            this.Peak_name.Add(Peak_name);
            this.Factor.Add(factor);
        }
        public void addPeakEntry(string name, int start, int end, string Peak_name, string Peak_seq,string factor)
        {
            this.Name.Add(name);
            this.Start.Add(start);
            this.End.Add(end);
            this.Peak_name.Add(Peak_name);
            this.Peak_seq.Add(Peak_seq);
            this.Factor.Add(factor);
        }
        public void peakSeq(GRCh38 gRCh38)
        {
            int j = 0;

            for (int i = 0; i < this.Name.Count - 1; i++)
            {
                if (this.Name[i] != this.Name[i + 1])
                {
                    this.Peak_seq.AddRange(gRCh38.returnGeneSeq(this.Name[i], this, i, j));
                    j = i + 1;
                }
            }
            if (this.Name.Count > 0)
            {
                this.Peak_seq.AddRange(gRCh38.returnGeneSeq(this.Name[this.Name.Count - 1], this, this.Name.Count - 1, j));
                //Console.WriteLine("hmmmmmmmm" + gRCh38.returnGeneSeq(this.Name[this.Name.Count - 1], this, this.Name.Count - 1, j)[1]);
                //Console.WriteLine("baah" + this.Peak_seq[1]);
            }
        }
        public void write(string dir_name)
        {
            string dir = @"PeakResult" + dir_name;
            DirectoryInfo di = Directory.CreateDirectory(dir);
            string file = @"PeakResult" + dir_name + "\\" + this.Citrome_id.ToString() + ".txt";
            using (FileStream fs = File.Create(file)) { }
            using (StreamWriter sw = new StreamWriter(file))
            {
                for (int j = 0; j < this.Name.Count - 1; j++)
                {
                    //Console.WriteLine(this.Name[j] + " " + this.Start[j] + " " + this.End[j] + " " + this.Peak_name[j] + " " + this.Peak_seq[j] + " " + this.Factor[j]);
                    sw.WriteLine(this.Name[j] + " " + this.Start[j] + " " + this.End[j] + " " + this.Peak_name[j] + " " + this.Peak_seq[j] + " " + this.Factor[j]);
                }
            }
        }
        public void free()
        {
            this.Name.RemoveAll(x => x != null);
            this.Start.RemoveAll(x => x != null);
            this.End.RemoveAll(x => x != null);
            this.Peak_name.RemoveAll(x => x != null);
            this.Peak_seq.RemoveAll(x => x != null);
            this.Factor.RemoveAll(x => x != null);
        }
        public void doGPUmatch(Promoters promoter, PeakCommon result)
        {
            if(this.Start.Count == 0)
            {
                return;
            }
            int[,] output = GPUPeakCommon.peakCommon(promoter.Start, promoter.End, this.Start, this.End);

            for (int i = 0; i < output.GetLength(0); i++)
            {

                if (result.index.Contains(promoter.Promoter_id[i]))
                {
                    for (int j = 2; j < output[i, 0]; j++)
                    {
                        result.output[i].addPromoter(this.Peak_seq[output[i, j]]);
                        result.resultPeak[i].addPeakEntry(this, output[i, j]);
                    }
                }
                else
                {
                    result.index.Add(promoter.Promoter_id[i]);
                    result.resultPromoter.addPromoterEntry(promoter, i);
                    result.output.Add(new ResultHash(this.Peak_seq[output[i, 1]], promoter.Promoter_id[i]));
                    result.resultPeak.Add(new Peak(this, output[i, 1]));
                    for (int j = 2; j < output[i, 0]; j++)
                    {
                        result.output[i].addPromoter(this.Peak_seq[output[i, j]]);
                        result.resultPeak[i].addPeakEntry(this, output[i, j]);
                    }
                }
            }
        }
    }
}
