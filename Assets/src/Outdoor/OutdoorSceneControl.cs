using Godot;
using RecipeGame.Helpers;
using RecipeGame.Inventory;
using RecipeGame.Models;
using System;
using System.Linq;
using static RecipeGame.Helpers.Enums;

public class OutdoorSceneControl : Node2D
{
    [Signal]
    public delegate void OnGameVictory();
    [Signal]
    public delegate void OnTransitionToCottage();
    [Signal]
    public delegate void OnPurchasedRecipe();

    [Export]
    public NodePath promptPath;
    [Export]
    public NodePath caveAreaPath;
    [Export]
    public NodePath shoreAreaPath;
    [Export]
    public NodePath forestAreaPath;
    [Export]
    public NodePath lakeAreaPath;
    [Export]
    public NodePath townAreaPath;
    [Export]
    public NodePath caveWarningPanelPath;
    [Export]
    public NodePath shoreWarningPanelPath;

    private CauldronService cauldronService;

    private OutdoorSceneItemControl sceneItems;

    private KeyPromptControl keyPrompt;

    public PlayerData PlayerData { get; set; }

    private Action queuedAction;

    private PlayerMapController player;

    private Panel caveWarningPanel;
    private Panel shoreWarningPanel;

    private InventoryService inventoryService;

    public override void _Ready()
    {
        cauldronService = new CauldronService();
        keyPrompt = GetNode<KeyPromptControl>(promptPath) ?? throw new NullReferenceException();
        sceneItems = GetNode<OutdoorSceneItemControl>("OutdoorSceneItems") ?? throw new NullReferenceException();
        player = GetNode<PlayerMapController>("Player") ?? throw new NullReferenceException();
        caveWarningPanel = this.MustGetNode<Panel>(caveWarningPanelPath);
        shoreWarningPanel = this.MustGetNode<Panel>(shoreWarningPanelPath);

        keyPrompt.Connect("pressed", this, nameof(HandlePromptPress));

        var forest = GetNode<BiomeAreaControl>(forestAreaPath) ?? throw new NullReferenceException();
        forest.Connect(nameof(BiomeAreaControl.OnBiomeAreaEnter), this, nameof(HandleEnterBiomeArea));
        forest.Connect(nameof(BiomeAreaControl.OnBiomeAreaExit), this, nameof(HandleExitBiomeArea));
        var cave = GetNode<BiomeAreaControl>(caveAreaPath) ?? throw new NullReferenceException();
        cave.Connect(nameof(BiomeAreaControl.OnBiomeAreaEnter), this, nameof(HandleEnterBiomeArea));
        cave.Connect(nameof(BiomeAreaControl.OnBiomeAreaExit), this, nameof(HandleExitBiomeArea));
        var lake = GetNode<BiomeAreaControl>(lakeAreaPath) ?? throw new NullReferenceException();
        lake.Connect(nameof(BiomeAreaControl.OnBiomeAreaEnter), this, nameof(HandleEnterBiomeArea));
        lake.Connect(nameof(BiomeAreaControl.OnBiomeAreaExit), this, nameof(HandleExitBiomeArea));
        var shore = GetNode<BiomeAreaControl>(shoreAreaPath) ?? throw new NullReferenceException();
        shore.Connect(nameof(BiomeAreaControl.OnBiomeAreaEnter), this, nameof(HandleEnterBiomeArea));
        shore.Connect(nameof(BiomeAreaControl.OnBiomeAreaExit), this, nameof(HandleExitBiomeArea));
        var town = GetNode<BiomeAreaControl>(townAreaPath) ?? throw new NullReferenceException();
        town.Connect(nameof(BiomeAreaControl.OnBiomeAreaEnter), this, nameof(HandleEnterMarketArea));
        town.Connect(nameof(BiomeAreaControl.OnBiomeAreaExit), this, nameof(HandleExitBiomeArea));

        sceneItems.Connect(nameof(OutdoorSceneItemControl.OnLeavePanel), this, nameof(HandleLeavePanel));
        sceneItems.Connect(nameof(OutdoorSceneItemControl.OnPurchasedRecipe), this, nameof(HandlePurchasedRecipe));
        
        sceneItems.Init(PlayerData);

        inventoryService = new InventoryService();
    }

    public override void _Process(float delta)
    {
        if(PlayerData?.Cauldron == null) {
            GD.PushError("No cauldron to update!");
            return;
        }

        cauldronService.Update(PlayerData.Cauldron, delta);

        if(Input.IsActionJustPressed("perform_action"))
        {
            HandlePromptPress();
        }
    }

    private void HandlePromptPress()
    {
        if(queuedAction != null)
        {
            queuedAction();
            queuedAction = null;
            keyPrompt.Visible = false;
        }
    }

    void OnCottageEntered(Node node) 
    {
        if(node is PlayerMapController player) 
        {
            EmitSignal(nameof(OnTransitionToCottage));
        }
    }

    void HandleEnterBiomeArea(BiomeType biome)
    {
        var biomeName = Enum.GetName(typeof(BiomeType), biome);

        if(!inventoryService.CanEnterBiome(PlayerData, biome))
        {
            if(biome == BiomeType.Cave)
            {
                caveWarningPanel.Visible = true;
            }
            else if(biome == BiomeType.Shore)
            {
                shoreWarningPanel.Visible = true;
            }

            return;
        }

        keyPrompt.Text = $"Enter {biomeName} (E)";
        keyPrompt.Visible = true;
        queuedAction = () => {
            sceneItems.ShowForageBiome(biome);
            player.ControlEnabled = false;
        };
    }

    void HandleExitBiomeArea(BiomeType biome)
    {
        if(biome == BiomeType.Cave)
        {
            caveWarningPanel.Visible = false;
        }
        else if(biome == BiomeType.Shore)
        {
            shoreWarningPanel.Visible = false;
        }

        keyPrompt.Visible = false;
        queuedAction = null;
    }

    void HandleLeavePanel()
    {
        player.ControlEnabled = true;
    }

    void HandlePurchasedRecipe()
    {
        EmitSignal(nameof(OnPurchasedRecipe));
    }

    void HandleEnterMarketArea(BiomeType _)
    {
        keyPrompt.Text = $"E: Enter Town Market";
        keyPrompt.Visible = true;
        queuedAction = () => {
            if(!PlayerData.victoryShown && PlayerData.Inventory.Items.OfType<InventoryItem>().Any(i => i.Stats.ItemType == ItemType.CharismaCharm && !i.Processed))
            {
                EmitSignal(nameof(OnGameVictory));
                sceneItems.ShowVictoryPanel();
            }
            else
            {
                sceneItems.ShowMarketPanel();
            }

            player.ControlEnabled = false;
        };
    }

    void HandleLeaveMarketArea()
    {
        keyPrompt.Visible = false;
        queuedAction = null;
    }
}
