using Godot;
using System;
using static RecipeGame.Helpers.Enums;

public class ToolTableControl : Sprite
{
    [Export]
    public NodePath scoopAreaPath;
    [Export]
    public NodePath slottedAreaPath;
    [Export]
    public NodePath tableAreaPath;
    [Export]
    public NodePath bowlPath;

    [Signal]
    public delegate void OnToolClicked(HeldTool tool, bool leftClick);

    private Area2D scoopArea;
    private Area2D slottedArea;
    private Area2D tableArea;
    private Area2D bowlArea;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        scoopArea = GetNode<Area2D>(scoopAreaPath) ?? throw new NullReferenceException();
        slottedArea = GetNode<Area2D>(slottedAreaPath) ?? throw new NullReferenceException();
        tableArea = GetNode<Area2D>(tableAreaPath) ?? throw new NullReferenceException();
        bowlArea = GetNode<Area2D>(bowlPath) ?? throw new NullReferenceException();

        scoopArea.Connect("input_event", this, nameof(HandleToolClicked), new Godot.Collections.Array{HeldTool.Scoop});
        slottedArea.Connect("input_event", this, nameof(HandleToolClicked), new Godot.Collections.Array{HeldTool.SlottedSpoon});
        bowlArea.Connect("input_event", this, nameof(HandleToolClicked), new Godot.Collections.Array{HeldTool.Bowl});
        tableArea.Connect("input_event", this, nameof(HandleToolClicked), new Godot.Collections.Array{HeldTool.Empty});
    }

    public void SetHeldTool(HeldTool tool)
    {
        scoopArea.Visible = tool != HeldTool.Scoop;
        slottedArea.Visible = tool != HeldTool.SlottedSpoon;
        bowlArea.Visible = tool != HeldTool.Bowl;
    }

    void HandleToolClicked(Node vp, InputEvent e, int shapeIdx, HeldTool tool)
    {
        if(e is InputEventMouseButton mEvent && mEvent.Pressed)
        {
            switch((ButtonList)mEvent.ButtonIndex)
            {
                case ButtonList.Left:
                    EmitSignal(nameof(OnToolClicked), tool, true);
                    break;
                case ButtonList.Right:
                    EmitSignal(nameof(OnToolClicked), tool, false);
                    break;
                default:
                    break;
            }
        }
    }
}
