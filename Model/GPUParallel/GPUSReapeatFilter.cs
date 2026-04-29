using ILGPU;
using ILGPU.Runtime;
using ILGPU.Runtime.Cuda;

namespace RyhaanSeq.Model.GPUParallel
{
    internal static class GPUSReapeatFilter
    {
        static void Kernel
            (Index1D i,
            ArrayView<int> promoter_start_buffer,
            ArrayView<int> promoter_end_buffer,
            ArrayView<int> sRepeats_end_buffer,
            ArrayView2D<int, Stride2D.DenseX> output)
        {
            int ptr = 1;
            for(int j = 0; j < sRepeats_end_buffer.Length; j++)
            {
                if (sRepeats_end_buffer[j] >= promoter_start_buffer[i] - 500 && sRepeats_end_buffer[j] <= promoter_end_buffer[i])
                {
                    output[i, ptr] = j;
                    ptr++;
                }
                output[i, 0] = ptr-1;
            }
        }

        public static int[,] SReapeatFilter(List<int> promoter_start,List<int> promoter_end,List<int> sRepeats_end)
        {
            Context context = Context.CreateDefault();
            Accelerator accelerator = context.CreateCudaAccelerator(0); 

            MemoryBuffer1D<int, Stride1D.Dense> promoter_start_buffer = accelerator.Allocate1D(promoter_start.ToArray());
            MemoryBuffer1D<int, Stride1D.Dense> promoter_end_buffer = accelerator.Allocate1D(promoter_end.ToArray());
            MemoryBuffer1D<int, Stride1D.Dense> sRepeats_end_buffer = accelerator.Allocate1D(sRepeats_end.ToArray());
            MemoryBuffer2D<int, Stride2D.DenseX> output = accelerator.Allocate2DDenseX<int>(new Index2D(promoter_start.Count,500));

            Action
                <Index1D,
                ArrayView<int>,
                ArrayView<int>,
                ArrayView<int>,
                ArrayView2D<int, Stride2D.DenseX>> loadedKernel = accelerator.LoadAutoGroupedStreamKernel
                <Index1D,
                ArrayView<int>,
                ArrayView<int>,
                ArrayView<int>,
                ArrayView2D<int, Stride2D.DenseX>>(Kernel);

            loadedKernel
                ((int)promoter_start.Count,
                promoter_start_buffer.View,
                promoter_end_buffer.View,
                sRepeats_end_buffer.View,
                output.View);

            accelerator.Synchronize();

            int[,] CPUOutput = output.GetAsArray2D();

            return CPUOutput;

            accelerator.Dispose();
            context.Dispose();
        }
    }
}
