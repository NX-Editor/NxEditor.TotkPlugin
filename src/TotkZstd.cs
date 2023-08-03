using NxEditor.PluginBase.Models;
using NxEditor.PluginBase.Services;
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
    private static Decompressor _commonDecompressor = new();
    private static Decompressor _bcettDecompressor = new();
    private static Decompressor _packDecompressor = new();

    static TotkZstd()
    {
        byte[] zsDicPack = File.ReadAllBytes(Path.Combine(TotkConfig.Shared.GamePath, "Pack", "ZsDic.pack.zs"));
        zsDicPack = _defaultDecompressor.Unwrap(zsDicPack).ToArray();
        SarcFile sarc = SarcFile.FromBinary(zsDicPack);

        _commonDecompressor.LoadDictionary(sarc["zs.zsdic"]);
        _bcettDecompressor.LoadDictionary(sarc["bcett.byml.zsdic"]);
        _packDecompressor.LoadDictionary(sarc["pack.zsdic"]);

        _commonCompressor.LoadDictionary(sarc["zs.zsdic"]);
        _bcettCompressor.LoadDictionary(sarc["bcett.byml.zsdic"]);
        _packCompressor.LoadDictionary(sarc["pack.zsdic"]);
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
        handle.Data = (handle.Name.EndsWith(".bcett.byml")
            ? _bcettDecompressor.Unwrap(handle.Data) : handle.Name.EndsWith(".pack")
            ? _packDecompressor.Unwrap(handle.Data) : handle.Name.EndsWith(".rsizetable")
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
