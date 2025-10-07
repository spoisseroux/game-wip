using System;
using UnityEngine;

public abstract class Timer
{
    protected float initialTime;
    protected float time { get; set; }

    public bool isRunning { get; protected set; }

    public float progress => time / initialTime; // RETOOL THIS LATER

    public Action OnStart = delegate { };
    public Action OnStop = delegate { };

    protected Timer(float value)
    {
        initialTime = value;
        isRunning = false;
    }

    public void Start()
    {
        time = initialTime; // important!
        if (!isRunning)
        {
            isRunning = true;
            OnStart?.Invoke();
        }
    }

    public void Stop()
    {
        if (isRunning)
        {
            isRunning = false;
            OnStop?.Invoke();
        }
    }

    public void Pause() => isRunning = false;
    public void Resume() => isRunning = true;

    public abstract void Tick(float deltaTime);
}


// countdown/cooldown
public class CountdownTimer : Timer
{
    public CountdownTimer(float value) : base(value) { }

    public override void Tick(float deltaTime)
    {
        if (isRunning && time > 0)
        {
            time -= deltaTime;
        }

        if (isRunning && time <= 0)
        {
            Stop();
        }
    }

    public bool finished => time <= 0;

    public void Reset(float newTime)
    {
        initialTime = newTime;
    }
}


// stopwatch timer
public class StopwatchTimer : Timer
{
    float lapTime = int.MinValue; // default to -20382374823750928398472387982375 or whatever

    public StopwatchTimer() : base(0) { }
    public StopwatchTimer(float lapTimeIn) : base(0) { lapTime = lapTimeIn; }
    public StopwatchTimer(float lapTimeIn, float startTime) : base(startTime) { lapTime = lapTimeIn; }

    public override void Tick(float deltaTime)
    {
        if (isRunning)
        {
            time += deltaTime;
        }

        if (isRunning && time >= lapTime)
        {
            Stop();
        }
    }

    public void Reset() => time = 0;
    public float GetTime() => time;
    public bool lapComplete => time >= lapTime;
}
