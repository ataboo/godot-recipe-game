using Godot;
using System;

public class MainMenuControl : Node
{
    private Panel panel;

    public bool Visible 
    {
        get 
        {
            return panel.Visible;
        }
        set 
        {
            panel.Visible = value;
        }
    }

    public override void _Ready()
    {
        panel = GetNode<Panel>("Panel") ?? throw new NullReferenceException();
        var gameRoot = GetNode<GameRoot>("/root/GameRoot") ?? throw new NullReferenceException();
        var newGameBtn = panel.GetNode<Button>("NewGame") ?? throw new NullReferenceException();
        var continueBtn = panel.GetNode<Button>("Continue") ?? throw new NullReferenceException();
        var instructionsBtn = panel.GetNode<Button>("Instructions") ?? throw new NullReferenceException();
        var quitBtn = panel.GetNode<Button>("Quit") ?? throw new NullReferenceException();

        newGameBtn.Connect("pressed", gameRoot, "OnNewGameButtonPress");
        continueBtn.Connect("pressed", gameRoot, "OnContinueButtonPress");
        instructionsBtn.Connect("pressed", gameRoot, "OnInstructionsButtonPress");
        quitBtn.Connect("pressed", gameRoot, "OnQuitButtonPress");
    }

}
