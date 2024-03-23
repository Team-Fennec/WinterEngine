using System;
using log4net;
#if HAS_PROFILING
using WinterEngine.Diagnostics;
#endif

namespace WinterEngine.SceneSystem;

public static class SceneManager
{
	private static readonly ILog log = LogManager.GetLogger("SceneManager");
	private static Camera m_ActiveCamera;
	private static Scene m_CurrentScene;
	private static List<Scene> m_Scenes = new List<Scene>();

	public static Camera ActiveCamera => m_ActiveCamera;
	public static Scene[] SceneList => m_Scenes.ToArray();
	public static Scene CurrentScene => m_CurrentScene;

	public static void Update(double deltaTime)
	{
#if HAS_PROFILING
		Profiler.PushProfile("SceneUpdate");
#endif
		foreach (Scene scene in m_Scenes)
		{
			scene.Update(deltaTime);
		}
#if HAS_PROFILING
	    Profiler.PopProfile();
#endif
	}

#region Loading/Unloading Scenes
	public static void UnloadScene(Scene scene)
	{
		log.Info($"Unloading scene {scene.Name}");
		if (scene == m_CurrentScene)
		{
			m_Scenes.Remove(scene);
			if (m_Scenes.Count == 0)
			{
				m_CurrentScene = null;
			}
			else 
			{
				m_Scenes[0] = m_CurrentScene;
			}
		}
		else
		{
			m_Scenes.Remove(scene);
		}
	}

	public static void UnloadScene(int index)
	{
		log.Info($"Unloading scene {m_Scenes[index].Name}");
		if (m_Scenes[index] == m_CurrentScene)
		{
			m_Scenes.RemoveAt(index);
			if (m_Scenes.Count == 0)
			{
				m_CurrentScene = null;
			}
			else 
			{
				m_Scenes[0] = m_CurrentScene;
			}
		}
		else
		{
			m_Scenes.RemoveAt(index);
		}
	}

	public static void UnloadScene(string name)
	{
		foreach (Scene scene in m_Scenes)
		{
			if (scene.Name != name)
				continue;

			log.Info($"Unloading scene {scene.Name}");
			if (scene == m_CurrentScene)
			{
				m_Scenes.Remove(scene);
				if (m_Scenes.Count == 0)
				{
					m_CurrentScene = null;
				}
				else 
				{
					m_Scenes[0] = m_CurrentScene;
				}
			}
			else
			{
				m_Scenes.Remove(scene);
			}
		}
	}

	// attempts to load the given scene from an asset file
	public static Scene LoadScene(string name)
	{
		// todo(scenesystem): Load scene from file
		throw new NotImplementedException();
	}

	public static Scene LoadScene(Scene scene)
	{
		if (m_Scenes.Contains(scene))
		{
			Exception ex = new Exception();
			log.Fatal("Attempted to add scene object to the array >1 times. Don't do that.", ex);
			throw ex;
		}
		else
		{
			log.Info($"Loading scene {scene.Name}");
			m_CurrentScene = scene;
			m_Scenes.Add(scene);
			return scene;
		}
	}
#endregion
}
