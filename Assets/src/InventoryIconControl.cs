using Godot;
using RecipeGame.Models;
using System;
using static RecipeGame.Helpers.Enums;

public class InventoryIconControl : VBoxContainer
{
    [Signal]
    public delegate void OnLeftPress();

    [Signal]
    public delegate void OnRightPress();

    [Export]
    public NodePath countLabelPath;

    [Export]
    public NodePath progressBarPath;

    [Export]
    public NodePath buttonPath;


    private TextureButton button;
    private Label countLabel;
    private ProgressBar progressBar;

    public override void _Ready()
    {
        button = GetNode<TextureButton>(buttonPath) ?? throw new NullReferenceException();
        countLabel = GetNode<Label>(countLabelPath) ?? throw new NullReferenceException();
        progressBar = GetNode<ProgressBar>(progressBarPath) ?? throw new NullReferenceException();

        button.Connect("gui_input", this, "OnGuiInput");
    }

    public void SetItem(InventoryItem item)
    {
        var iconName = "Empty";

        if(item != null)
        {
            iconName = "Mushroom";
        }
        
        var iconPath = $"res://Art/UI/Items/{iconName}.png";
        var icon = GD.Load<Texture>(iconPath);
        if(icon == null) 
        {
            throw new NullReferenceException();
        }
        button.TextureNormal = icon;

        if(item == null) 
        {
            progressBar.Visible = false;
            countLabel.Visible = false;

            return;
        }

        switch(item.Stats.DisplayMode)
        {
            case ItemDisplayMode.Discrete:
                countLabel.Visible = true;
                progressBar.Visible = false;
                countLabel.Text = $"{item.StackAmount} / {item.Stats.StackSize}";
                break;
            case ItemDisplayMode.Bar:
                countLabel.Visible = false;
                progressBar.Visible = true;
                progressBar.MinValue = 0;
                progressBar.MaxValue = item.Stats.StackSize;
                progressBar.Value = item.StackAmount;
                break;
            case ItemDisplayMode.Single:
                countLabel.Visible = false;
                progressBar.Visible = false;
                break;
        }
    }

    void OnGuiInput(InputEvent iEvent)
    {
        if(iEvent is InputEventMouseButton mEvent && mEvent.Pressed)
        {
            switch((ButtonList)mEvent.ButtonIndex)
            {
                case ButtonList.Left:
                    EmitSignal(nameof(OnLeftPress));
                    break;
                case ButtonList.Right:
                    EmitSignal(nameof(OnRightPress));
                    break;
                default:
                    break;
            }
        }
    }
}
