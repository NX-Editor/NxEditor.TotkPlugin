using NxEditor.PluginBase.Models;
using NxEditor.PluginBase.Services;
using Revrs;
using SarcLibrary;
using ZstdSharp;

namespace NxEditor.TotkPlugin;

public class TotkZstd : IProcessingService
{
    private static readonly int _level = Convert.ToInt32(TotkConfig.Shared.ZstdCompressionLevel);

    private static Compressor _defaultCompressor = new(_level);
    private static Compressor _commonCompressor = new(_level);
    private static Compressor _bcettCompressor = new(_level);
    private static Compressor _packCompressor = new(_level);

    private static readonly Decompressor _defaultDecompressor = new();
    private static readonly Decompressor _commonDecompressor = new();
    private static readonly Decompressor _bcettDecompressor = new();
    private static readonly Decompressor _packDecompressor = new();

    static TotkZstd()
    {
        Span<byte> zsDicPack = _defaultDecompressor.Unwrap(
            File.ReadAllBytes(Path.Combine(TotkConfig.Shared.GamePath, "Pack", "ZsDic.pack.zs"))
        );

        RevrsReader reader = new(zsDicPack);
        ImmutableSarc sarc = new(ref reader);

        _commonDecompressor.LoadDictionary(sarc["zs.zsdic"].Data);
        _bcettDecompressor.LoadDictionary(sarc["bcett.byml.zsdic"].Data);
        _packDecompressor.LoadDictionary(sarc["pack.zsdic"].Data);

        _commonCompressor.LoadDictionary(sarc["zs.zsdic"].Data);
        _bcettCompressor.LoadDictionary(sarc["bcett.byml.zsdic"].Data);
        _packCompressor.LoadDictionary(sarc["pack.zsdic"].Data);
    }

    public static void ChangeCompressionLevel(int level)
    {
        _defaultCompressor = new(level);
        _commonCompressor = new(level);
        _bcettCompressor = new(level);
        _packCompressor = new(level);
    }

    public IFileHandle Process(IFileHandle handle)
    {
        handle.Data = (handle.Name.EndsWith(".bcett.byml.zs")
            ? _bcettDecompressor.Unwrap(handle.Data) : handle.Name.EndsWith(".pack.zs")
            ? _packDecompressor.Unwrap(handle.Data) : handle.Name.EndsWith(".rsizetable.zs")
            ? _defaultDecompressor.Unwrap(handle.Data) : _commonDecompressor.Unwrap(handle.Data)).ToArray();

        return handle;
    }

    public IFileHandle Reprocess(IFileHandle handle)
    {
        handle.Data = (handle.Name.EndsWith(".bcett.byml.zs")
            ? _bcettCompressor.Wrap(handle.Data) : handle.Name.EndsWith(".pack.zs")
            ? _packCompressor.Wrap(handle.Data) : handle.Name.EndsWith(".rsizetable.zs")
            ? _defaultCompressor.Wrap(handle.Data) : _commonCompressor.Wrap(handle.Data)).ToArray();

        return handle;
    }

    public bool IsValid(IFileHandle handle)
    {
        return handle.Name.EndsWith(".zs");
    }
}
