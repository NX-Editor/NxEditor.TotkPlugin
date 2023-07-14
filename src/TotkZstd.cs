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

    static TotkZstd()
    {
        byte[] zsDicPack = File.ReadAllBytes(Path.Combine(TotkConfig.Shared.GamePath, "Pack", "ZsDic.pack.za"));
        zsDicPack = _defaultDecompressor.Unwrap(zsDicPack).ToArray();
        SarcFile sarc = SarcFile.FromBinary(zsDicPack);

        foreach ((_, var data) in sarc) {
            _defaultDecompressor.LoadDictionary(data);
        }

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
        handle.Data = _defaultDecompressor
            .Unwrap(handle.Data).ToArray();
        return handle;
    }

    public IFileHandle Reprocess(IFileHandle handle)
    {
        throw new NotImplementedException();
    }

    public bool IsValid(IFileHandle handle)
    {
        return handle.Name.EndsWith(".zs");
    }
}
