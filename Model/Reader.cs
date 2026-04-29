using RyhaanSeq.Model.Factory;


namespace RyhaanSeq.Model
{
    internal static class Read
    {    
        public static List<Peak> readPeak(string path)
        {
            string[] files = Directory.GetFiles(path);
            List<Peak> peakList = new List<Peak>();
            foreach (string file in files)
            {
                string[] Lines = File.ReadAllLines(file);
                int fileNo = int.Parse(Path.GetFileName(file).Substring(0, Path.GetFileName(file).Length - 4));
                Peak P1 = new Peak(fileNo);
                foreach (string line in Lines)
                {
                    try
                    {
                        char[] delimiters = { '	' ,' '};
                        string[] column = line.Split(delimiters);

                        int start = int.Parse(column[1]);
                        int end = int.Parse(column[2]);
                        

                        P1.addPeakEntry(column[0], start, end, column[3], column[5]);
                        //Console.WriteLine(temp);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine(file);
                        break;
                    }
                }
                peakList.Add(P1);

            }
            return peakList;
        }


        public static PeakCommon readPeakSeq(string path, Promoters promoter)
        {
            int count = 0;
            string[] files = Directory.GetFiles(path);
            PeakCommon result_peakK562 = new PeakCommon();
            ParallelOptions options = new ParallelOptions {MaxDegreeOfParallelism = 4 };
            Parallel.ForEach(files, options, file =>
            {
                string[] Lines = File.ReadAllLines(file);
                int fileNo = int.Parse(Path.GetFileName(file).Substring(0, Path.GetFileName(file).Length - 4));
                Peak P1 = new Peak(fileNo);
                foreach (string line in Lines)
                {
                    try
                    {
                        char[] delimiters = { '	', ' ' };
                        string[] column = line.Split(delimiters);

                        int start = int.Parse(column[1]);
                        int end = int.Parse(column[2]);

                        P1.addPeakEntry(column[0], start, end, column[3], column[4], column[5]);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine(file);
                        continue;
                    }
                }
                count++;
                Console.WriteLine(count + " / " + files.Length);
            });
            return result_peakK562;
        }

        public static void readPeakSeq2(string path,string return_path, List<List<int>> LenSortedPromoter, Promoters promoter)
        {
            int count = 0;
            string[] files = Directory.GetFiles(path);
            using (StreamWriter sw = new StreamWriter(return_path))
            {
                foreach (string file in files)
                {
                    string[] Lines = File.ReadAllLines(file);
                    int fileNo = int.Parse(Path.GetFileName(file).Substring(0, Path.GetFileName(file).Length - 4));
                    Peak P1 = new Peak(fileNo);
                    foreach (string line in Lines)
                    {
                        try
                        {
                            char[] delimiters = { '	', ' ' };
                            string[] column = line.Split(delimiters);

                            int start = int.Parse(column[1]);
                            int end = int.Parse(column[2]);

                            if (column.Length == 5) { P1.addPeakEntry(column[0], start, end, column[3], column[4], column[5]); }
                            else { P1.addPeakEntry(column[0], start, end, column[3], column[4], column[5]); }
                        }
                        catch (Exception)
                        {
                            Console.WriteLine(file);
                            break;
                        }
                    }
                    SeqFn.peakSimilar(LenSortedPromoter, P1, promoter, return_path,sw);
                    P1.free();
                    count++;
                    Console.WriteLine(count + "/" + files.Length);
                }
            }
            //return result_peakK562;
        }


        public static Promoters readPromoters()
        {
            string promoterPath = Path.GetFullPath(@"Data\promotersHg38.txt");
            string[] Lines = File.ReadAllLines(promoterPath);
            Promoters P1 = new Promoters();
            foreach (string line in Lines)
            {
                char[] delimiters = { '	' };
                string[] column = line.Split(delimiters);

                
                    int start = int.Parse(column[1]);
                    int end = int.Parse(column[2]);

                    P1.addPromoterEntry(column[0], start-200, end+50, column[3]);

                
            }
            return P1;
        }
        public static Promoters readPromoters_hg19mm()
        {
            string promoterPath = Path.GetFullPath(@"Data\hg38PromoterSet.txt");
            string[] Lines = File.ReadAllLines(promoterPath);
            Promoters P1 = new Promoters();

            foreach (string line in Lines)
            {
                char[] delimiters = { '	' };
                string[] column = line.Split(delimiters);

                if (column[3].Substring(0, 4) == "hg38")
                {
                    int start = int.Parse(column[1]);
                    int end = int.Parse(column[2]);
                    string promoter_id_hg38 = "hg38" + "::" + start + ".." + end;
                    P1.addPromoterEntry(column[0], start, end, promoter_id_hg38);
                }
            }
            return P1;
        }
        public static Promoters readPromoters_hg38()
        {
            string promoterPath = Path.GetFullPath(@"Data\reducedPromoterSet.txt");
            string[] Lines = File.ReadAllLines(promoterPath);
            Promoters P1 = new Promoters();

            foreach (string line in Lines)
            {
                char[] delimiters = { '	' };
                string[] column = line.Split(delimiters);
                
                if (column[3].Substring(0, 4) == "hg38")
                {
                    int start = int.Parse(column[1]);
                    int end = int.Parse(column[2]);

                    P1.addPromoterEntry(column[0], start, end, column[3]);
                }
                else
                {
                    int start = int.Parse(column[1]);
                    int end = int.Parse(column[2]);
                    string promoter_id_hg38 = "hg38"+"::"+start+".."+end+ column[3].Substring(20, column[3].Length-20);
                    P1.addPromoterEntry(column[0], start, end, promoter_id_hg38);
                }
            }
            return P1;
        }

        public static void readPeaks(List<Peak> PeakRead,string peakFile, GRCh38 gRCh38, ParallelOptions options)
        {
            PeakRead = Read.readPeak(peakFile);
            int count = 0;
            Parallel.ForEach(PeakRead, options, peak =>
            {
                peak.peakSeq(gRCh38);

                count++;
                Console.WriteLine(count + "/" + PeakRead.Count);
                peak.write("peakHUVEC-C");
                peak.free();
            });
        }

        /*
        public static SimpleRepeat readSimpleRepeat() 
        {

            string simpleRepeatPath = Path.GetFullPath(@"Data\simpleRepeat.txt");
            string[] Lines = File.ReadAllLines(simpleRepeatPath);
            SimpleRepeat S1 = new SimpleRepeat();

            foreach (string line in Lines)
            {
                char[] delimiters = { '	' };

                string[] column = line.Split(delimiters);
                int end = int.Parse(column[3]);
                int start = int.Parse(column[2]);
                if (column[16].Length > 5)
                {
                    S1.addSReapeatEntry(
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
            return S1;

        }     
        
        public static List<string> readGRCh38(string name,Promoters promoter,int i,int j)
        {
            string halfPath = Path.GetFullPath(@"Data\hg38_split\");
            string filePath = halfPath + name + ".txt";
            string read = string.Empty;
            read = File.ReadAllText(filePath);
            string Line = Regex.Replace(read, @"\r\n?|\n", "");

            int check = Line.Length;
            List<string> output = new List<string>();

            for (;j<=i;j++)
            {
                if(promoter.End[j] - promoter.Start[j] > 5)
                {
                    output.Add(Line.Substring(promoter.Start[j], promoter.End[j] - promoter.Start[j]));
                }
            }
            return output;
        }
        */
    }
}
