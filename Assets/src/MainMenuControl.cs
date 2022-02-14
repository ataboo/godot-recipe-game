using Godot;
using System;

public class MainMenuControl : Node2D
{
    public override void _Ready()
    {
        var gameRoot = GetNode<GameRoot>("/root/GameRoot") ?? throw new NullReferenceException();
        var newGameBtn = GetNode<Button>("Panel/NewGame") ?? throw new NullReferenceException();
        var continueBtn = GetNode<Button>("Panel/Continue") ?? throw new NullReferenceException();
        var instructionsBtn = GetNode<Button>("Panel/Instructions") ?? throw new NullReferenceException();
        var quitBtn = GetNode<Button>("Panel/Quit") ?? throw new NullReferenceException();

        newGameBtn.Connect("pressed", gameRoot, "OnNewGameButtonPress");
        continueBtn.Connect("pressed", gameRoot, "OnContinueButtonPress");
        instructionsBtn.Connect("pressed", gameRoot, "OnInstructionsButtonPress");
        quitBtn.Connect("pressed", gameRoot, "OnQuitButtonPress");
    }

}
