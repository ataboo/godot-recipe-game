using Godot;
using RecipeGame.Inventory;
using RecipeGame.Models;
using System;
using static RecipeGame.Helpers.Enums;

public class ForagePanelControl : Control
{
    [Export]
    public NodePath inventoryPath;

    [Export]
    public NodePath forageButtonPath;
    
    [Export]
    public NodePath leaveButtonPath;

    [Export]
    public NodePath titlePath;

    private Label titleLabel;

    public StorageInventory Inventory { get; private set; }

    public override void _Ready()
    {
        titleLabel = GetNode<Label>(titlePath) ?? throw new NullReferenceException();
    }

    
    private InventoryGridControl _inventoryGrid;
    public InventoryGridControl InventoryGrid => _inventoryGrid ?? (_inventoryGrid = GetNode<InventoryGridControl>(inventoryPath));
    
    private Button _leaveButton;
    public Button LeaveButton => _leaveButton ?? (_leaveButton = GetNode<Button>(leaveButtonPath));

    private Button _forageButton;
    public Button ForageButton => _forageButton ?? (_forageButton = GetNode<Button>(forageButtonPath));

    public void InitBiome(BiomeType biome)
    {
        var biomeName = Enum.GetName(typeof(BiomeType), biome);
        titleLabel.Text = $"Ingredients From the {biomeName}";
    }
}
