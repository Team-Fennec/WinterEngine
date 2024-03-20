using WinterEngine.SceneSystem;
using System.Diagnostics;

namespace SceneSystemTest;

public class SceneTests
{
    public Scene testScene;
    public TestEntA testEnt;
    public int countPaused;

    [Fact]
    public void SceneLoopTest()
    {
        #region Creating and Loading Scene
        //=================================================//
        Console.WriteLine("Creating and Loading Scene...");

        // if the name isn't matching something is very wrong
        Scene newScene = new Scene("Testing Scene");
        Assert.True(newScene.Name == "Testing Scene");

        // these should be the same object, as load scene returns what's added
        testScene = SceneManager.LoadScene(newScene);
        Assert.True(testScene == newScene);

        Assert.True(testScene == SceneManager.CurrentScene);
        #endregion

        #region Double Load Scene Fail
        //=================================================//
        Console.WriteLine("Attempting to double load Scene...\n(a pass means this operation failed which is good)");

        bool didFail = false;
        try
        {
            SceneManager.LoadScene(testScene);
        }
        catch (Exception ex)
        {
            didFail = true;
        }

        Assert.True(didFail);
        #endregion

        #region Creating and Adding Entity
        //=================================================//
        Console.WriteLine("Creating and Adding Test Entity...");
        
        testEnt = new TestEntA();
        testEnt.Name = "Testing Entity";

        Assert.True(testScene.AddEntity(testEnt) == testEnt);
        #endregion

        #region Checking Entity Data
        //=================================================//
        Console.WriteLine("Checking entity data in scene...");

        Assert.True(testScene.GetEntity<TestEntA>("Testing Entity") == testEnt);
        Assert.True(testEnt.Transform.Parent != null);
        Assert.True(testEnt.Transform.Parent == testScene.RootTransform);
        #endregion

        #region Double Add Fail
        //=================================================//
        Console.WriteLine("Attempting to double add entity...\n(a pass means this operation failed which is good)");

        didFail = false;
        try
        {
            testScene.AddEntity(testEnt);
        }
        catch (Exception ex)
        {
            didFail = true;
        }

        Assert.True(didFail);
        #endregion

        #region Get Nonexistent Entity Fail
        //=================================================//
        Console.WriteLine("Attempting to get an entity that doesn't exist...\n(a pass means this operation failed which is good)");
        Assert.True(testScene.GetEntity("I don't exist") == null);
        #endregion

        #region Scene Loop
        //=================================================//
        Console.WriteLine("Running scene loop...");
        
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
        string elapsedTime = String.Format("{2:00}.{3:00}", ts.Seconds, ts.Milliseconds / 10);
        Console.WriteLine($"Scene Runtime: {elapsedTime}");

        Console.WriteLine("Entity counter should NOT be zero.");
        Assert.True(testEnt.Counter > 0);
        #endregion

        #region Scene Pause
        //=================================================//
        Console.WriteLine("Pausing scene and running loop...");
        testScene.Paused = true;
        countPaused = testEnt.Counter;

        timer = new Stopwatch();
        timer.Start();
        while(true)
        {
            // todo: we should try deltatime probably?
            SceneManager.Update(0.0);

            if (timer.Elapsed.Seconds > 2) break;
        }
        timer.Stop();

        // Get the elapsed time as a TimeSpan value.
        ts = timer.Elapsed;
        // Format and display the TimeSpan value.
        elapsedTime = String.Format("{2:00}.{3:00}", ts.Seconds, ts.Milliseconds / 10);
        Console.WriteLine($"Scene Runtime: {elapsedTime}");

        Console.WriteLine("Counter should not have changed");
        Assert.True(testEnt.Counter == countPaused);
        #endregion

        #region Unload Scene
        //=================================================//
        Console.WriteLine("Unloading scene and running loop...");
        SceneManager.UnloadScene(testScene);
        countPaused = testEnt.Counter;

        timer = new Stopwatch();
        timer.Start();
        while(true)
        {
            // todo: we should try deltatime probably?
            SceneManager.Update(0.0);

            if (timer.Elapsed.Seconds > 2) break;
        }
        timer.Stop();

        // Get the elapsed time as a TimeSpan value.
        ts = timer.Elapsed;
        // Format and display the TimeSpan value.
        elapsedTime = String.Format("{2:00}.{3:00}", ts.Seconds, ts.Milliseconds / 10);
        Console.WriteLine($"Scene Runtime: {elapsedTime}");

        Console.WriteLine("Counter should not have changed");
        Assert.True(testEnt.Counter == countPaused);
        #endregion

        Console.WriteLine("All pass!");
    }
}