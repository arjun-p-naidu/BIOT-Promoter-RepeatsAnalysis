using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RyhaanSeq.Model.Factory
{
    internal class ResultHash
    {
        public List<string> genome_list = new List<string>();
        public string promoterIndex;

        public ResultHash(string promoter, string genome)
        {
            genome_list.Add(promoter);
            this.promoterIndex = genome;
        }
        public void addPromoter(string promoter)
        {
            genome_list.Add(promoter);
        }
    }
}
