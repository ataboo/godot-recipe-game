using Godot;
using RecipeGame.Inventory;
using RecipeGame.Models;
using System;
using static RecipeGame.Helpers.Enums;

public class OutdoorSceneControl : Node2D
{
    [Export]
    public NodePath satchelPath;
    [Export]
    public NodePath foragePath;
    [Export]
    public NodePath promptPath;

    private GameRoot gameRoot;
    private CauldronService cauldronService;

    private ForagePanelControl foragePanel;
    private KeyPromptControl keyPrompt;
    private InventoryGridControl satchel;

    public PlayerData PlayerData { get; set; }

    private Action queuedAction;

    public override void _Ready()
    {
        cauldronService = new CauldronService();
        gameRoot = GetNode<GameRoot>("/root/GameRoot") ?? throw new NullReferenceException();
        foragePanel = GetNode<ForagePanelControl>(foragePath) ?? throw new NullReferenceException();
        satchel = GetNode<InventoryGridControl>(satchelPath) ?? throw new NullReferenceException();
        keyPrompt = GetNode<KeyPromptControl>(promptPath) ?? throw new NullReferenceException();

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
            gameRoot.TransitionToCottage();
        }
    }

    void HandleEnterBiomeArea(BiomeType biome)
    {
        GD.Print("Fired enter.");

        var biomeName = Enum.GetName(typeof(BiomeType), biome);

        keyPrompt.SetText($"E: Enter {biomeName}");
        keyPrompt.Visible = true;
        queuedAction = () => {GD.Print($"Do {biomeName} thing.");};
    }

    void HandleExitBiomeArea(BiomeType biome)
    {
        keyPrompt.Visible = false;
        queuedAction = null;
    }
}
