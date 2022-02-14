using Godot;
using RecipeGame.Inventory;
using RecipeGame.Models;
using System;

public class OutdoorSceneControl : Node2D
{
    private GameRoot gameRoot;
    private CauldronService cauldronService;

    public PlayerData PlayerData { get; set; }

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

    void OnCottageEntered(Node node) 
    {
        if(node is PlayerMapController player) 
        {
            gameRoot.TransitionToCottage();
        }
    }
}
