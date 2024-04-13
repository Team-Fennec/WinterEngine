using System.IO;
using Veldrid;

namespace WinterEngine.Core;

public struct ConVar
{
    public string Key { get; }
    public Type Type { get; }
    public object Value;
    public object DefaultValue { get; }

    public ConVar(string key, object value)
    {
        Key = key;
        Type = value.GetType();
        Value = value;
        DefaultValue = value;
    }
}

public static class ConfigManager
{
    private static readonly ILog m_Log = LogManager.GetLogger("Config");

    private static List<ConVar> m_ConVars = new List<ConVar>();

    public static void Init()
    {
        m_Log.Info("Initializing ConfigManager");
    }

    public static void RegisterCVar(string key, object value)
    {
        if (m_ConVars.Where(i => i.Key == key).ToList().Count > 0)
        {
            m_Log.Error($"Convar {key} aready exists!");
        }
        else
        {
            m_ConVars.Add(new ConVar(key, value));
        }
    }

    public static ConVar? GetCVar(string key)
    {
        foreach (ConVar cvar in m_ConVars)
        {
            if (cvar.Key == key)
            {
                return cvar;
            }
        }

        m_Log.Error($"No convar by key {key}");
        return null;
    }

    public static void SetValue(string key, object value)
    {
        // no you can't null check this variable, it's typed. (CS0019,CS0037)
        // you also can't make it nullable, that makes it readonly. (CS0200)
        ConVar cvar = m_ConVars.Where(i => i.Key == key).First();
        try
        {
            cvar.Value = value;
        }
        catch
        {
            m_Log.Error($"No convar by key {key}");
        }
    }

    public static void SetValue<T>(string key, T value)
    {
        // no you can't null check this variable, it's typed. (CS0019,CS0037)
        // you also can't make it nullable, that makes it readonly. (CS0200)
        ConVar cvar = m_ConVars.Where(i => i.Key == key).First();
        try
        {
            if (cvar.Type == typeof(T))
                cvar.Value = value;
            else
                m_Log.Error($"Convar {key} is not of type {typeof(T)}");
        }
        catch
        {
            m_Log.Error($"No convar by key {key}");
        }
    }

    public static object? GetValue(string key)
    {
        foreach (ConVar cvar in m_ConVars)
        {
            if (cvar.Key == key)
            {
                return cvar.Value;
            }
        }

        m_Log.Error($"No convar by key {key}");
        return null;
    }
    public static T? GetValue<T>(string key) => (T)GetValue(key);
}
