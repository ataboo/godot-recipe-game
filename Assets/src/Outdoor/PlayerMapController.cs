using Godot;
using RecipeGame.Helpers;
using RecipeGame.Shared;
public class PlayerMapController : KinematicBody2D
{
    [Export]
    public NodePath footAudioPath;

    public bool ControlEnabled { get; set; } = true;

    private Sprite playerSprite;

    private Vector2 velocity;

    private CommonPlayerControl playerControl;

    private AudioStreamPlayer2D footAudio;

    public override void _Ready()
    {
        playerSprite = GetNode<Sprite>("PlayerSprite");
        footAudio = this.MustGetNode<AudioStreamPlayer2D>(footAudioPath);

        playerControl = new CommonPlayerControl() {
            WalkAccelRate = 15f,
            WalkAccelDecay = 20f,
            MaxWalkSpeed = 250f
        };
    }

    public override void _PhysicsProcess(float delta) 
    {
        var shouldPlayFootAudio = false;
        if(ControlEnabled) 
        {
            velocity = playerControl.PhysicsUpdate(velocity, delta);
            velocity = MoveAndSlide(velocity);

            if(velocity.LengthSquared() > 1e-4) 
            {
                shouldPlayFootAudio = true;
                playerSprite.FlipH = velocity.x > 0;
            } 
        }

        if(footAudio.Playing && !shouldPlayFootAudio)
        {
            footAudio.Stop();
        }
        else if(!footAudio.Playing && shouldPlayFootAudio)
        {
            footAudio.Play();
        }
    }
}
