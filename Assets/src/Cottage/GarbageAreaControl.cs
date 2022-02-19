using System;
using Godot;

public class GarbageAreaControl : Area2D
{
    [Signal]
    public delegate void OnClickGarbage(bool leftClick);

    [Signal]
    public delegate void OnConfirm(bool yes);

    [Export]
    public NodePath confirmPanelPath;
    [Export]
    public NodePath confirmYesButtonPath;
    [Export]
    public NodePath confirmNoButtonPath;
    [Export]
    public NodePath confirmTitlePath;

    private PanelContainer confirmPanel;
    private Button confirmYesButton;
    private Button confirmNoButton;
    private Label confirmTitle;


    public override void _Ready()
    {
        confirmPanel = GetNode<PanelContainer>(confirmPanelPath) ?? throw new NullReferenceException();
        confirmYesButton = GetNode<Button>(confirmYesButtonPath) ?? throw new NullReferenceException();
        confirmNoButton = GetNode<Button>(confirmNoButtonPath) ?? throw new NullReferenceException();
        confirmTitle = GetNode<Label>(confirmTitlePath) ?? throw new NullReferenceException();

        Connect("input_event", this, nameof(HandleInputEvent));
        confirmYesButton.Connect("pressed", this, nameof(HandlePushedConfirmButton), new Godot.Collections.Array{true});
        confirmNoButton.Connect("pressed", this, nameof(HandlePushedConfirmButton), new Godot.Collections.Array{false});
    }

    public void ShowConfirm(bool visible, string title)
    {
        confirmTitle.Text = title;
        confirmPanel.MouseFilter = visible ? Control.MouseFilterEnum.Stop : Control.MouseFilterEnum.Ignore;
        confirmPanel.Visible = visible;
    }

    void HandlePushedConfirmButton(bool yes)
    {
        EmitSignal(nameof(OnConfirm), yes);
    }

    void HandleInputEvent(Node node, InputEvent iEvent, int shapeIdx)
    {
        if(iEvent is InputEventMouseButton mEvent && mEvent.Pressed)
        {
            switch((ButtonList)mEvent.ButtonIndex)
            {
                case ButtonList.Left:
                    EmitSignal(nameof(OnClickGarbage), true);
                    break;
                case ButtonList.Right:
                    EmitSignal(nameof(OnClickGarbage), false);
                    break;
                default:
                    break;
            }
        }
    }
}
