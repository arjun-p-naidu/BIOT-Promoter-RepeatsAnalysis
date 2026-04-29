using RyhaanSeq.Model.GPUParallel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace RyhaanSeq.Model.Factory
{

    internal class SRepeatsFilter 
    {
        public List<ResultHash> output = new List<ResultHash>();
        public List<string> index = new List<string>();  
        public Promoters resultPromoter = new Promoters();
        public List<SimpleRepeat> resultSRepeats = new List<SimpleRepeat>();
    }

}
