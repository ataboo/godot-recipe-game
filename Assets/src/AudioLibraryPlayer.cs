using Godot;
using System;
using System.Collections.Generic;

public class AudioLibraryPlayer : Node2D
{
    protected Dictionary<string, AudioStreamPlayer2D> sources;

    public override void _Ready()
    {
        sources = new Dictionary<string, AudioStreamPlayer2D>();
        foreach(AudioStreamPlayer2D child in GetChildren())
        {
            sources[child.Name.ToLower()] = child;
        }
    }

    public void PlaySound(string name)
    {
        if(sources.TryGetValue(name.ToLower(), out var source))
        {
            source.Play();
            return;
        }

        GD.PushError($"Failed to find audio: '{name}'");
    }

    public AudioStreamPlayer2D GetSound(string name)
    {
        return sources[name];
    }

    public void StopIfPlaying(string soundName)
    {
        if(sources.TryGetValue(soundName, out var source))
        {
            if(source.Playing)
            {
                source.Stop();
            }
            return;
        }

        GD.PushError($"Can't find source '{soundName}'");
    }
}
