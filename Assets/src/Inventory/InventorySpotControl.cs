using Godot;
using RecipeGame.Models;
using System;
using static RecipeGame.Helpers.Enums;

public class InventorySpotControl : NinePatchRect
{
    [Signal]
    public delegate void OnLeftPress(int itemIdx);

    [Signal]
    public delegate void OnRightPress(int itemIdx);

    [Export]
    public NodePath countLabelPath;

    [Export]
    public NodePath progressBarPath;

    [Export]
    public NodePath buttonPath;


    private TextureButton button;
    private Label countLabel;
    private ProgressBar progressBar;
    
    private int itemIndex;

    public override void _Ready()
    {
        button = GetNode<TextureButton>(buttonPath) ?? throw new NullReferenceException();
        countLabel = GetNode<Label>(countLabelPath) ?? throw new NullReferenceException();
        progressBar = GetNode<ProgressBar>(progressBarPath) ?? throw new NullReferenceException();

        button.Connect("gui_input", this, "OnGuiInput");

        itemIndex = GetIndex();
    }

    public void SetItem(InventoryItem item)
    {
        if(item == null) 
        {
            button.TextureNormal = null;
            progressBar.Visible = false;
            countLabel.Visible = false;

            return;
        }
         
        var iconPath = $"res://Art/UI/Items/Mushroom.png";
    
        // var iconPath = $"res://Art/UI/Items/{item.Stats.Name}.png";
        var icon = GD.Load<Texture>(iconPath);
        if(icon == null) 
        {
            throw new NullReferenceException();
        }
        button.TextureNormal = icon;
        

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
                progressBar.Value = (double)item.StackAmount / item.Stats.StackSize;
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
                    EmitSignal(nameof(OnLeftPress), itemIndex);
                    break;
                case ButtonList.Right:
                    EmitSignal(nameof(OnRightPress), itemIndex);
                    break;
            }
        }
    }
}
