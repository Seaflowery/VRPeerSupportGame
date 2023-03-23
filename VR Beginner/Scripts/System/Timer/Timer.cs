using UnityEngine;

public class Timer
{
    protected float TotalTime;
    protected float PassedTime;
    
    public Timer(float time)
    {
        TotalTime = time;
        Reset();
    }
    
    public void Tick(float deltaTime)
    {
        PassedTime += deltaTime;
    }

    public bool TimeOut
    {
        get => PassedTime >= TotalTime;
    }

    public void Reset()
    {
        PassedTime = 0;
    }
}