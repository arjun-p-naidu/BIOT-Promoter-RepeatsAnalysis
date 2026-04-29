using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RyhaanSeq.Model.Factory
{
    internal class TranscriptionFactor
    {
        public List<string> tfName = new List<string>();
        public List<string> tfSeq = new List<string>();
        public List<List<string>> tfSeqVariants = new List<List<string>>();
        public List<string> tfPeak = new List<string>();
        public List<int> tfPeakStart = new List<int>();
        public List<int> tfPeakEnd = new List<int>();
        public List<string> tfPeakSeq = new List<string>();
        public List<string> peakPromoterChr = new List<string>();
        public List<string> peakPromoterID = new List<string>();
        public List<int> peakPromoterStart = new List<int>();
        public List<int> peakPromoterEnd = new List<int>();
        public List<string> peakPromoterSeq = new List<string>();

        public void read(string path)
        {
            string dest_path = @"Data\TfMatch"; 
            string[] files = Directory.GetFiles(path);
            List<Peak> peakList = new List<Peak>();
            int line_no = 1;
            foreach (string file in files)
            {
                string[] Lines = File.ReadAllLines(file);
                foreach (string line in Lines)
                {
                    
                        char[] delimiters = { ' ' };
                        string[] column = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        if (line_no == 1)
                        {
                            int start = int.Parse(column[2]);
                            int end = int.Parse(column[3]);
                            read(column[0], column[1], start, end, column[4]);
                            line_no++;
                        }
                        else
                        {
                            int start = int.Parse(column[2]);
                            int end = int.Parse(column[3]);
                            read(column[0], column[1], start, end, column[4], column[5]);
                            line_no--;
                        }
                    
                    /*
                    catch (Exception)
                    {
                        Console.WriteLine(file);
                        break;
                    }
                    */
                }
            }
        }
        private string stringChange(char c, string s, int i)
        {
            string str = s.Substring(0, i) + c + s.Substring(i + 1);
            return str; 
        }
        private List<string> GenerateVariants(string tfSeq)
        {
            List<string> tfSeqVariants = new List<string> { tfSeq };
            List<int> remove = new List<int>();
            for (int i = 0; i < tfSeq.Length; i++)
            {
                List<string> newTfSeqVariants = new List<string>();
                for (int j = 0; j < tfSeqVariants.Count; j++)
                {
                    switch (tfSeq[i])
                    {
                        case 'R':
                        case 'r':
                            newTfSeqVariants.Add(stringChange('G', tfSeqVariants[j], i));
                            newTfSeqVariants.Add(stringChange('A', tfSeqVariants[j], i));
                            break;
                        case 'y':
                        case 'Y':
                            newTfSeqVariants.Add(stringChange('T', tfSeqVariants[j], i));
                            newTfSeqVariants.Add(stringChange('C', tfSeqVariants[j], i));
                            break;
                        case 'M':
                        case 'm':
                            newTfSeqVariants.Add(stringChange('A', tfSeqVariants[j], i));
                            newTfSeqVariants.Add(stringChange('C', tfSeqVariants[j], i));
                            break;
                        case 'k':
                        case 'K':
                            newTfSeqVariants.Add(stringChange('G', tfSeqVariants[j], i));
                            newTfSeqVariants.Add(stringChange('T', tfSeqVariants[j], i));
                            break;
                        case 'S':
                        case 's':
                            newTfSeqVariants.Add(stringChange('G', tfSeqVariants[j], i));
                            newTfSeqVariants.Add(stringChange('C', tfSeqVariants[j], i));
                            break;
                        case 'w':
                        case 'W':
                            newTfSeqVariants.Add(stringChange('T', tfSeqVariants[j], i));
                            newTfSeqVariants.Add(stringChange('A', tfSeqVariants[j], i));
                            break;
                        case 'H':
                        case 'h':
                            newTfSeqVariants.Add(stringChange('A', tfSeqVariants[j], i));
                            newTfSeqVariants.Add(stringChange('C', tfSeqVariants[j], i));
                            newTfSeqVariants.Add(stringChange('T', tfSeqVariants[j], i));
                            break;
                        case 'B':
                        case 'b':
                            newTfSeqVariants.Add(stringChange('G', tfSeqVariants[j], i));
                            newTfSeqVariants.Add(stringChange('C', tfSeqVariants[j], i));
                            newTfSeqVariants.Add(stringChange('T', tfSeqVariants[j], i));
                            break;
                        case 'v':
                        case 'V':
                            newTfSeqVariants.Add(stringChange('G', tfSeqVariants[j], i));
                            newTfSeqVariants.Add(stringChange('A', tfSeqVariants[j], i));
                            newTfSeqVariants.Add(stringChange('C', tfSeqVariants[j], i));
                            break;
                        case 'D':
                        case 'd':
                            newTfSeqVariants.Add(stringChange('A', tfSeqVariants[j], i));
                            newTfSeqVariants.Add(stringChange('G', tfSeqVariants[j], i));
                            newTfSeqVariants.Add(stringChange('T', tfSeqVariants[j], i));
                            break;
                        case 'n':
                        case 'N':
                            newTfSeqVariants.Add(stringChange('G', tfSeqVariants[j], i));
                            newTfSeqVariants.Add(stringChange('C', tfSeqVariants[j], i));
                            newTfSeqVariants.Add(stringChange('T', tfSeqVariants[j], i));
                            newTfSeqVariants.Add(stringChange('A', tfSeqVariants[j], i));
                            break;

                        default:
                            newTfSeqVariants.Add(tfSeqVariants[j]);
                            break;
                    }
                }
                tfSeqVariants.Clear();
                tfSeqVariants = newTfSeqVariants;
            }
            foreach(string s in tfSeqVariants)
            {
                Console.WriteLine(s);
            }
            return tfSeqVariants;
        }

        public List<char> GenerateVariants(char tfChar)
        {
            switch (tfChar)
            {
                case 'R':
                case 'r':
                    return new List<char> { 'G','A' };
                    break;
                case 'y':
                case 'Y':
                    return new List<char> { 'T', 'C' };
                    break;
                case 'M':
                case 'm':
                    return new List<char> { 'C', 'A' };
                    break;
                case 'k':
                case 'K':
                    return new List<char> { 'G', 'T' };
                    break;
                case 'S':
                case 's':
                    return new List<char> { 'G', 'C' };
                    break;
                case 'w':
                case 'W':
                    return new List<char> { 'T', 'A' };
                    break;
                case 'H':
                case 'h':
                    return new List<char> { 'C', 'A', 'T' };
                    break;
                case 'B':
                case 'b':
                    return new List<char> { 'C', 'G', 'T' };
                    break;
                case 'v':
                case 'V':
                    return new List<char> { 'C', 'A', 'G' };
                    break;
                case 'D':
                case 'd':
                    return new List<char> { 'G', 'A', 'T' };
                    break;
                case 'n':
                case 'N':
                    return new List<char> { 'C', 'A', 'T', 'g' };
                    break;

                default:
                    return new List<char> { tfChar };
                    break;
            }
        }

        public void match(string tfSeq, string promoterSeq, StreamWriter sw)
        {
            List<int> StatePointer = new List<int>();
            List<int> errorCount = new List<int>();
            int threshold = (int)(tfSeq.Length * 0.75);
            for (int c = 0; c < promoterSeq.Length; c++)
            {
                if (GenerateVariants(tfSeq[0]).Contains(promoterSeq[c]))
                {
                    StatePointer.Add(1);
                    errorCount.Add(1);
                }
                for(int i = 0; i < StatePointer.Count; i++)
                {
                    if (GenerateVariants(tfSeq[StatePointer[i]]).Contains(promoterSeq[c]))
                    {
                        StatePointer[i]++;
                        if (StatePointer[i] >= tfSeq.Length)
                        {
                            //Console.WriteLine("Match Found at promoter index " + c + " for string " + promoterSeq.Substring(c - tfSeq.Length,tfSeq.Length) + " with " + errorCount[i] + " calculate the percentage urself u lazy fuk");
                            sw.WriteLine(promoterSeq);
                            sw.WriteLine("------");
                            sw.WriteLine("Index = " + c );
                            //sw.WriteLine(promoterSeq.Substring(c - tfSeq.Length, tfSeq.Length) + " | " + tfSeq );
                            sw.WriteLine(tfSeq);
                            float Similarity = tfSeq.Length / (tfSeq.Length - errorCount[i]) * 100;
                            sw.WriteLine("Similarity = " + Similarity + "%");
                            sw.WriteLine("//////////////////////////////////////////////////");
                            StatePointer.RemoveAt(i);
                            errorCount.RemoveAt(i);
                            i--;
                        }
                    }
                    else
                    {
                        if (errorCount[i] < threshold)
                        {
                            errorCount[i]++;
                        }
                        else
                        {
                            StatePointer.RemoveAt(i);
                            errorCount.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
        }

        public void match(List<string> promoterSeq)
        {
            //for(int i = 0; i < this.tfSeq.Count; i++)
            //{
            //    match(this.tfSeq[i], this.peakPromoterSeq[i]);
            //}

            string dest_path = @"Data\TfMatch";
            Directory.CreateDirectory(dest_path);

            for(int i = 0; i < this.tfSeq.Count; i++)
            {
                string dest_file = @"Data\TfMatch\" + tfName[i] + ".txt";
                using (FileStream fs = File.Create(dest_file)) { }
                using (StreamWriter sw = new StreamWriter(dest_file))
                {
                    foreach (string promoter in promoterSeq)
                    {
                        match(tfSeq[i], promoter, sw);
                    }
                }
            }
        }

        public void read(string peakPromoterChr, string peakPromoterID, int peakPromoterStart, int peakPromoterEnd, string peakPromoterSeq)
        {
            this.peakPromoterChr.Add(peakPromoterChr);
            this.peakPromoterID.Add(peakPromoterID);
            this.peakPromoterStart.Add(peakPromoterStart);
            this.peakPromoterEnd.Add(peakPromoterEnd);
            this.peakPromoterSeq.Add(peakPromoterSeq);
        }
        public void read(string tfPeak, string tfName, int tfPeakStart, int tfPeakEnd, string tfSeq, string tfPeakSeq)
        {
            this.tfPeak.Add(tfPeak);
            this.tfName.Add(tfName);
            this.tfPeakStart.Add(tfPeakStart);
            this.tfPeakEnd.Add(tfPeakEnd);
            this.tfSeq.Add(tfSeq);
            this.tfPeakSeq.Add(tfPeakSeq);
            //this.tfSeqVariants.Add(GenerateVariants(tfSeq));
        }
        
    }
}
