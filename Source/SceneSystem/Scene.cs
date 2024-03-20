using System;
using System.Numerics;
using System.Collections.Generic;
using Datamodel;
using log4net;

namespace WinterEngine.SceneSystem;

public class Scene
{
	private static readonly ILog log = LogManager.GetLogger("Scene");
	public string Name;
	public bool Paused;
	public Transform RootTransform => m_Root;

	private List<Entity> m_Entities;
	private Transform m_Root;

	#region Constructors
	public Scene() : this("You Forgot To Name Your Scene") {}
	public Scene(string name)
	{
		Name = name;
		Paused = false;
		m_Entities = new List<Entity>();
		m_Root = new Transform();
	}
	#endregion

	public void Update(double deltaTime)
	{
		if (Paused) return;
		foreach (Entity ent in m_Entities)
		{
			ent.Think(deltaTime);
		}
	}

	#region Entity Management Methods
	public Entity AddEntity(Entity entity)
	{
		if (m_Entities.Contains(entity))
		{
			Exception ex = new Exception();
			log.Fatal("Attempted to duplicate entity!", ex);
			throw ex;
		}

		if (entity.Transform.Parent == null)
		{
			entity.Transform.SetParent(m_Root);
		}
		m_Entities.Add(entity);
		return entity;
	}

	public Entity? GetEntity(string name)
	{
		foreach (Entity ent in m_Entities)
		{
			if (ent.Name == name)
				return ent;
		}
		log.Error($"Entity by name {name} doesn't exist in scene!");
		return null;
	}
	public T? GetEntity<T>(string name) where T : Entity => (T)GetEntity(name);

	public void RemoveEntity(Entity entity)
	{
		if (!m_Entities.Contains(entity))
		{
			log.Error("Attempted to remove entity that does not exist in the list!");
			return;
		}
		else
		{
			m_Entities.Remove(entity);
		}
	}

	public void RemoveEntity(string name)
	{
		foreach (Entity ent in m_Entities)
		{
			if (ent.Name == name)
			{
				RemoveEntity(ent);
				return;
			}
		}
		log.Error($"Entity by name {name} doesn't exist in scene!");
		return;
	}
	#endregion
}
