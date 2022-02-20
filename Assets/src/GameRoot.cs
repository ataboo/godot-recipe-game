using Godot;
using RecipeGame.Helpers;
using RecipeGame.Inventory;
using RecipeGame.Models;
using System;
using static RecipeGame.Helpers.Enums;

public class GameRoot : Node2D
{
    [Export]
    public NodePath recipePanelPath;

    private readonly InventoryService _inventoryService = new InventoryService();

    private MainMenuControl mainMenu;

    private FadePanel fadePanel;

    private bool transitioning = false;

    private Node level;

    private Node2D levelHolder;

    private PlayerData playerData;

    private RecipePanelControl recipePanel;

    private PackedScene cottageSceneRes = GD.Load<PackedScene>("res://Assets/scenes/CottageScene.tscn");
    private PackedScene outdoorSceneRes = GD.Load<PackedScene>("res://Assets/scenes/OutdoorScene.tscn");

    public override void _Ready()
    {
        mainMenu = GetNode<MainMenuControl>("MainMenu") ?? throw new NullReferenceException();
        fadePanel = GetNode<FadePanel>("FadePanel") ?? throw new NullReferenceException();
        levelHolder = GetNode<Node2D>("LevelHolder") ?? throw new NullReferenceException();
        recipePanel = this.MustGetNode<RecipePanelControl>(recipePanelPath);

        mainMenu.Connect(nameof(MainMenuControl.OnStartNewGame), this, nameof(HandleNewGamePress));
        mainMenu.HamburgerButton.Connect("pressed", this, nameof(HandlePausePress));

        recipePanel.Visible = false;
    }

    public override void _Process(float delta)
    {
        if(Input.IsActionJustPressed("ui_cancel")) 
        {
            HandlePausePress();
        }        
    }

    void HandlePausePress()
    {
        if(!transitioning && level != null) 
        {
            mainMenu.Visible = !mainMenu.Visible;
            GetTree().Paused = mainMenu.Visible;
        }  
    }

    void HandleNewGamePress() 
    {
        if(transitioning) 
        {
            return;
        }

        playerData = new PlayerData();
        _inventoryService.InitNewInventories(playerData);

        recipePanel.Show();
        recipePanel.UpdateUnlockedRecipes(playerData);
        mainMenu.ConfirmNewGames = true;
        mainMenu.SpeedClock.Start();
        
        TransitionToCottage();
    }
    
    void OnInstructionsButtonPress() 
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
            outdoorLevel.Connect(nameof(OutdoorSceneControl.OnTransitionToCottage), this, nameof(TransitionToCottage));
            outdoorLevel.Connect(nameof(OutdoorSceneControl.OnPurchasedRecipe), this, nameof(HandlePurchasedRecipe));
            outdoorLevel.Connect(nameof(OutdoorSceneControl.OnGameVictory), this, nameof(HandleGameVictory));
            return outdoorLevel;
        });
    }

    public void TransitionToCottage()
    {
        DoTransition(() => {
            var cottageLevel = cottageSceneRes.Instance<CottageSceneControl>();
            cottageLevel.PlayerData = playerData;
            cottageLevel.Connect(nameof(CottageSceneControl.OnTransitionToMap), this, nameof(TransitionToMap));
            
            return cottageLevel;
        });
    }

    public void HandlePurchasedRecipe()
    {
        recipePanel.UpdateUnlockedRecipes(playerData);
    }

    public void HandleGameVictory()
    {
        mainMenu.SpeedClock.Stop();
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
