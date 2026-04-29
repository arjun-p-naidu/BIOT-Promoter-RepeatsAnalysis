using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace RyhaanSeq.Model.Factory
{

    internal class Promoters
    {

        public List<string> Name = new List<string>();
        public List<int> Start = new List<int>();
        public List<int> End = new List<int>();
        public List<string> Promoter_id = new List<string>();
        public List<string> Promoter_seq = new List<string>();

        public void addPromoterEntry(string name, int start, int end, string promoter_id)
        {

            this.Name.Add(name);
            this.Start.Add(start);
            this.End.Add(end);
            this.Promoter_id.Add(promoter_id);
            this.Promoter_seq.Add("");
        }
        public void addPromoterEntry(Promoters promoter,int index)
        {
            this.Name.Add(promoter.Name[index]);
            this.Start.Add(promoter.Start[index]);
            this.End.Add(promoter.End[index]);
            this.Promoter_id.Add(promoter.Promoter_id[index]);
            this.Promoter_seq.Add(promoter.Promoter_seq[index]);
        }
        public void promSeq(GRCh38 gRCh38)
        {
            List<string> output = new List<string>();
            int j = 0;


            for (int i = 0; i < this.Name.Count - 1; i++)
            {
                if (this.Name[i] != this.Name[i + 1])
                {
                    output.AddRange(gRCh38.returnGeneSeq(this.Name[i], this, i, j));
                    j = i + 1;
                }
            }
            output.AddRange(gRCh38.returnGeneSeq(this.Name[this.Name.Count - 1], this, this.Name.Count - 1, j));
            this.Promoter_seq = output;
        }
    }
}

