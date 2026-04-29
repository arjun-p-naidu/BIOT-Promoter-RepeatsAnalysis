using RyhaanSeq.Model.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RyhaanSeq.Model
{
    internal static class Writer
    {
        
        public static void write(List<ResultHash> result, string div,string file_location) 
        {
            string filePath = file_location;
            string link = WriteLink(filePath);
            using (FileStream fs = File.Create(filePath)) { }
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                foreach (ResultHash promoter in result)
                {
                    int count = promoter.genome_list.Count;
                    sw.Write(string.Format("{0,-20} was found {1,-3} times\n", promoter.promoterIndex, count));
                    List<string> promoterListCopyDistinct = new List<string>(promoter.genome_list.Distinct());
                    foreach (string sRepeats in promoterListCopyDistinct)
                    {
                        int countPromoter = promoter.genome_list.Count(i => i == sRepeats);
                        sw.WriteLine(string.Format("{0,-20} {1,-80} - {2,-3} {3,8}", " ", sRepeats, countPromoter, "Times"));


                    }
                    sw.WriteLine("\n" + div + "\n");

                }
            }
            Console.WriteLine("File written to "+link+" (Ctrl+click to open)");       
        }

        public static void writeRepeat(SRepeatsFilter output, string div, string file_location)
        {
            string filePath = file_location;
            string link = WriteLink(filePath);
            using (FileStream fs = File.Create(filePath)) { }
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                Promoters tempPromoter = new Promoters();
                SimpleRepeat tempSRepeats = new SimpleRepeat();
                for (int i = 0; i < output.resultPromoter.Name.Count; i++)
                {
                    tempPromoter = output.resultPromoter;
                    for (int j = output.resultSRepeats[i].Name.Count - 1; j > 0; j--)
                    {
                        tempSRepeats = output.resultSRepeats[i];
                        sw.WriteLine(string.Format(

                            "{0,-4} {1,-8} {2,-8} {3,-8} {4,-8} {5,-3}{6,-3}{7,-3}{8,-3}{9,-3}{10,-3}{11,-3}{12,-3}{13,-3}{14,-3}{15,-3}{16,-60}",
                            tempPromoter.Promoter_id[i].Substring(0, 4),
                            tempPromoter.Start[i],
                            tempPromoter.End[i],
                            tempSRepeats.Start[j],
                            tempSRepeats.End[j],
                            tempSRepeats.Length[j],
                            tempSRepeats.Percent_identity[j],
                            tempSRepeats.Copy_number[j],
                            tempSRepeats.PercentATCG[j],
                            tempSRepeats.Mismatches[j],
                            tempSRepeats.Alignment[j],
                            tempSRepeats.score7[j],
                            tempSRepeats.score8[j],
                            tempSRepeats.score9[j],
                            tempSRepeats.score10[j],
                            tempSRepeats.FinalScore[j],
                            tempSRepeats.Genome[j]
                            ));

                    }
                    sw.WriteLine("-----------------------------------------------------------------");
                }
            }
            Console.WriteLine("File written to " + link + " (Ctrl+click to open)");
        }

        

        public static void writePeak(PeakCommon output, string div, string file_location)
        {
            string filePath = file_location;
            string link = WriteLink(filePath);
            using (FileStream fs = File.Create(filePath)) { }
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                Promoters tempPromoter = new Promoters();
                Peak tempSRepeats = new Peak();
                for(int i = 0;i< output.resultPromoter.Name.Count;i++)
                {
                    tempPromoter = output.resultPromoter;
                    for (int j = output.resultPeak[i].Name.Count-1; j > 0;j--)
                    {
                        tempSRepeats = output.resultPeak[i];
                        sw.WriteLine(tempPromoter.Promoter_id[i].Substring(0, 4)+
                            tempPromoter.Start[i]+
                            tempPromoter.End[i]+
                            tempSRepeats.Start[j]+
                            tempSRepeats.End[j]
                            );
                        
                    }
                }
            }
            Console.WriteLine("File written to " + link + " (Ctrl+click to open)");
        }
        public static void write(Promoters promoter, string file_location)
        {
            string filePath = file_location;
            string link = WriteLink(filePath);
            using (FileStream fs = File.Create(filePath)) { }
            using (StreamWriter sw = new StreamWriter(filePath))
            {


                sw.WriteLine("Promoter_id" + "  |  " + "Promoter_seq" + "  |  " + "promoter Start[i]" + "  |  " + "promoter End");
                for (int i = 0; i < promoter.Name.Count - 1; i++)
                {
                    
                    sw.WriteLine
                        (string.Format("{0,-55} | {1,-40:C} | {2,10} | {3,10}",
                        promoter.Promoter_id[i],
                        promoter.Promoter_seq[i],
                        promoter.Start[i],
                        promoter.End[i]
                        )
                        );
                    
                }
            }
            Console.WriteLine("File written to " + link + " (Ctrl+click to open)");
        }

        public static void writeSimProRep(Promoters promoterSet, List<ResultHash> result, string file_location)
        {
            string filePath = file_location;
            string link = WriteLink(filePath);
            using (FileStream fs = File.Create(filePath)) { }
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                string temp = "";
                for (int i = promoterSet.Name.Count - 1; i > 0; i--)
                {
                    if (temp != promoterSet.Promoter_seq[i])
                    {
                        sw.WriteLine("Here’s the list of all promoters that have the sequence " + promoterSet.Promoter_seq[i] + " in them");
                        temp = promoterSet.Promoter_seq[i];
                    }

                    sw.WriteLine(promoterSet.Promoter_seq[i]+" with promoter id \n" + promoterSet.Promoter_id[i] + "  has:");
                    foreach (string str in result[i].genome_list)
                    {
                        sw.WriteLine(str);
                    }
                    sw.WriteLine();
                    sw.WriteLine("------------------------------------");
                }
            }
            Console.WriteLine("File written to " + link + " (Ctrl+click to open)");
        }
        public static string WriteLink(string filePath)
        {
            string path = Path.Combine(Environment.CurrentDirectory, filePath);
            string url = $"file:///{path}";
            string link = $"\u001B]8;;{url}\u0007{filePath}\u001B]8;;\u0007";
            return link;
        }
    }
}
