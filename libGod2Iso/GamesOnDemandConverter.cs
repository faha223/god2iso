namespace libGod2Iso;

public class GamesOnDemandConverter
{
    public delegate void ProgressChangedEvent(float packageProgress, float totalProgress);
    public event ProgressChangedEvent? ProgressChanged = null;

    private int _packagesCompleted;

    public string OutputDirectory { get;set; } = string.Empty;

    public bool CreateGoodIsoHeader { get;set; } = false;

    public List<string> Packages { get; init; } = [];
    public bool ConvertPackages()
    {
        if(string.IsNullOrWhiteSpace(OutputDirectory))
            return false;
        if(!Directory.Exists(OutputDirectory))
            return false;

        _packagesCompleted = 0;

        ProgressChanged?.Invoke(0, 0);
        foreach(var package in Packages)
        {
            if(!ConvertPackage(package))
                return false;
            _packagesCompleted++;
            ProgressChanged?.Invoke(1.0f, _packagesCompleted / (float)Packages.Count);
        }

        return true;
    }

    private bool ConvertPackage(string package)
    {
        FileStream? iso = null;
        FileStream? data = null;

        try
        {
            string baseName = Path.GetFileName(package);
            string dataPath = package + ".data";

            // count files
            int totalFiles;
            for (totalFiles = 0; true; totalFiles++) 
            {
                string path = Path.Combine(dataPath, "Data" + totalFiles.ToString("D4"));
                if (!File.Exists(path)) 
                    break;
            }

            if (totalFiles < 1) 
                return false;

            // check for xsf header
            bool hasXSF = HasXSFHeader(Path.Combine(dataPath, "Data0000"));

            // open new iso file
            iso = new FileStream(Path.Combine(OutputDirectory, baseName + ".iso"), FileMode.Create, FileAccess.ReadWrite);

            // add header, if needed
            if (!hasXSF) 
                iso.Write(Resources.XSFHeader, 0, Resources.XSFHeader.Length);

            // loop through data parts
            for (int i = 0; i < totalFiles; i++) {
                var packageProgress = i / (float)totalFiles;
                var overallProgress = (_packagesCompleted + packageProgress) / Packages.Count;
                ProgressChanged?.Invoke(packageProgress, overallProgress);
                string path = Path.Combine(dataPath, "Data" + i.ToString("D4"));
                data = new FileStream(path, FileMode.Open, FileAccess.Read)
                {
                    Position = 0x2000
                };

                int length = 0;
                while (true) {
                    byte[] buff = new byte[0xcc000];
                    length = data.Read(buff, 0, buff.Length);
                    iso.Write(buff, 0, length);
                    if (length < 0xcc000) break;
                    length = data.Read(buff, 0, 0x1000);
                    if (length < 0x1000) break;
                }
                data.Close();
                data = null;
            }

            if (!hasXSF) {
                FixXFSHeader(iso);
                FixSectorOffsets(iso, package);
            }
            if (CreateGoodIsoHeader) 
                FixCreateIsoGoodHeader(iso);
        }
        finally 
        {
            if (iso != null) try { iso.Close(); } catch (Exception) { }
            if (data != null) try { data.Close(); } catch (Exception) { }
        }

        return true;
    }

    public static bool ConvertPackage(string package, string outputDirectory, bool createGoodIsoHeader = false)
    {
        var godConverter = new GamesOnDemandConverter()
        {
            OutputDirectory = outputDirectory,
            Packages = [ package ], 
            CreateGoodIsoHeader = createGoodIsoHeader
        };

        return godConverter.ConvertPackages();
    }

    private static bool HasXSFHeader(string file)
    {
        byte[] buff = new byte[3];

        FileStream data = new (file, FileMode.Open, FileAccess.Read);
        data.Position = 0x2000;
        data.Read(buff, 0, buff.Length);
        data.Close();

        if (buff[0] != 0x58) return false;
        if (buff[1] != 0x53) return false;
        if (buff[2] != 0x46) return false;

        return true;
    }

