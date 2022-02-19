using Godot;
using System;

public class CauldronPanelControl : Node2D
{
    [Export]
    public NodePath leaveButtonPath;

    private Button _leaveButton;
    public Button LeaveButton => _leaveButton ?? (_leaveButton = GetNode<Button>(leaveButtonPath));
}
