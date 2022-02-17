using Godot;
using RecipeGame.Inventory;
using RecipeGame.Models;
using System;
using static RecipeGame.Helpers.Enums;

public class OutdoorSceneControl : Node2D
{
    [Signal]
    public delegate void OnTransitionToCottage();

    [Export]
    public NodePath promptPath;

    private CauldronService cauldronService;

    private OutdoorSceneItemControl sceneItems;

    private KeyPromptControl keyPrompt;

    public PlayerData PlayerData { get; set; }

    private Action queuedAction;

    private PlayerMapController player;

    public override void _Ready()
    {
        cauldronService = new CauldronService();
        keyPrompt = GetNode<KeyPromptControl>(promptPath) ?? throw new NullReferenceException();
        sceneItems = GetNode<OutdoorSceneItemControl>("OutdoorSceneItems") ?? throw new NullReferenceException();
        player = GetNode<PlayerMapController>("Player") ?? throw new NullReferenceException();

        var triggers = GetNode<Node>("Triggers");
        var forest = triggers.GetNode<BiomeAreaControl>("ForestArea");
        forest.Connect(nameof(BiomeAreaControl.OnBiomeAreaEnter), this, nameof(HandleEnterBiomeArea));
        forest.Connect(nameof(BiomeAreaControl.OnBiomeAreaExit), this, nameof(HandleExitBiomeArea));
        var cave = triggers.GetNode<BiomeAreaControl>("CaveArea");
        cave.Connect(nameof(BiomeAreaControl.OnBiomeAreaEnter), this, nameof(HandleEnterBiomeArea));
        cave.Connect(nameof(BiomeAreaControl.OnBiomeAreaExit), this, nameof(HandleExitBiomeArea));
        var swamp = triggers.GetNode<BiomeAreaControl>("SwampArea");
        swamp.Connect(nameof(BiomeAreaControl.OnBiomeAreaEnter), this, nameof(HandleEnterBiomeArea));
        swamp.Connect(nameof(BiomeAreaControl.OnBiomeAreaExit), this, nameof(HandleExitBiomeArea));
        var shore = triggers.GetNode<BiomeAreaControl>("ShoreArea");
        shore.Connect(nameof(BiomeAreaControl.OnBiomeAreaEnter), this, nameof(HandleEnterBiomeArea));
        shore.Connect(nameof(BiomeAreaControl.OnBiomeAreaExit), this, nameof(HandleExitBiomeArea));

        sceneItems.Connect(nameof(OutdoorSceneItemControl.OnLeaveForage), this, nameof(HandleLeaveForage));
        sceneItems.Init(PlayerData);
    }

    public override void _Process(float delta)
    {
        if(PlayerData?.Cauldron == null) {
            GD.PushError("No cauldron to update!");
            return;
        }

        cauldronService.Update(PlayerData.Cauldron, delta);

        if(queuedAction != null && Input.IsActionJustPressed("perform_action"))
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

        keyPrompt.SetText($"E: Enter {biomeName}");
        keyPrompt.Visible = true;
        queuedAction = () => {
            sceneItems.ShowForageBiome(biome);
            player.ControlEnabled = false;
        };
    }

    void HandleExitBiomeArea(BiomeType biome)
    {
        keyPrompt.Visible = false;
        queuedAction = null;
    }

    void HandleLeaveForage()
    {
        player.ControlEnabled = true;
    }
}
