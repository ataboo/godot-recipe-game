using System;
using Godot;
using RecipeGame.Inventory;
using RecipeGame.Models;

public class CottageSceneControl : Node2D
{
    public PlayerData PlayerData { get; set; }
    
    private GameRoot gameRoot;
    private CauldronService cauldronService;

    public override void _Ready()
    {
        cauldronService = new CauldronService();
        gameRoot = GetNode<GameRoot>("/root/GameRoot") ?? throw new NullReferenceException();
    }

    public override void _Process(float delta)
    {
        if(PlayerData?.Cauldron == null) {
            GD.PushError("No cauldron to update!");
            return;
        }

        cauldronService.Update(PlayerData.Cauldron, delta);
    }

    void OnHitExitTrigger(Node other)
    {
        if(other is CottagePlayerController) 
        {
            gameRoot.TransitionToMap();
        }
    }
}
