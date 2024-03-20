using WinterEngine.SceneSystem;
using System.Diagnostics;
using Xunit.Abstractions;
using XUnit.Project.Attributes;

namespace SceneSystemTest;

public class SSFixt : IDisposable
{
    public SSFixt()
    {
        // nothing needed
    }

    public void Dispose()
    {
        // nothing needed
    }

    public Scene testScene;
    public TestEntA testEnt;
    public int countPaused;
}

[TestCaseOrderer(
    ordererTypeName: "XUnit.Project.Orderers.PriorityOrderer",
    ordererAssemblyName: "SceneSystemTest")]
public class SceneTests : IClassFixture<SSFixt>
{
    SSFixt fixture;

    public SceneTests(SSFixt fixture)
    {
        this.fixture = fixture;
    }

    [Fact, TestPriority(0)]
    public void CreateLoadSceneTest()
    {
        //output.WriteLine("================================================");
        #region Creating and Loading Scene
        //=================================================//
        //output.WriteLine("Creating and Loading Scene...");

        // if the name isn't matching something is very wrong
        Scene newScene = new Scene("Testing Scene");
        Assert.True(newScene.Name == "Testing Scene");

        // these should be the same object, as load scene returns what's added
        fixture.testScene = SceneManager.LoadScene(newScene);
        Assert.True(fixture.testScene == newScene);

        Assert.True(fixture.testScene == SceneManager.CurrentScene);
        #endregion
    }

    [Fact, TestPriority(1)]
    public void DoubleLoadFailTest()
    {
        //output.WriteLine("================================================");
        #region Double Load Scene Fail
        //=================================================//
        //output.WriteLine("Attempting to double load Scene...\n(a pass means this operation failed which is good)");

        bool didFail = false;
        try
        {
            SceneManager.LoadScene(fixture.testScene);
        }
        catch (Exception ex)
        {
            didFail = true;
        }

        Assert.True(didFail);
        #endregion
    }

    [Fact, TestPriority(2)]
    public void CreateAddEntityTest()
    {
        //output.WriteLine("================================================");
        #region Creating and Adding Entity
        //=================================================//
        //output.WriteLine("Creating and Adding Test Entity...");
        
        fixture.testEnt = new TestEntA();
        fixture.testEnt.Name = "Testing Entity";

        Assert.True(fixture.testScene.AddEntity(fixture.testEnt) == fixture.testEnt);
        #endregion
    }

    [Fact, TestPriority(3)]
    public void CheckEntityDataTest()
    {
        //output.WriteLine("================================================");
        #region Checking Entity Data
        //=================================================//
        //output.WriteLine("Checking entity data in scene...");

        Assert.True(fixture.testScene.GetEntity<TestEntA>("Testing Entity") == fixture.testEnt);
        Assert.True(fixture.testEnt.Transform.Parent != null);
        Assert.True(fixture.testEnt.Transform.Parent == fixture.testScene.RootTransform);
        #endregion
    }

    [Fact, TestPriority(4)]
    public void DoubleAddFailTest()
    {
        //output.WriteLine("================================================");
        #region Double Add Fail
        //=================================================//
        //output.WriteLine("Attempting to double add entity...\n(a pass means this operation failed which is good)");

        bool didFail = false;
        try
        {
            fixture.testScene.AddEntity(fixture.testEnt);
        }
        catch (Exception ex)
        {
            didFail = true;
        }

        Assert.True(didFail);
        #endregion
    }

    [Fact, TestPriority(5)]
    public void GetNonexistFailTest()
    {   
        //output.WriteLine("================================================");
        #region Get Nonexistent Entity Fail
        //=================================================//
        //output.WriteLine("Attempting to get an entity that doesn't exist...\n(a pass means this operation failed which is good)");
        Assert.True(fixture.testScene.GetEntity("I don't exist") == null);
        #endregion
    }

    [Fact, TestPriority(6)]
    public void SceneLoopTest()
    {
        //output.WriteLine("================================================");
        #region Scene Loop
        //=================================================//
        //output.WriteLine("Running scene loop...");
        
        Stopwatch timer = new Stopwatch();
        timer.Start();
        while(true)
        {
            // todo: we should try deltatime probably?
            SceneManager.Update(0.0);

            if (timer.Elapsed.Seconds > 9) break;
        }
        timer.Stop();

        // Get the elapsed time as a TimeSpan value.
        TimeSpan ts = timer.Elapsed;
        // Format and display the TimeSpan value.
        string elapsedTime = String.Format("{0:00}.{1:00}", ts.Seconds, ts.Milliseconds / 10);
        //output.WriteLine($"Scene Runtime: {elapsedTime}");

        //output.WriteLine("Entity counter should NOT be zero.");
        Assert.True(fixture.testEnt.Counter > 0);
        #endregion
    }

    [Fact, TestPriority(7)]
    public void ScenePauseTest()
    {
        //output.WriteLine("================================================");
        #region Scene Pause
        //=================================================//
        //output.WriteLine("Pausing scene and running loop...");
        fixture.testScene.Paused = true;
        fixture.countPaused = fixture.testEnt.Counter;

        Stopwatch timer = new Stopwatch();
        timer.Start();
        while(true)
        {
            // todo: we should try deltatime probably?
            SceneManager.Update(0.0);

            if (timer.Elapsed.Seconds > 2) break;
        }
        timer.Stop();

        // Get the elapsed time as a TimeSpan value.
        TimeSpan ts = timer.Elapsed;
        // Format and display the TimeSpan value.
        string elapsedTime = String.Format("{0:00}.{1:00}", ts.Seconds, ts.Milliseconds / 10);
        //output.WriteLine($"Scene Runtime: {elapsedTime}");

        //output.WriteLine("Counter should not have changed");
        Assert.True(fixture.testEnt.Counter == fixture.countPaused);
        #endregion
    }

    [Fact, TestPriority(8)]
    public void UnloadSceneTest()
    {
        //output.WriteLine("================================================");
        #region Unload Scene
        //=================================================//
        //output.WriteLine("Unloading scene and running loop...");
        SceneManager.UnloadScene(fixture.testScene);
        fixture.countPaused = fixture.testEnt.Counter;

        Stopwatch timer = new Stopwatch();
        timer.Start();
        while(true)
        {
            // todo: we should try deltatime probably?
            SceneManager.Update(0.0);

            if (timer.Elapsed.Seconds > 2) break;
        }
        timer.Stop();

        // Get the elapsed time as a TimeSpan value.
        TimeSpan ts = timer.Elapsed;
        // Format and display the TimeSpan value.
        string elapsedTime = String.Format("{0:00}.{1:00}", ts.Seconds, ts.Milliseconds / 10);
        //output.WriteLine($"Scene Runtime: {elapsedTime}");

        //output.WriteLine("Counter should not have changed");
        Assert.True(fixture.testEnt.Counter == fixture.countPaused);
        #endregion
    }
}