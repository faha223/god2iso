using System.Security.Cryptography;

namespace libGod2Iso;

internal static class Resources
{
    private static Stream LoadResourceStream(string resourceName){
        Console.WriteLine(string.Join(Environment.NewLine, typeof(Resources).Assembly.GetManifestResourceNames()));
        return typeof(Resources).Assembly.GetManifestResourceStream(resourceName) ??
            throw new Exception("Resource Not Found: " + resourceName);
    }
        

    private static byte[] LoadResource(string resourceName)
    {
        var stream = LoadResourceStream("libGod2Iso.Resources." + resourceName);
        var buffer = new byte[stream.Length];
        stream.ReadExactly(buffer);
        return buffer;
    }


    private static readonly byte[] _xsfHeader = LoadResource("XSFHeader.bin");
    public static byte[] XSFHeader
    {
        get {
            var hash = SHA256.HashData(_xsfHeader);
            if(!hash.SequenceEqual(XSFHeaderSha256))
                throw new Exception("Resource XSFHeader.bin has incorrect hash");
            return _xsfHeader;
        }
    }

    private static readonly byte[] XSFHeaderSha256 = [
        0x78, 0x98, 0xe6, 0x46, 0xe7, 0xf6, 0xc9, 0xbd,
        0x9c, 0x52, 0x10, 0xd4, 0x4d, 0xf0, 0x7d, 0xe4,
        0xc4, 0x68, 0xa1, 0xd6, 0xa8, 0xcd, 0xa4, 0x95, 
        0x4c, 0x8f, 0x59, 0xbe, 0xd1, 0x92, 0x05, 0x37
    ];
}