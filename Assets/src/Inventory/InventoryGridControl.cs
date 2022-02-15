using Godot;
using RecipeGame.Models;
using System;

public class InventoryGridControl : PanelContainer
{
    [Signal]
    public delegate void OnItemLeftPress(int itemIdx);

    [Signal]
    public delegate void OnItemRightPress(int itemIdx);

    [Export]
    public NodePath gridContainerPath;

    private GridContainer gridContainer;

    public override void _Ready()
    {
        gridContainer = GetNode<GridContainer>(gridContainerPath);
    }

    public void SetInventoryData(IInventory inventory)
    {
        for(int i=0; i<inventory.Items.Length; i++)
        {
            var gridItem = gridContainer.GetChild<InventorySpotControl>(i);
            gridItem.SetItem(inventory.Items[i]);

            gridItem.Connect(nameof(InventorySpotControl.OnLeftPress), this, nameof(HandleItemLeftClick));
            gridItem.Connect(nameof(InventorySpotControl.OnRightPress), this, nameof(HandleItemRightClick));
        }
    }

    void HandleItemLeftClick(int index)
    {
        EmitSignal(nameof(OnItemLeftPress), index);
    }

    void HandleItemRightClick(int index)
    {
        EmitSignal(nameof(OnItemRightPress), index);
    }
}
