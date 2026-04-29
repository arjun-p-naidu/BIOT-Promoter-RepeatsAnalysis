using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RyhaanSeq.Model.Factory
{
    internal class PeakCommon
    {
        public List<ResultHash> output = new List<ResultHash>();
        public List<string> index = new List<string>();
        public Promoters resultPromoter = new Promoters();
        public List<Peak> resultPeak = new List<Peak>();
    }
}
