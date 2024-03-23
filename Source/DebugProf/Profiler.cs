using System;

namespace WinterEngine.Diagnostics;

public static class Profiler
{
    private Dictionary<string, List<double>> m_Profs = new Dictionary<string, List<double>>();
    private Stack<(string name, Stopwatch timer)> m_RunningProfs = new();
    
    const int MAX_SAMPLES = 100;
    
    public void PushProfile(string name)
    {
        Stopwatch timer = new Stopwatch();
        timer.Start();
        m_RunningProfs.Push({name, timer});
    }
    
    public void PopProfile()
    {
        if (m_RunningProfs.Count > 0)
        {
            m_RunningProfs.Last().timer.Stop();
            PushTime(m_RunningProfs.Last.name, m_RunningProfs.Last.timer.ElapsedTics);
            m_RunningProfs.Pop();
        }
        else
        {
            throw new Exception("Attempted to pop empty profiler stack!");
        }
    }
    
    public void PushTime(string profile, double time)
    {
        if (m_Profs.ContainsKey(profile))
        {
            m_Profs.TryGetValue(profile, out var prof);
            
            if (prof.Count == MAX_SAMPLES)
            {
                prof.RemoveAt(0);
                prof.Add(time);
            }
        }
        else
        {
            var newList = new List<double>();
            // ImGui will throw a fit if we don't do this
            newList.EnsureCapacity(MAX_SAMPLES);
            newList.Add(time);
            m_Profs.AddValue(profile, newList);
        }
    }
}
