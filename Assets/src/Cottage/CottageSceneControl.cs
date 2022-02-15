using System;
using Godot;
using RecipeGame.Inventory;
using RecipeGame.Models;

public class CottageSceneControl : Node2D
{
    [Export]
    public NodePath satchelPath;

    [Export]
    public NodePath storagePath;

    [Export]
    public NodePath keyPromptPath;

    public PlayerData PlayerData { get; set; }
    private InventoryGridControl satchel;
    private InventoryGridControl storagePanel;
    private GameRoot gameRoot;
    private CauldronService cauldronService;
    private KeyPromptControl keyPromptContainer;
    private Action queuedPromptAction = null;

    public override void _Ready()
    {
        cauldronService = new CauldronService();
        gameRoot = GetNode<GameRoot>("/root/GameRoot") ?? throw new NullReferenceException();
        satchel = GetNode<InventoryGridControl>(satchelPath);
        storagePanel = GetNode<InventoryGridControl>(storagePath);
        keyPromptContainer = GetNode<KeyPromptControl>(keyPromptPath);
        
        satchel.SetInventoryData(PlayerData.Inventory);
        storagePanel.SetInventoryData(PlayerData.Storage);
    }

    public override void _Process(float delta)
    {
        if(PlayerData?.Cauldron == null) {
            GD.PushError("No cauldron to update!");
            return;
        }

        cauldronService.Update(PlayerData.Cauldron, delta);

        if(queuedPromptAction != null && Input.IsActionJustPressed("perform_action"))
        {
            queuedPromptAction();
            ClearKeyPromptAction();
        }
    }

    void OnHitExitTrigger(Node other)
    {
        if(other is CottagePlayerController) 
        {
            gameRoot.TransitionToMap();
        }
    }

    void OnEnterBenchTrigger(Node other)
    {
        QueueKeyPromptAction("Use Prep Bench", () => {GD.Print("Did bench thing!");});
    }

    void OnExitBenchTrigger(Node other)
    {
        ClearKeyPromptAction();
    }

    void OnEnterStorageTrigger(Node other)
    {
        QueueKeyPromptAction("Open Storage", () => {GD.Print("Did storage thing!");});
    }

    void OnExitStorageTrigger(Node other)
    {
        ClearKeyPromptAction();
    }

    void OnEnterCauldronTrigger(Node other)
    {
        QueueKeyPromptAction("Use Cauldron", () => {GD.Print("Did cauldron thing!");});
    }

    void OnExitCauldronTrigger(Node other)
    {
        ClearKeyPromptAction();
    }

    private void QueueKeyPromptAction(string prompt, Action action)
    {
        keyPromptContainer.SetText($"E - {prompt}");
        keyPromptContainer.Visible = true;
        queuedPromptAction = action;
    }

    private void ClearKeyPromptAction()
    {
        keyPromptContainer.Visible = false;
        queuedPromptAction = null;
    }
}
