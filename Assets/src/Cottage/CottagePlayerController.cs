using Godot;
using RecipeGame.Helpers;
using RecipeGame.Shared;

public class CottagePlayerController : KinematicBody2D
{
    private RandomNumberGenerator rand;

    public bool ControlEnabled { get; set; } = true;

    private Vector2 velocity;

    private Sprite playerSprite;

    private CommonPlayerControl playerControl;

    private AudioStreamPlayer2D footSounds;

    private float footFallTimeout = 0;

    bool rightFoot = false;

    public override void _Ready()
    {
        rand = new RandomNumberGenerator();
        rand.Randomize();
        playerSprite= GetNode<Sprite>("PlayerSprite");
        footSounds = this.MustGetNode<AudioStreamPlayer2D>("Footsounds");
        playerControl = new CommonPlayerControl() {
            WalkAccelRate = 30f,
            WalkAccelDecay = 30f,
            MaxWalkSpeed = 500f
        };
    }

    public override void _PhysicsProcess(float delta) 
    {
        var shouldPlayFoot = false;
        if(ControlEnabled) 
        {
            velocity = playerControl.PhysicsUpdate(velocity, delta);
            velocity = MoveAndSlide(velocity);

            if(velocity.LengthSquared() > 1e-4) {
                shouldPlayFoot = true;
                playerSprite.Rotation = Mathf.LerpAngle(playerSprite.Rotation, velocity.Angle() + Mathf.Pi/2, 0.8f);
            }
        }
        footFallTimeout -= delta;

        if(shouldPlayFoot && footFallTimeout <= 0)
        {
            footSounds.PitchScale = rightFoot ? rand.RandfRange(1.0f, 1.1f) : rand.RandfRange(0.9f, 1.0f);
            footSounds.Play();
            footFallTimeout = .200f;
            rightFoot = !rightFoot;
        }
    }
}
