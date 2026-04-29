using RyhaanSeq.Model.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RyhaanSeq.Model
{
    internal static class Processes
    {
        public static void PeakProcess(GRCh38 gRCh38, Promoters promoters)
        {
            List<Peak> peakK562 = new List<Peak>();
            List<Peak> peakHepG2 = new List<Peak>();
            List<Peak> peakHUVEC = new List<Peak>();
            List<Peak> peakH1hESC = new List<Peak>();
            List<Peak> peakHeLaS3 = new List<Peak>();

            string peakK562_file = @"Data\peakK562";
            string peakHepG2_file = @"Data\peakHepG2";
            string peakHUVEC_file = @"Data\peakHUVEC-C";
            string peakH1hESC_file = @"Data\peakH1-hESC";
            string peakHeLaS3_file = @"Data\peakHeLa-S3";

            ParallelOptions options = new ParallelOptions
            {
                MaxDegreeOfParallelism = 8
            };

            Read.readPeaks(peakK562, peakK562_file, gRCh38, options);
            Read.readPeaks(peakHepG2, peakHepG2_file, gRCh38, options);
            Read.readPeaks(peakHUVEC, peakHUVEC_file, gRCh38, options);
            Read.readPeaks(peakH1hESC, peakH1hESC_file, gRCh38, options);
            Read.readPeaks(peakHeLaS3, peakHeLaS3_file, gRCh38, options);

            peakK562_file = @"PeakResult" + "peakK562";
            peakHepG2_file = @"PeakResult" + "peakHepG2";
            peakHUVEC_file = @"PeakResult" + "peakHUVEC-C";
            peakH1hESC_file = @"PeakResult" + "peakH1-hESC";
            peakHeLaS3_file = @"PeakResult" + "peakHeLa-S3";

            List<List<int>> LenSortedPromoter = new List<List<int>>();
            LenSortedPromoter = SeqFn.lenSortedPromoterFn(promoters);

            string file_peakK562 = @"Data\ResultPeak_peakK562.txt";
            using (FileStream fs = File.Create(file_peakK562)) { }


            Read.readPeakSeq2(peakK562_file, file_peakK562, LenSortedPromoter, promoters);

            string file_peakHepG2 = @"Data\ResultPeak_peakHepG2.txt";
            using (FileStream fs = File.Create(file_peakHepG2)) { }

            Read.readPeakSeq2(peakHepG2_file, file_peakHepG2, LenSortedPromoter, promoters);

            string file_peakHUVEC = @"Data\ResultPeak_HUVEC-C.txt";
            using (FileStream fs = File.Create(file_peakHUVEC)) { }


            Read.readPeakSeq2(peakHUVEC_file, file_peakHUVEC, LenSortedPromoter, promoters);

            string file_peakH1hESC = @"Data\ResultPeak_peakH1-hESC.txt";
            using (FileStream fs = File.Create(file_peakH1hESC)) { }

            Read.readPeakSeq2(peakH1hESC_file, file_peakH1hESC, LenSortedPromoter, promoters);

            string file_peakHeLaS3 = @"Data\ResultPeak_peakHeLa-S3.txt";
            using (FileStream fs = File.Create(file_peakHeLaS3)) { }

            Read.readPeakSeq2(peakHeLaS3_file, file_peakHeLaS3, LenSortedPromoter, promoters);
        }
        public static void PromoterRepeatProcess(GRCh38 gRCh38)
        {
            int terminalWidth = Console.WindowWidth;
            string div = new string('-', terminalWidth);

            SimpleRepeat sRepeat = new SimpleRepeat();

            Promoters promoters_hg38 = new Promoters();
            Promoters promoters_hg19mm = new Promoters();
            SRepeatsFilter strMatch_hg38 = new SRepeatsFilter();
            SRepeatsFilter strMatch_hg19mm = new SRepeatsFilter();

            Promoters newPromoter_hg38 = new Promoters();
            Promoters newPromoter_hg19mm = new Promoters();
            SRepeatsFilter newStrMatch_hg38 = new SRepeatsFilter();
            SRepeatsFilter newStrMatch_hg19mm = new SRepeatsFilter();

            promoters_hg38 = Read.readPromoters();
            promoters_hg38 = Read.readPromoters_hg38();
            promoters_hg19mm = Read.readPromoters_hg19mm();
            sRepeat.read();

            sRepeat.doGPUmatch(promoters_hg38, strMatch_hg38);
            //sRepeat.doGPUmatch(promoters_hg19mm, strMatch_hg19mm);

            promoters_hg38.promSeq(gRCh38);
            //promoters_hg19mm.Promoter_seq = SeqFn.promSeq(promoters_hg19mm);

            newPromoter_hg38 = SeqFn.seqSimilar(promoters_hg38);
            //newPromoter_hg19mm = SeqFn.seqSimilar(promoters_hg19mm);

            sRepeat.doGPUmatch(newPromoter_hg38, newStrMatch_hg38);
            //sRepeat.doGPUmatch(newPromoter_hg19mm, newStrMatch_hg19mm);

            string hg38_result_file = @"Data\Result_hg38.txt";
            string hg19mm_result_file = @"Data\Result_19mm.txt";

            Writer.write(strMatch_hg38.output, div, hg38_result_file);
            //Writer.write(strMatch_hg19mm.output, div, hg19mm_result_file);

            string hg38_resultRepeats_file = @"Data\ResultRepeats_hg38.txt";
            string hg19mm_resultRepeats_file = @"Data\ResultRepeats_19mm.txt";

            Writer.writeRepeat(strMatch_hg38, div, hg38_resultRepeats_file);
            //Writer.writeRepeat(strMatch_hg19mm, div, hg19mm_resultRepeats_file);

            string hg38_file = @"Data\newPromoters_hg38.txt";
            string hg19mm_file = @"Data\newPromoters_hg19mm.txt";

            Writer.write(newPromoter_hg38, hg38_file);
            //Writer.write(newPromoter_hg19mm, hg19mm_file);

            string hg19mm_bigResult_file = @"Data\BigResult_hg19mm.txt";
            string hg38_bigResult_file = @"Data\BigResult_hg38.txt";

            //Writer.writeSimProRep(newPromoter_hg19mm, newStrMatch_hg19mm.output,hg19mm_bigResult_file);
            Writer.writeSimProRep(newPromoter_hg38, newStrMatch_hg38.output, hg38_bigResult_file);

        }


    }
}
