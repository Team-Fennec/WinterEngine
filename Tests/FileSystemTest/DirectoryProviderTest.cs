using WinterEngine.Resource;
using WinterEngine.Resource.Providers;
using ValveKeyValue;
using XUnit.Project.Attributes;
using Xunit.Abstractions;

namespace FileSystemTest;

[TestCaseOrderer(
    ordererTypeName: "XUnit.Project.Orderers.PriorityOrderer",
    ordererAssemblyName: "FileSystemTest")]
public class DirectoryProviderTest
{
    private readonly ITestOutputHelper output;

    public DirectoryProviderTest(ITestOutputHelper helper)
    {
        output = helper;
    }

    [Fact, TestPriority(0)]
    public void AddProvider()
    {
        output.WriteLine("Adding 'testres' with DirectoryProvider");
        ResourceManager.AddProvider(new DirectoryProvider("testres"));
    }

    // the order of the others doesn't matter, we just need the provider added first.
    [Fact, TestPriority(1)]
    public void GetFileData()
    {
        output.WriteLine("Checking contents of 'keyvalues.txt'");
        Stream kvData = ResourceManager.GetData("keyvalues.txt");
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
        output.WriteLine("Checking contents of 'keyvalues.txt' as KVTestRes");
        KVTestRes testRes = ResourceManager.Load<KVTestRes>("keyvalues.txt");

        output.WriteLine($"Checking prop Apple:\n   expected: 'fruit'\n   got: {testRes.Apple}");
        Assert.True(testRes.Apple == "fruit");
        output.WriteLine($"Checking prop Tomato:\n   expected: 'vegetable'\n   got: {testRes.Tomato}");
        Assert.True(testRes.Tomato == "vegetable");
    }
}