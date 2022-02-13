using Godot;
using System;

public class MapCameraFollow : Camera2D
{
    private Node2D followTarget;

    [Export]
    public float MoveRate = 1f;

    public override void _Ready()
    {
        followTarget = GetNode<Node2D>("../Player");
    }

    public override void _Process(float delta)
    {
        Position = Position.LinearInterpolate(followTarget.Position, MoveRate * delta);    
    }
}
