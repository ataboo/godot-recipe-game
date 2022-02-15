using Godot;
using RecipeGame.Models;
using System;

public class ForagePanelControl : Control
{
    [Signal]
    public delegate void OnItemLeftClick(int index);

    [Signal]
    public delegate void OnItemRightClick(int index);

    [Signal]
    public delegate void OnLeaveClick();

    [Export]
    public NodePath inventoryPath;

    [Export]
    public NodePath forageButtonPath;
    
    [Export]
    public NodePath leaveButtonPath;

    private InventoryGridControl inventoryGrid;

    private Button forageButton;

    private Button leaveButton;

    public override void _Ready()
    {
        inventoryGrid = GetNode<InventoryGridControl>(inventoryPath) ?? throw new NullReferenceException();
        forageButton = GetNode<Button>(forageButtonPath) ?? throw new NullReferenceException();
        leaveButton = GetNode<Button>(leaveButtonPath) ?? throw new NullReferenceException();

        inventoryGrid.Connect(nameof(InventoryGridControl.OnItemLeftPress), this, nameof(HandleItemLeftClick));
        inventoryGrid.Connect(nameof(InventoryGridControl.OnItemRightPress), this, nameof(HandleItemRightClick));
    }

    public void SetInventoryData(IInventory inventory)
    {
        inventoryGrid.SetInventoryData(inventory);
    }

    void HandleItemLeftClick(int index)
    {
        EmitSignal(nameof(OnItemLeftClick), index);
    }

    void HandleItemRightClick(int index)
    {
        
        EmitSignal(nameof(OnItemRightClick), index);
    }
}
