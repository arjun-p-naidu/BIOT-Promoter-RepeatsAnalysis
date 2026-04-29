using RyhaanSeq.Model;
using RyhaanSeq.Model.Factory;

class Program
{
    static async Task Main()
    {
        GRCh38 gRCh38 = new GRCh38();
        gRCh38.read();

        Promoters promoters = new Promoters();
        promoters = SeqFn.seqSimilar(Read.readPromoters());
        promoters.promSeq(gRCh38);
        gRCh38.free();
        

        TranscriptionFactor tf = new TranscriptionFactor();

        string file = @"Data\Tfs";
         tf.read(file);
        tf.match(promoters.Promoter_seq);
        //Processes.PeakProcess(gRCh38, promoters);
        //Processes.PromoterRepeatProcess(gRCh38);
    }
}

