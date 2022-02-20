using Godot;
using RecipeGame.Helpers;
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
    public NodePath choppedLabelPath;
    [Export]
    public NodePath buttonPath;
    [Export]
    public NodePath textureDictionaryPath;


    private TextureButton button;
    private Label countLabel;
    private ProgressBar progressBar;
    private TextureRect choppedLabel;
    private ItemTextureDictionary textureDictionary;

    public override void _Ready()
    {
        button = GetNode<TextureButton>(buttonPath) ?? throw new NullReferenceException();
        countLabel = GetNode<Label>(countLabelPath) ?? throw new NullReferenceException();
        progressBar = GetNode<ProgressBar>(progressBarPath) ?? throw new NullReferenceException();
        choppedLabel = GetNode<TextureRect>(choppedLabelPath) ?? throw new NullReferenceException();
        textureDictionary = this.MustGetNode<ItemTextureDictionary>(textureDictionaryPath);
        button.Connect("gui_input", this, "OnGuiInput");
    }

    public void SetItem(InventoryItem item)
    {
        if (item == null)
        {
            button.HintTooltip = "";
            button.Modulate = new Color(button.Modulate, 0);
            progressBar.Visible = false;
            countLabel.Visible = false;
            choppedLabel.Visible = false;

            return;
        }

        button.Modulate = new Color(button.Modulate, 1);
        var iconName = item.Stats.Name;
        button.HintTooltip = item.DisplayName;
        button.TextureNormal = textureDictionary.GetTexture(item.Stats.ItemType);
        choppedLabel.Visible = item.Processed;

        switch (item.Stats.DisplayMode)
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
        if (iEvent is InputEventMouseButton mEvent && mEvent.Pressed)
        {
            switch ((ButtonList)mEvent.ButtonIndex)
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
