using Godot;
using RecipeGame.Models;

public class PrepTablePanel : Node2D
{
    [Export]
    public NodePath processButtonPath;
    [Export]
    public NodePath leaveButtonPath;
    [Export]
    public NodePath inputSpotPath;
    [Export]
    public NodePath outputSpotPath;

    private Button _processButton;
    public Button ProcessButton => _processButton ?? (_processButton = GetNode<Button>(processButtonPath));
    
    private Button _leaveButton;
    public Button LeaveButton => _leaveButton ?? (_leaveButton = GetNode<Button>(leaveButtonPath));

    private InventorySpotControl _inputSpot;
    public InventorySpotControl InputSpot => _inputSpot ?? (_inputSpot = GetNode<InventorySpotControl>(inputSpotPath));

    private InventorySpotControl _outputSpot;
    public InventorySpotControl OutputSpot => _outputSpot ?? (_outputSpot = GetNode<InventorySpotControl>(outputSpotPath));

    public void SetItems(InventoryItem inputItem, InventoryItem outputItem, bool canProcess)
    {
        InputSpot.IconControl.SetItem(inputItem);
        OutputSpot.IconControl.SetItem(outputItem);

        ProcessButton.Disabled = !canProcess;
    }
}
