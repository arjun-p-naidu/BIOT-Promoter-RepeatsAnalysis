using System.Text.RegularExpressions;


namespace RyhaanSeq.Model.Factory
{
    internal class GRCh38
    {
        List<string> Name = new List<string>();
        List<string> GRCh38_seq = new List<string>();
        List<int> Len = new List<int>();

        public GRCh38()
        {

        }
        public void read()
        {
            string[] files = Directory.GetFiles(@"Data\hg38_split\");
            foreach (string file in files)
            {
                string GRCh38Path = Path.GetFullPath(file);
                string read = read = File.ReadAllText(GRCh38Path);
                string GRCh38_seq = Regex.Replace(read, @"\r\n?|\n", "");
                int len = GRCh38_seq.Length;
                string name = Path.GetFileName(file).Substring(0, Path.GetFileName(file).Length-4); ;

                this.Name.Add(name);
                this.GRCh38_seq.Add(GRCh38_seq);
                this.Len.Add(len);
            }
        }
        public List<string> returnGeneSeq(string name, Promoters promoter, int i, int j)
        {
            List<string> output = new List<string>();

            for (; j <= i; j++)
            {
                if (promoter.End[j] - promoter.Start[j] > 5)
                {
                    output.Add(this.GRCh38_seq[this.Name.IndexOf(name)].Substring(promoter.Start[j], promoter.End[j] - promoter.Start[j]));
                }
            }
            return output;
        }
        public List<string> returnGeneSeq(string name, Peak peak, int i, int j)
        {
            List<string> output = new List<String>();
            int file = this.Name.IndexOf(name);
            if (file != -1)
            {
                for (; j <= i; j++)
                {
                    output.Add(this.GRCh38_seq[file].Substring(peak.Start[j], peak.End[j] - peak.Start[j]));
                    //Console.WriteLine(this.GRCh38_seq[file].Substring(peak.Start[j], peak.End[j] - peak.Start[j]));
                }
            }
            else
            {
                for (; j <= i; j++)
                {
                    Console.WriteLine(name);
                    output.Add("un-id");
                }
            }
            return output;
        }

        public void free()
        {
            this.Name.RemoveAll(x => x != null);
            this.GRCh38_seq.RemoveAll(x => x != null);
            this.Len.RemoveAll(x => x != null);
        }
    }
}
