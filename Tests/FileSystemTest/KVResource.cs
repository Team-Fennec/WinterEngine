using WinterEngine.Resource;
using ValveKeyValue;

namespace FileSystemTest;

public sealed class KVTestRes : IResource
{
    public string Apple;
    public string Tomato;

    public void LoadData(Stream stream)
    {
        // read KVData
        var kv = KVSerializer.Create(KVSerializationFormat.KeyValues1Text);
        KVObject TestData = kv.Deserialize(stream);

        Apple = TestData["apple"].ToString();
        Tomato = TestData["tomato"].ToString();
        stream.Close();
    }
}
