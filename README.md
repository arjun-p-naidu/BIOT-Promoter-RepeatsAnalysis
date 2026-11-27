# BIOT-Promoter-RepeatsAnalysis

A C# / .NET pipeline for exploring the sequence context of human promoters on the **GRCh38 (hg38)** reference genome. It finds:

- **simple tandem repeats** near each promoter,
- **ChIP-seq peaks** that overlap promoters across several ENCODE cell lines,
- **transcription-factor (TF) binding motifs** inside promoter sequences, using fuzzy matching on IUPAC-degenerate motifs,
- **k-mers shared** between TF motifs and nearby repeats, using a 2-bit encoded substring hash.

Interval filtering (promoter ↔ repeat, promoter ↔ peak) runs as GPU kernels written in C# with [ILGPU](https://github.com/m4rs-mt/ILGPU) on a CUDA device. Sequence processing runs on the CPU with `Parallel.For`.

---

## What it does

| Stage | Where | Description |
|---|---|---|
| Load genome | `Factory/GRCh38.cs` | Reads per-chromosome hg38 files from `Data/hg38_split/` into memory (newlines removed). |
| Load promoters | `Reader.cs → readPromoters()` | Reads promoter coordinates and widens each one to **TSS −200 / +50 bp**, then pulls out its sequence (`Promoters.promSeq`). |
| Duplicate promoters | `SeqFn.seqSimilar()` | Groups promoters by length and collects those with identical sequences. |
| Repeats near promoters | `Factory/SimpleRepeat.cs`, `GPUParallel/GPUSReapeatFilter.cs` | Loads the UCSC `simpleRepeat` (Tandem Repeats Finder) track and, on the GPU, finds repeats whose end falls within **[promoter start − 500, promoter end + 100]**. |
| Peaks over promoters | `Factory/Peak.cs`, `GPUParallel/GPUPeakCommon.cs`, `SeqFn.peakSimilar()` | Loads ChIP-seq peak files for **K562, HepG2, HUVEC, H1-hESC and HeLa-S3**, pulls out peak sequences, and finds peaks that contain a promoter or whose sequence exactly matches one. |
| TF motif matching | `Factory/TranscriptionFactor.cs` | Reads TF consensus motifs (Cistrome SELEX table) and ChIP-Atlas peaks. Expands IUPAC codes (R, Y, M, K, S, W, H, B, V, D, N) and scans promoter sequences with a mismatch-tolerant state machine (≈90% threshold). |
| k-mer comparison | `Factory/TrackingHash.cs`, `Processes.tfSrSeqMatch*()` | Counts all substrings up to length 9 as 2-bit hashes (A=0, T=1, G=2, C=3) for every TF motif and repeat, then reports k-mers they share plus the most frequent and longest k-mers. |

The entry point (`Program.cs`) currently runs the **TF ↔ simple-repeat k-mer analysis** (`Processes.tfSrSeqMatchMaxQuery`). The other pipelines (`PromoterRepeatProcess`, `PeakProcess`, `PeakProcess2`) are present but commented out. Uncomment the one you want to run.

## Project layout

```
Model/
├── Program.cs              # entry point; choose which pipeline to run
├── Processes.cs            # end-to-end pipelines (repeats, peaks, TF ↔ repeat)
├── Reader.cs               # file parsers (promoters, peaks, TF tables)
├── Writer.cs               # result writers (prints clickable file links)
├── SeqFn.cs                # sequence grouping and exact-match helpers
├── Display.cs              # console output helpers
├── Factory/                # data models
│   ├── GRCh38.cs           # in-memory reference genome
│   ├── Promoters.cs
│   ├── SimpleRepeat.cs
│   ├── Peak.cs
│   ├── TranscriptionFactor.cs
│   ├── TrackingHash.cs     # 2-bit k-mer hashing
│   └── ResultHash.cs, SRepeatsFilter.cs, PeakCommon.cs
└── GPUParallel/            # ILGPU kernels (CUDA backend)
    ├── GPUSReapeatFilter.cs
    └── GPUPeakCommon.cs
```

## Requirements

- **.NET 6 or later** (the code uses `MaxBy` and top-level `async Main`)
- **ILGPU** NuGet package: `dotnet add package ILGPU`
- An **NVIDIA GPU** with a CUDA driver (the kernels call `CreateCudaAccelerator(0)`)
- **Windows**: file paths are hard-coded with `\` separators
- Enough RAM to hold the full hg38 genome as strings (~3 GB of sequence; plan for 8–16 GB)

## Data

Input files are not included in the repo because of their size. Place them under `Data/` in the working directory:

```
Data/
├── hg38_split/                         # one file per chromosome, e.g. chr1.txt (raw sequence)
├── promotersHg38.txt                   # promoter BED-like file: chr, start, end, id (tab-separated)
├── simpleRepeat.txt                    # UCSC hg38 simpleRepeat table (TRF)
├── peakK562/  peakHepG2/  peakHUVEC-C/  peakH1-hESC/  peakHeLa-S3/   # ChIP-seq peak files per cell line
├── maxQuery/                           # peak set for PeakProcess2
├── cistrome_SELEX_consensus_table.txt  # TF consensus motifs
└── Oth.ALL.05.AllAg.AllCell.txt        # ChIP-Atlas peaks (all antigens, all cells, q < 1e-5)
```

Sources:
- hg38 genome and `simpleRepeat`: [UCSC Genome Browser downloads](https://hgdownload.soe.ucsc.edu/downloads.html#human)
- Cell-line ChIP-seq peaks: [ENCODE](https://www.encodeproject.org/)
- TF peaks: [ChIP-Atlas](https://chip-atlas.org/)
- TF motifs: Cistrome SELEX consensus table

## Running

```bash
cd Model
dotnet restore
dotnet run -c Release
```

Results are written under `Data/` (for example `Data/TFSRMatchResultMaxQuery/`, `Data/TfMatchMaxQuery+/`, `Data/Result_hg38.txt`, `Data/ResultPeak_*.txt`). The console prints a clickable link to each output file.

## Known limitations / roadmap

- **Interval search is brute force.** Each GPU kernel compares every promoter with every repeat or peak, which is O(P × R) work. Sorting the intervals by chromosome and position and using binary search (or an interval tree / sweep line) would make this O((P + R) log R) and fast enough on a CPU. This is the planned replacement.
- The interval kernels **don't check chromosome**: they compare coordinates only, so they can match features on different chromosomes.
- Kernel output rows have a fixed width (500 columns for repeats, 10 for peaks). Rows with more hits than that overflow, and the hit-collection loops skip the last hit.
- Paths are Windows-only and inputs are hard-coded. A CLI and config file are planned.
- `TrackingHash` shifts the hash once for each possible base of a degenerate IUPAC code, so k-mers containing degenerate bases produce inconsistent hashes.
