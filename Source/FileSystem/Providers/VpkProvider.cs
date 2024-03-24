using libvpkedit;

namespace WinterEngine.Resource.Providers;

public sealed class VpkProvider : ResourceProvider
{
    PackFile m_Pack;

    public VpkProvider(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Unable to locate VPK file {path}");
        }

        m_Pack = PackFile.Open(path);
        if (m_Pack == null)
        {
            throw new FileLoadException($"Failed to load VPK file {path}");
        }
        if (m_Pack.Type != PackFileType.VPK)
        {
            // I know we technically support more than vpk but this is vpk provider.
            throw new InvalidDataException("Provided pack file was not a VPK file");
        }
    }

    public override bool DirExists(string dirPath)
    {
        // we can't determine this, just say true
        return true;
    }

    public override bool FileExists(string filePath)
    {
        return (m_Pack.FindEntry(filePath) != null);
    }

    public override Stream? OpenFile(string filePath)
    {
        if (!FileExists(filePath))
        {
            return null;
        }

#pragma warning disable CS8604
        MemoryStream entryData = new MemoryStream(m_Pack.ReadEntry(m_Pack.FindEntry(filePath)));
#pragma warning restore

        return entryData;
    }
}
