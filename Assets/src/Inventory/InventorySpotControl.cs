using Godot;
using System;

public class InventorySpotControl : NinePatchRect
{
    [Export]
    public NodePath iconPath;

    private InventoryIconControl _iconControl;

    public InventoryIconControl IconControl => _iconControl ?? (_iconControl = GetNode<InventoryIconControl>(iconPath));
}
