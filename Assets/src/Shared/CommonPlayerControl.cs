using System;
using Godot;

namespace RecipeGame.Shared 
{
    public class CommonPlayerControl 
    {
        public float WalkAccelRate { get; set; } = 20f;

        public float WalkAccelDecay { get; set; } = 20;

        public float MaxWalkSpeed { get; set; } = 300f;

        public Vector2 PhysicsUpdate(Vector2 velocity, float delta)
        {
            return UpdateVelocityFromInput(velocity, delta);
        }

        private Vector2 UpdateVelocityFromInput(Vector2 velocity, float delta) {
            var leftDown = Input.IsActionPressed("ui_left");
            var rightDown = Input.IsActionPressed("ui_right");
            var upDown = Input.IsActionPressed("ui_up");
            var downDown = Input.IsActionPressed("ui_down");

            if(leftDown == rightDown) 
            {
                velocity.x -= velocity.x * WalkAccelDecay * delta;
                if(Math.Abs(velocity.x) < 1e-4) {
                    velocity.x = 0;
                }
            }
            else 
            {
                if(leftDown) 
                {
                    velocity.x -= WalkAccelRate;
                } 
                if(rightDown)
                {
                    velocity.x += WalkAccelRate;
                } 
            }

            if(upDown == downDown) 
            {
                velocity.y -= velocity.y * WalkAccelDecay * delta;
                if(Math.Abs(velocity.y) < 1) {
                    velocity.y = 0;
                }
            }
            else 
            {
                if(upDown) 
                {
                    velocity.y -= WalkAccelRate;
                }
                if(downDown) 
                {
                    velocity.y += WalkAccelRate;
                }
            }

            return velocity.Clamped(MaxWalkSpeed);
        }
    }
}