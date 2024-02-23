using NxEditor.PluginBase.Models;
using NxEditor.PluginBase.Services;
using Revrs;
using Revrs.Extensions;
using SarcLibrary;
using System.Buffers;
using System.Diagnostics;
using ZstdSharp;

namespace NxEditor.TotkPlugin;

public class TotkZstd : ITransformer
{
    private const uint ZSTD_MAGIC = 0xFD2FB528;
    private static readonly Dictionary<string, int> _cache = [];

    private static readonly Decompressor _defaultDecompressor = new();
    private static readonly Dictionary<int, Decompressor> _decompressors = [];

    private static readonly Compressor _defaultCompressor = new();
    private static readonly Dictionary<int, Compressor> _compressors = [];

    public static void Reload(string zsDicPath)
    {
        if (!File.Exists(zsDicPath)) {
            Trace.WriteLine("[Info] Dictionaries not found, skipping...");
            return;
        }

        using FileStream fs = File.OpenRead(zsDicPath);
        byte[] buffer = ArrayPool<byte>.Shared.Rent((int)fs.Length);
        int size = fs.Read(buffer);

        if (buffer.Length < 5) {
            Trace.WriteLine("[Info] Invalid dictionary pack, skipping...");
            return;
        }

        Span<byte> data = _defaultDecompressor.Unwrap(buffer.AsSpan()[..size]);
        RevrsReader reader = new(data);
        ImmutableSarc sarc = new(ref reader);

        foreach ((var _, var fileData) in sarc) {
            int id = fileData[4..8].Read<int>();

            Decompressor decompressor = new();
            decompressor.LoadDictionary(fileData);
            _decompressors[id] = decompressor;

            Compressor compressor = new();
            compressor.LoadDictionary(fileData);
            _compressors[id] = compressor;
        }
    }

    public static void SetLevel(int level)
    {
        _defaultCompressor.Level = level;
        foreach (var (_, compressor) in _compressors) {
            compressor.Level = level;
        }
    }

    public void TransformSource(IEditorFile handle)
    {
        Span<byte> buffer = handle.Source.AsSpan();
        if (buffer.Length < 5 || buffer.Read<uint>() != ZSTD_MAGIC) {
            return;
        }

        int id = GetDictionaryId(buffer);

        if (id > -1 && _decompressors.TryGetValue(id, out Decompressor? decompressor)) {
            _cache[handle.Id] = id;
            handle.Source = decompressor.Unwrap(buffer).ToArray();
            return;
        }

        lock (_defaultDecompressor) {
            handle.Source = _defaultDecompressor.Unwrap(buffer).ToArray();
        }
    }

    public void Transform(ref Span<byte> data, IEditorFile handle)
    {
        if (_cache.TryGetValue(handle.Id, out int id) && _compressors.TryGetValue(id, out Compressor? compressor)) {
            data = compressor.Wrap(data);
            return;
        }

        data = _defaultCompressor.Wrap(data);
    }

    public bool IsValid(IEditorFile handle)
    {
        return handle.Source.AsSpan().Read<uint>() == ZSTD_MAGIC;
    }

    private static int GetDictionaryId(Span<byte> buffer)
    {
        byte descriptor = buffer[4];
        int windowDescriptorSize = ((descriptor & 0b00100000) >> 5) ^ 0b1;
        int dictionaryIdFlag = descriptor & 0b00000011;

        return dictionaryIdFlag switch {
            0x0 => -1,
            0x1 => buffer[5 + windowDescriptorSize],
            0x2 => buffer[(5 + windowDescriptorSize)..].Read<short>(),
            0x3 => buffer[(5 + windowDescriptorSize)..].Read<int>(),
            _ => throw new OverflowException("""
                Two bits cannot exceed 0x3, something terrible has happened!
                """)
        };
    }
}
