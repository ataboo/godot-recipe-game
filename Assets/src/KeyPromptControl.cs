using Godot;
using System;

public class KeyPromptControl : PanelContainer
{
    private Label label;

    public override void _Ready()
    {
        label = GetNode<Label>("Label");
    }

    public void SetText(string text)
    {
        label.Text = text;
    }   
}