    private static void FixSectorOffsets(FileStream iso, string godPath) 
    {
        int sector, size, offset;
        byte[] buffer;
        Queue<DirEntry> directories = new Queue<DirEntry>();

        buffer = File.ReadAllBytes(godPath);

        // offset type?
        if ((buffer[0x391] & 0x40) != 0x40)
            return;

        // calculate the offset
        offset = BitConverter.ToInt32(buffer, 0x395);
        if (offset == 0) 
            return;

        offset *= 2;
        offset -= 34;

        buffer = new byte[4];
        iso.Position = 0x10014;
        iso.Read(buffer, 0, 4);
        sector = BitConverter.ToInt32(buffer, 0);
        if (sector > 0)
        {
            sector -= offset;
            byte[] corrected = BitConverter.GetBytes(sector);
            iso.Position -= 4;
            iso.Write(corrected, 0, 4);
            iso.Read(buffer, 0, 4);
            size = BitConverter.ToInt32(buffer, 0);
            directories.Enqueue(new DirEntry(sector, size));
        }

        while (directories.Count > 0)
        {
            DirEntry dirEntry = directories.Dequeue();
            iso.Position = dirEntry.StartPos();

            while ((iso.Position + 4) < dirEntry.EndPos())
            {
                // crossed a sector boundary?
                if ((iso.Position + 4) / 2048L > iso.Position / 2048L)
                {
                    iso.Position += 2048L - (iso.Position % 2048L);
                }
            
                // read subtrees
                iso.Read(buffer, 0, 4);
                if (buffer[0] == 0xff && buffer[1] == 0xff && buffer[2] == 0xff && buffer[3] == 0xff)
                {
                    // another sector to process?
                    if (dirEntry.EndPos() - iso.Position > 2048) {
                        iso.Position += 2048L - (iso.Position % 2048L);
                        continue;
                    }
                    break;
                }

                // read sector
                iso.Read(buffer, 0, 4);
                sector = BitConverter.ToInt32(buffer, 0);
                if (sector > 0) {
                    sector -= offset;
                    byte[] corrected = BitConverter.GetBytes(sector);
                    iso.Position -= 4;
                    iso.Write(corrected, 0, 4);
                }

                // get size
                iso.Read(buffer, 0, 4);
                size = BitConverter.ToInt32(buffer, 0);

                // get attributes
                iso.Read(buffer, 0, 1);

                // if directory add to list of tables to process
                if ((buffer[0] & 0x10) == 0x10) 
                    directories.Enqueue(new DirEntry(sector, size));

                // get filename length
                iso.Read(buffer, 0, 1);
                // skip it
                iso.Position += buffer[0];

                // skip padding
                if ((14 + buffer[0]) % 4 > 0) 
                    iso.Position += 4 - ((14 + buffer[0]) % 4);
            }
        }
    }

    private static void FixCreateIsoGoodHeader(FileStream iso) 
    {
        byte[] bytes = new byte[8];
        iso.Position = 8;
        iso.Read(bytes, 0, 8);
        if (BitConverter.ToInt64(bytes, 0) == 2587648L) {
            iso.Position = 0;
            iso.Write(Resources.XSFHeader, 0, Resources.XSFHeader.Length);
            FixXFSHeader(iso);
        }
    }

    private static void FixXFSHeader(FileStream iso)
    {
        byte[] bytes;

        iso.Position = 8;
        bytes = BitConverter.GetBytes(iso.Length - 0x400);
        iso.Write(bytes, 0, bytes.Length);

        iso.Position = 0x8050;
        bytes = BitConverter.GetBytes((uint)(iso.Length / 2048));
        iso.Write(bytes, 0, bytes.Length);
        for (int i = bytes.Length - 1; i >= 0; i--) {
            iso.WriteByte(bytes[i]);
        }

        iso.Position = 0x7a69;
        bytes = System.Text.Encoding.ASCII.GetBytes(GetAppName());
        iso.Write(bytes, 0, bytes.Length);
    }

    private static string GetAppName() 
    {
        string s = "God2Iso v" + System.Reflection.Assembly.GetEntryAssembly()?.GetName()?.Version?.ToString() ??
            throw new Exception("Assembly Not Available");
        return s.Substring(0, s.Length - 2);
    }
}