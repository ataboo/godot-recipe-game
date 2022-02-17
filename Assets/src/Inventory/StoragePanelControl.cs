using Godot;

public class StoragePanelControl : Control
{
    [Export]
    public NodePath leaveButtonPath;
    [Export]
    public NodePath inventoryGridPath;

    private InventoryGridControl _inventoryGrid;
    public InventoryGridControl InventoryGrid => _inventoryGrid ?? (_inventoryGrid = GetNode<InventoryGridControl>(inventoryGridPath));

    private Button _leaveButton;
    public Button LeaveButton => _leaveButton ?? (_leaveButton = GetNode<Button>(leaveButtonPath));
}
