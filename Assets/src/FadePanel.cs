using Godot;
using System;

public class FadePanel : Node
{
    [Export]
    public NodePath animPlayerPath;

    private AnimationPlayer animPlayer;

    public override void _Ready()
    {
        animPlayer = GetNode<AnimationPlayer>(animPlayerPath);
    }

    public SignalAwaiter FadeOutAndWait() 
    {
        animPlayer.Play("PanelFadeOut");

        return ToSignal(animPlayer, "animation_finished");
    }

    public SignalAwaiter FadeInAndWait() 
    {
        animPlayer.Play("PanelFadeIn");

        return ToSignal(animPlayer, "animation_finished");
    }
}
