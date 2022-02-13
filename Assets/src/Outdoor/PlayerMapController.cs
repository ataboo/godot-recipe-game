using Godot;
using System;

public class PlayerMapController : KinematicBody2D
{
    public float walkAccelRate = 20f;

    public float walkAccelDecay = 20;

    public float maxWalkSpeed = 300f;

    private Vector2 velocity;

    public override void _Ready()
    {
        
    }

    private void GetInput(float delta) {
        var leftDown = Input.IsActionPressed("ui_left");
        var rightDown = Input.IsActionPressed("ui_right");
        var upDown = Input.IsActionPressed("ui_up");
        var downDown = Input.IsActionPressed("ui_down");

        if(leftDown == rightDown) 
        {
            velocity.x -= velocity.x * walkAccelDecay * delta;
            if(Math.Abs(velocity.x) < 1e-4) {
                velocity.x = 0;
            }
        }
        else 
        {
            if(leftDown) 
            {
                velocity.x -= walkAccelRate;
            } 
            if(rightDown)
            {
                velocity.x += walkAccelRate;
            } 
        }

        if(upDown == downDown) 
        {
            velocity.y -= velocity.y * walkAccelDecay * delta;
            if(Math.Abs(velocity.y) < 1) {
                velocity.y = 0;
            }
        }
        else 
        {
            if(upDown) 
            {
                velocity.y -= walkAccelRate;
            }
            if(downDown) 
            {
                velocity.y += walkAccelRate;
            }
        }

        velocity = velocity.Clamped(maxWalkSpeed);
        // velocity = velocity.Normalized() * maxWalkSpeed;
        // GD.Print($"LR:{leftDown == rightDown}, UD:{upDown == downDown}, Velocity: {velocity}");
    }

    public override void _PhysicsProcess(float delta) 
    {
        GetInput(delta);
        velocity = MoveAndSlide(velocity);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
