using System;
using Godot;
using RecipeGame.Models;

public class CottageSceneControl : Node2D
{
    [Export]
    public NodePath keyPromptPath;
    [Export]
    public NodePath itemControlPath;
    [Export]
    public NodePath playerControlPath;

    [Signal]
    public delegate void OnTransitionToMap();

    public PlayerData PlayerData { get; set; }

    private CottagePlayerController playerControl;
    private KeyPromptControl keyPromptContainer;
    private Action queuedPromptAction = null;
    private CottageSceneItemControl itemControl;

    public override void _Ready()
    {
        keyPromptContainer = GetNode<KeyPromptControl>(keyPromptPath) ?? throw new NullReferenceException();
        itemControl = GetNode<CottageSceneItemControl>(itemControlPath) ?? throw new NullReferenceException();
        playerControl = GetNode<CottagePlayerController>(playerControlPath) ?? throw new NullReferenceException();

        itemControl.Init(PlayerData);
        itemControl.Connect(nameof(CottageSceneItemControl.OnLeaveUIPanels), this, nameof(HandleLeaveUIPanels));
    }

    public override void _Process(float delta)
    {
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
            EmitSignal(nameof(OnTransitionToMap));
        }
    }

    void HandleLeaveUIPanels()
    {
        playerControl.ControlEnabled = true;
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
        QueueKeyPromptAction("Open Storage", () => {
            itemControl.ShowStoragePanel();
            playerControl.ControlEnabled = false;
        });
    }

    void OnExitStorageTrigger(Node other)
    {
        ClearKeyPromptAction();
    }

    void OnEnterCauldronTrigger(Node other)
    {
        QueueKeyPromptAction("Use Cauldron", () => {
            itemControl.ShowCauldron();
            playerControl.ControlEnabled = false;
        });
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
