using RyhaanSeq.Model.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RyhaanSeq.Model
{
    internal class Display
    {
        //displays the result
        public void display(List<ResultHash> result,string div) { 
        foreach (ResultHash promoter in result)
        {
            int count = promoter.genome_list.Count;
            Console.Write(string.Format("{0,-20} was found {1,-3} times\n", promoter.promoterIndex, count));
            List<string> promoterListCopyDistinct = new List<string>(promoter.genome_list.Distinct());
            foreach (string sRepeats in promoterListCopyDistinct)
            {
                int countPromoter = promoter.genome_list.Count(i => i == sRepeats);
                Console.WriteLine(string.Format("{0,-20} {1,-80} - {2,-3} {3,8}", " ", sRepeats, countPromoter, "Times"));


            }
            Console.WriteLine("\n" + div + "\n");

            }
         Console.ReadKey();
        }
        public void displayPromoterGenome(Promoters promoter)
        {
            for(int i = 0;i < promoter.Name.Count-1;i++)
            {
                Console.Write(promoter.Name[i] + "    " + promoter.Start[i] + "    " + promoter.End[i] + "    " + promoter.Promoter_seq[i] +"\n");
            }
        }
    }
}
