using Godot;
using System;
using static RecipeGame.Helpers.Enums;

public class BiomeAreaControl : Area2D
{
    [Export]
    public BiomeType biome;

    [Signal]
    public delegate void OnBiomeAreaEnter(BiomeType type);

    [Signal]
    public delegate void OnBiomeAreaExit(BiomeType type);

    public override void _Ready()
    {
        Connect("body_entered", this, nameof(HandleAreaEnter));
        Connect("body_exited", this, nameof(HandleAreExit));
    }

    void HandleAreaEnter(Node other)
    {
        if(other is PlayerMapController)
        {
            EmitSignal(nameof(OnBiomeAreaEnter), biome);
        }
    }

    void HandleAreExit(Node other)
    {
        if(other is PlayerMapController)
        {
            EmitSignal(nameof(OnBiomeAreaExit), biome);
        }
    }
}
