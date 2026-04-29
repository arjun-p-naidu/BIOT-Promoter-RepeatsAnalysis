using RyhaanSeq.Model.Factory;


namespace RyhaanSeq.Model
{
    internal static class SeqFn
    {
        public static Promoters seqSimilar(Promoters promoter)
        {
            List<List<int>> LenSorted = new List<List<int>>();
            Promoters output = new Promoters();
            for(int i = 0; i<promoter.Promoter_seq.Count-1;i++)
            {
                int index = promoter.Promoter_seq[i].Length;
                try 
                {
                    LenSorted[index].Add(i);
                }
                catch (ArgumentOutOfRangeException)
                {
                    for(int j = LenSorted.Count; j <= index; j++)
                    {
                        LenSorted.Add(new List<int>());
                    }
                    LenSorted[index].Add(i);
                }
            }

            for(int i = 0; i < LenSorted.Count; i++)
            {
                int j = 0;
                int count = LenSorted[i].Count;
                while(j < count)
                {
                    int k = j+1;
                    bool firstMatch = true;
                    while(k<count)
                    {
                        if (promoter.Promoter_seq[LenSorted[i][j]] == promoter.Promoter_seq[LenSorted[i][k]])
                        {
                            if (firstMatch)
                            {
                                output.addPromoterEntry(promoter, LenSorted[i][j]);
                                firstMatch = false;
                            }
                            output.addPromoterEntry(promoter, LenSorted[i][k]);
                            LenSorted[i].RemoveAt(k);
                            count--;
                        }
                        k++;
                    }
                    j++;
                }
            }
            return output;
        }

        public static List<List<int>> lenSortedPromoterFn(Promoters promoter)
        {
            List<List<int>> LenSortedPromoter = new List<List<int>>();
            for (int i = 0; i < promoter.Promoter_seq.Count - 1; i++)
            {
                int index = promoter.Promoter_seq[i].Length;
                try
                {
                    LenSortedPromoter[index].Add(i);
                }
                catch (ArgumentOutOfRangeException)
                {
                    for (int j = LenSortedPromoter.Count; j <= index; j++)
                    {
                        LenSortedPromoter.Add(new List<int>());
                    }
                    LenSortedPromoter[index].Add(i);
                }
            }
            return LenSortedPromoter;
        }

        public static void peakSimilar(List<List<int>> LenSortedPromoter, Peak peak, Promoters promoter, string return_path, StreamWriter sw)
        {
            Promoters output = new Promoters();
            
            List<List<int>> LenSortedpeak = new List<List<int>>();
            for (int i = 0; i < peak.Peak_seq.Count - 1; i++)
            {
                //Console.WriteLine("2");
                int index = peak.Peak_seq[i].Length;
                try
                {
                    LenSortedpeak[index].Add(i);
                }
                catch (ArgumentOutOfRangeException)
                {
                    for (int j = LenSortedpeak.Count; j <= index; j++)
                    {
                        LenSortedpeak.Add(new List<int>());
                    }
                    LenSortedpeak[index].Add(i);
                }
            }
            
            
                for (int i = 0; i < LenSortedPromoter.Count && i < LenSortedpeak.Count; i++)
                {
                    int j = 0;
                    while (j < LenSortedPromoter[i].Count)
                    {
                        int k = 0;
                        bool firstMatch = true;
                        while (k < LenSortedpeak[i].Count)
                        {
                            int pro_i = LenSortedPromoter[i][j];
                            int pea_j = LenSortedpeak[i][k];

                            if (promoter.Promoter_seq[pro_i] == peak.Peak_seq[pea_j])
                            {
                                if (firstMatch)
                                {
                                    output.addPromoterEntry(promoter, pro_i);
                                    //Console.WriteLine(promoter.Promoter_seq[pro_i]);
                                //Console.WriteLine(peak.Peak_seq[pea_j], peak.Factor[pea_j]);
                                    sw.WriteLine(promoter.Name[pro_i] + " " + promoter.Promoter_id[pro_i] + " " + promoter.Start[pro_i] + " " + promoter.End[pro_i] + " " + promoter.Promoter_seq[pro_i]);
                                    sw.WriteLine(peak.Peak_name[pea_j] + " " + peak.Factor[pea_j] + " " + peak.Start[pea_j] + " " + peak.End[pea_j] + " " + peak.Peak_seq[pea_j]);
                                    firstMatch = false;
                                }
                                output.addPromoterEntry(promoter, LenSortedPromoter[i][k]);
                                LenSortedPromoter[i].RemoveAt(k);
                            }
                            //Console.WriteLine(promoter.Promoter_seq[LenSortedPromoter[i][j]]);
                            //Console.WriteLine(peak.Peak_seq[LenSortedpeak[i][k]]);
                            k++;
                        }
                        j++;
                    }
                }
            
            seqMaxLen(LenSortedPromoter, LenSortedpeak);
            //return output;

        }

        private static bool isAFactor(List<string> factors, string title)
        {
            char[] delimiters = { '_' };
            string[] column = title.Split(delimiters);

            foreach (string c in column)
            {
                foreach (string f in factors)
                {
                    if (string.Equals(c, f, StringComparison.OrdinalIgnoreCase)) return true;
                }
            }
            return false;
        }

        public static void seqMaxLen(List<List<int>> LenSortedpromoter, List<List<int>> LenSortedpeak)
        {
            Console.WriteLine(LenSortedpromoter.Count);
            Console.WriteLine(LenSortedpeak.Count);
        }
    }
}
