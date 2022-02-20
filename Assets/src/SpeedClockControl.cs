using Godot;
using System;

public class SpeedClockControl : Label
{
    private float elapsedTime;
    private bool running = false;

    public override void _Ready()
    {
        Text = "0:00.000";
    }

    public override void _Process(float delta)
    {
        if(running)
        {
            elapsedTime += delta;
        }
    
        var timeSpan = TimeSpan.FromSeconds(elapsedTime);
        Text = $"{(int)timeSpan.TotalMinutes}:{timeSpan.Seconds:D2}.{timeSpan.Milliseconds:D3}";
    }

    public void Start()
    {
        elapsedTime = 0;
        running = true;
    }

    public void Stop()
    {
        running = false;
    }
}
