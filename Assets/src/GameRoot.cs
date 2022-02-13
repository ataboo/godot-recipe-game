using Godot;
using System;

public class GameRoot : Node2D
{
    private MainMenuControl mainMenu;

    private OutdoorSceneControl outdoorScene;

    private CottageSceneControl cottageScene;

    public override void _Ready()
    {
        mainMenu = GetNode<MainMenuControl>("OutdoorScene") ?? throw new NullReferenceException();
        outdoorScene = GetNode<OutdoorSceneControl>("Main Menu") ?? throw new NullReferenceException();
        // cottageScene = GetNode<CottageSceneControl>("CottageScene") ?? throw new NullReferenceException();
    }

    public override void _Process(float delta)
    {
        if(Input.IsActionJustPressed("pause")) 
        {
            var isPaused = mainMenu.Visible;

            outdoorScene.GetTree().Paused = !isPaused;
            //TODO pause other scenes.
            mainMenu.Visible = !isPaused;
        }        
    }

    public void MakeOutdoorActive() 
    {

    }

    public void MakeCottageActive()
    {

    }
}
