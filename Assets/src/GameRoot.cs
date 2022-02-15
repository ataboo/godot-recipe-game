using Godot;
using RecipeGame.Inventory;
using RecipeGame.Models;
using System;
using static RecipeGame.Helpers.Enums;

public class GameRoot : Node2D
{
    private readonly InventoryService _inventoryService = new InventoryService();

    private MainMenuControl mainMenu;

    private FadePanel fadePanel;

    private GameScene currentScene = GameScene.Map;

    private bool transitioning = false;

    private Node level;

    private Node2D levelHolder;

    private PlayerData playerData;

    private PackedScene cottageSceneRes = GD.Load<PackedScene>("res://Assets/scenes/CottageScene.tscn");
    private PackedScene outdoorSceneRes = GD.Load<PackedScene>("res://Assets/scenes/OutdoorScene.tscn");

    public override void _Ready()
    {
        mainMenu = GetNode<MainMenuControl>("MainMenu") ?? throw new NullReferenceException();
        fadePanel = GetNode<FadePanel>("FadePanel") ?? throw new NullReferenceException();
        levelHolder = GetNode<Node2D>("LevelHolder") ?? throw new NullReferenceException();
    }

    public override void _Process(float delta)
    {
        if(!transitioning && level != null && Input.IsActionJustPressed("ui_cancel")) 
        {
            mainMenu.Visible = !mainMenu.Visible;
            GetTree().Paused = mainMenu.Visible;
        }        
    }

    void OnNewGameButtonPress() 
    {
        if(transitioning) 
        {
            return;
        }

        playerData = new PlayerData();
        _inventoryService.InitNewInventories(playerData);
        
        // TransitionToCottage();

        TransitionToMap();
    }

    void OnContinueButtonPress() 
    {
        if(transitioning) 
        {
            return;
        }
        throw new NotImplementedException();
    }
    
    void OnInstructionsButtonPress() 
    {
        if(transitioning) 
        {
            return;
        }
        throw new NotSupportedException();
    }

    void OnQuitButtonPress() 
    {
        if(transitioning) 
        {
            return;
        }
        throw new NotSupportedException();
    }

    public void TransitionToMap() 
    {
        DoTransition(() => {
            var outdoorLevel = outdoorSceneRes.Instance<OutdoorSceneControl>();
            outdoorLevel.PlayerData = playerData;
            currentScene = GameScene.Map;
            
            return outdoorLevel;
        });
    }

    public void TransitionToCottage()
    {
        DoTransition(() => {
            var cottageLevel = cottageSceneRes.Instance<CottageSceneControl>();
            cottageLevel.PlayerData = playerData;
            currentScene = GameScene.Cottage;
            
            return cottageLevel;
        });
    }

    private async void DoTransition(Func<Node> loadSceneAction) 
    {
        if(transitioning) 
        {
            return;
        }

        GetTree().Paused = true;

        transitioning = true;
        await fadePanel.FadeOutAndWait();
        
        if(mainMenu.Visible) {
            mainMenu.Visible = false;
        }
        
        if(level != null) {
            levelHolder.RemoveChild(level);
            level.QueueFree();
        }

        level = loadSceneAction();
        levelHolder.AddChild(level);    

        GetTree().Paused = false;
        await fadePanel.FadeInAndWait();
        transitioning = false;
    }
}
