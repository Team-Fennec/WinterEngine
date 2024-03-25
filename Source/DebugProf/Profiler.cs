using System.Diagnostics;

namespace WinterEngine.Diagnostics;


public static class Profiler
{
    private static Dictionary<string, float[]> m_Profs = new Dictionary<string, float[]>();
    private static Stack<(string name, Stopwatch timer)> m_RunningProfs = new();
    public static Dictionary<string, float[]> Profs => m_Profs; // todo(prof): make this readonly

    const int MAX_SAMPLES = 100;
    
    public static void PushProfile(string name)
    {
        Stopwatch timer = new Stopwatch();
        timer.Start();
        m_RunningProfs.Push((name, timer));
    }
    
    public static void PopProfile()
    {
        if (m_RunningProfs.Count > 0)
        {
            m_RunningProfs.Last().timer.Stop();
            PushTime(m_RunningProfs.Last().name, m_RunningProfs.Last().timer.ElapsedTicks);
            m_RunningProfs.Pop();
        }
        else
        {
            throw new Exception("Attempted to pop empty profiler stack!");
        }
    }
    
    public static void PushTime(string profile, float time)
    {
        if (m_Profs.ContainsKey(profile))
        {
            m_Profs.TryGetValue(profile, out var prof);
            
            for (int i = 1; i < prof.Length; i++)
            {
                prof[i - 1] = prof[i];
            }
            prof[prof.Length - 1] = time;
        }
        else
        {
            var newList = new float[100];
            newList[99] = time;
            m_Profs.TryAdd(profile, newList);
        }
    }
}
