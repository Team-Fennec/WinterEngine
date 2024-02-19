using System.IO.Compression;
using System.IO;

public class SnowPack {
    public int ident = 4935763; // magic number
    public int version = 1; // should be 1
    public byte[] zipData;
}

public class Program {
    public static void Main(string[] args) {
        Console.WriteLine("SnowPack Tool");

        if (args.Length == 0) {
            Console.WriteLine("-x <filename> : extract a spk file");
            Console.WriteLine("-p <foldername> : compress a folder to a spk file");
            return;
        }

        if (args[0] == "-x" || File.Exists(args[0])) {
            string fileName;
            if (args.Length < 2) {
                fileName = args[0];
            } else {
                fileName = args[1];
            }

            SnowPack packData = new SnowPack();
            FileStream packStream = File.OpenRead(fileName);
            byte[] _buf = new byte[sizeof(int)];
            packStream.Read(_buf, 0, sizeof(int));

            if (BitConverter.ToInt32(_buf, 0) != packData.ident) {
                Console.WriteLine("Invalid file, not an SPK file");
                return;
            }

            packStream.Read(_buf, 0, sizeof(int));

            if (BitConverter.ToInt32(_buf, 0) > packData.version) {
                Console.WriteLine("Invalid file, version is too new");
                return;
            }
            packData.zipData = new byte[packStream.Length - packStream.Position];

            packStream.Read(packData.zipData, 0, (int)(packStream.Length - packStream.Position));
            packStream.Close();

            MemoryStream memStr = new MemoryStream(packData.zipData);
            ZipFile.ExtractToDirectory(memStr, "extracted_spk", null, true);
            Console.WriteLine("Extracted SnowPack file");
        } else if (args[0] == "-p" || Directory.Exists(args[0])) {
            string fileName;
            if (args.Length < 2) {
                fileName = args[0];
            } else {
                fileName = args[1];
            }

            SnowPack packData = new SnowPack();
            MemoryStream dataStr = new MemoryStream();

            if (!Directory.Exists(fileName)) {
                Console.WriteLine("Directory not found");
                return;
            }

            ZipFile.CreateFromDirectory(fileName, dataStr);
            packData.zipData = dataStr.GetBuffer();

            FileStream file = File.OpenWrite($"{fileName}.spk");
            file.Write(BitConverter.GetBytes(packData.ident));
            file.Write(BitConverter.GetBytes(packData.version));
            file.Write(packData.zipData);
            file.Flush();
            file.Close();

            Console.WriteLine("Created SnowPack file");
        } else {
            Console.WriteLine("-x <filename> : extract a spk file");
            Console.WriteLine("-p <foldername> : compress a folder to a spk file");
            return;
        }
    }
}
