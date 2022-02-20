using Godot;
using RecipeGame.Models;

public class InventoryGridControl : Control
{
    [Signal]
    public delegate void OnItemLeftPress(int itemIdx);

    [Signal]
    public delegate void OnItemRightPress(int itemIdx);

    [Export]
    public NodePath gridContainerPath;
    [Export]
    public NodePath sortButtonPath;

    private GridContainer gridContainer;

    private Button _sortButton;
    public Button SortButton => _sortButton ?? (_sortButton = GetNode<Button>(sortButtonPath));

    public override void _Ready()
    {
        gridContainer = GetNode<GridContainer>(gridContainerPath);

        foreach(var child in gridContainer.GetChildren())
        {
            if(child is InventorySpotControl gridItem)
            {
                gridItem.IconControl.Connect(nameof(InventoryIconControl.OnLeftPress), this, nameof(HandleItemLeftClick), new Godot.Collections.Array{gridItem.GetIndex()});
                gridItem.IconControl.Connect(nameof(InventoryIconControl.OnRightPress), this, nameof(HandleItemRightClick), new Godot.Collections.Array{gridItem.GetIndex()});
            }
        }
    }

    public void SetItems(InventoryItem[] items)
    {
        var idx = 0;
        foreach(var child in gridContainer.GetChildren())
        {
            if(child is InventorySpotControl gridItem)
            {
                var dataItem = items.Length <= idx ? null : items[idx];
                gridItem.IconControl.SetItem(dataItem);
                idx++;
            }
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
