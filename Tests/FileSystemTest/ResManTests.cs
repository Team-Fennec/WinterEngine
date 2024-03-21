using WinterEngine.Resource;
using WinterEngine.Resource.Providers;
using ValveKeyValue;

namespace FileSystemTest;

public class ResManTests
{
    public ResManTests()
    {
        // init log4net with defaults?
    }
    
    [Fact, TestPriority(0)]
    public void AddDirProviderTest()
    {
        ResourceManager.AddProvider(new DirectoryProvider("testres"));
    }
    
    // the order of the others doesn't matter, we just need the provider added first.
    [Fact, TestPriority(1)]
    public void GetFileDataTest()
    {
        Stream kvData = ResourceManager.GetData("keyvalues.txt");
        Assert.True(kvData != null);
        
        // read KVData
        var kv = KVSerializer.Create(KVSerializationFormat.KeyValues1Text);
        KVObject TestData = kv.Deserialize(kvData);
        kvData.Close();
        
        Assert.True(TestData["apple"].Value == "fruit");
        Assert.True(TestData["tomato"].Value == "vegetable");
    }
    
    [Fact, TestPriority(1)]
    public void GetTypedResource()
    {
        KVTestRes testRes = ResourceManager.Load<KVTestRes>("keyvalues.txt");
        
        Assert.True(testRes.Apple == "fruit");
        Assert.True(testRes.Tomato == "vegetable");
    }
}