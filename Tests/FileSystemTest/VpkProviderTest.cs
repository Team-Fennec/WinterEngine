using WinterEngine.Resource;
using WinterEngine.Resource.Providers;
using ValveKeyValue;
using XUnit.Project.Attributes;
using Xunit.Abstractions;

namespace FileSystemTest;

[TestCaseOrderer(
    ordererTypeName: "XUnit.Project.Orderers.PriorityOrderer",
    ordererAssemblyName: "FileSystemTest")]
public class VpkProviderTest
{
    private readonly ITestOutputHelper output;

    public VpkProviderTest(ITestOutputHelper helper)
    {
        output = helper;
    }

    [Fact, TestPriority(0)]
    public void AddProvider()
    {
        output.WriteLine("Adding 'testpak.vpk' with VpkProvider");
        ResourceManager.AddProvider(new VpkProvider("testpak.vpk"));
    }

    // the order of the others doesn't matter, we just need the provider added first.
    [Fact, TestPriority(1)]
    public void GetFileData()
    {
        output.WriteLine("Checking contents of 'packed_kv.txt'");
        Stream kvData = ResourceManager.GetData("packed_kv.txt");
        Assert.True(kvData != null);

        // read KVData
        var kv = KVSerializer.Create(KVSerializationFormat.KeyValues1Text);
        KVObject TestData = kv.Deserialize(kvData);
        kvData.Close();

        output.WriteLine($"Checking key 'apple':\n   expected: 'fruit'\n   got: {TestData["apple"]}");
        Assert.True(TestData["apple"].ToString() == "fruit");
        output.WriteLine($"Checking key 'tomato':\n   expected: 'vegetable'\n   got: {TestData["tomato"]}");
        Assert.True(TestData["tomato"].ToString() == "vegetable");
    }

    [Fact, TestPriority(1)]
    public void GetTypedResource()
    {
        output.WriteLine("Checking contents of 'packed_kv.txt' as KVTestRes");
        KVTestRes testRes = ResourceManager.Load<KVTestRes>("packed_kv.txt");

        output.WriteLine($"Checking prop Apple:\n   expected: 'fruit'\n   got: {testRes.Apple}");
        Assert.True(testRes.Apple == "fruit");
        output.WriteLine($"Checking prop Tomato:\n   expected: 'vegetable'\n   got: {testRes.Tomato}");
        Assert.True(testRes.Tomato == "vegetable");
    }
}
