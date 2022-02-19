using Godot;
using RecipeGame.Models;
using System;
using static RecipeGame.Helpers.Enums;

public class CursorItemIcon : CenterContainer
{   
    [Export]
    public NodePath scoopIconPath;
    [Export]
    public NodePath slottedIconPath;
    [Export]
    public NodePath bowlIconPath;
    [Export]
    public NodePath iconPath;

    private TextureRect scoopIcon;
    private TextureRect slottedIcon;
    private TextureRect bowlIcon;
    private InventoryIconControl iconControl; 

    public override void _Ready()
    {
        IgnoreMouseRecurse(this);
        scoopIcon = GetNode<TextureRect>(scoopIconPath);
        slottedIcon = GetNode<TextureRect>(slottedIconPath);
        bowlIcon = GetNode<TextureRect>(bowlIconPath);
        iconControl = GetNode<InventoryIconControl>(iconPath);
    }

    public void SetHeldItem(InventoryItem item, HeldTool tool)
    {
        iconControl.Visible = item != null;
        iconControl.SetItem(item);
        scoopIcon.Visible = tool == HeldTool.Scoop;
        slottedIcon.Visible = tool == HeldTool.SlottedSpoon;
        bowlIcon.Visible = tool == HeldTool.Bowl;
    }

    private void IgnoreMouseRecurse(Control node)
    {
        node.MouseFilter = MouseFilterEnum.Ignore;

        foreach(var child in node.GetChildren())
        {
            if(child is Control cntChild)
            {
                IgnoreMouseRecurse(cntChild);
            }
        }
    }

    public void SetTool()
    {
        throw new NotImplementedException();
    }

    public override void _Process(float delta)
    {
        if(Visible)
        {
            RectPosition = GetViewport().GetMousePosition() - new Vector2(RectSize.x / 2, RectSize.y/2);
        } 
    }
}
