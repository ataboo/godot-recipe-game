using Godot;
using RecipeGame.Shared;
public class PlayerMapController : KinematicBody2D
{
    public bool ControlEnabled { get; set; } = true;

    private Sprite playerSprite;

    private Vector2 velocity;

    private CommonPlayerControl playerControl;

    public override void _Ready()
    {
        playerSprite = GetNode<Sprite>("PlayerSprite");

        playerControl = new CommonPlayerControl() {
            WalkAccelRate = 15f,
            WalkAccelDecay = 20f,
            MaxWalkSpeed = 250f
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

        if(velocity.LengthSquared() > 1e-4) 
        {
            playerSprite.FlipH = velocity.x > 0;
        }
    }
}
