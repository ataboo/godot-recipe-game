using Godot;
using System;

public class FadePanel : ColorRect
{
    private AnimationPlayer anim;

    public override void _Ready()
    {
        anim = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public SignalAwaiter FadeOutAndWait() 
    {
        anim.Play("PanelFadeOut");

        return ToSignal(anim, "animation_finished");
    }

    public SignalAwaiter FadeInAndWait() 
    {
        anim.Play("PanelFadeIn");

        return ToSignal(anim, "animation_finished");
    }
}
