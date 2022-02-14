using Godot;
using RecipeGame.Shared;

public class CottagePlayerController : KinematicBody2D
{
    public bool ControlEnabled { get; set; } = true;

    private Vector2 velocity;

    private Sprite playerSprite;

    private CommonPlayerControl playerControl;

    public override void _Ready()
    {
        playerSprite= GetNode<Sprite>("PlayerSprite");
        playerControl = new CommonPlayerControl() {
            WalkAccelRate = 30f,
            WalkAccelDecay = 30f,
            MaxWalkSpeed = 500f
        };
    }

    public override void _PhysicsProcess(float delta) 
    {
        if(!ControlEnabled) 
        {
            return;
        }

        velocity = playerControl.PhysicsUpdate(velocity, delta);
        velocity = MoveAndSlide(velocity);

        if(velocity.LengthSquared() > 1e-4) {
            playerSprite.Rotation = Mathf.LerpAngle(playerSprite.Rotation, velocity.Angle() + Mathf.Pi/2, 0.8f);
        }
    }
}
