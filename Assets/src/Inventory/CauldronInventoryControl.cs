using System;
using System.Linq;
using Godot;
using RecipeGame.Helpers;
using RecipeGame.Inventory;
using RecipeGame.Models;
using static RecipeGame.Helpers.Enums;

public class CauldronInventoryControl : Area2D
{
    [Signal]
    public delegate void OnClickCauldron(bool leftClick);
    [Signal]
    public delegate void OnClickCauldronProduct(int index);

    [Export]
    public PackedScene gridItemPrefab;
    [Export]
    public Texture[] blueLiquidPrefabs;
    [Export]
    public Texture[] greenLiquidPrefabs;
    [Export]
    public Texture dryLiquidPrefab;
    [Export]
    public NodePath liquidSpritePath;
    [Export]
    public NodePath itemGridPath;
    [Export]
    public NodePath flameControlPath;
    [Export]
    public NodePath bubbleControlPath;

    private GridContainer itemGrid;

    private Sprite liquidSprite;

    private int lastFillLevel;

    private CauldronBubbleControl bubbleControl;

    private CauldronLiquidColor lastLiquidColor;

    private CauldronFlameControl flameControl;

    private float updateClock = 1e7f;

    public override void _Ready()
    {
        Connect("input_event", this, nameof(HandleInputEvent));
        itemGrid = GetNode<GridContainer>(itemGridPath) ?? throw new NullReferenceException();
        liquidSprite = GetNode<Sprite>(liquidSpritePath) ?? throw new NullReferenceException();
        flameControl = GetNode<CauldronFlameControl>(flameControlPath) ?? throw new NullReferenceException();
        bubbleControl = GetNode<CauldronBubbleControl>(bubbleControlPath) ?? throw new NullReferenceException();

        if(blueLiquidPrefabs.Length != 4)
        {
            GD.PushError("Unnexpected cound of blue liquid textures!");
        }
        if(greenLiquidPrefabs.Length != 4)
        {
            GD.PushError("Unnexpected cound of blue liquid textures!");
        }
    }

    public void UpdateVisuals(Cauldron cauldron, float delta)
    {
        if((updateClock += delta) < 0.25f)
        {
            return;
        }

        updateClock = 0f;

        var fillLevel = 0;
        var liquidColor = CauldronLiquidColor.Dry;

        if(cauldron.Products.Count == 0)
        {
            fillLevel = Mathf.Clamp((int)Mathf.Ceil((float)cauldron.IngredientVolume * 4 / InventoryService.CauldronVolumeCapacity), 0, 4);
            if(cauldron.CurrentRecipe != null)
            {
                liquidColor = CauldronLiquidColor.Green;
            }
            else
            {
                liquidColor = CauldronLiquidColor.Blue; 
            }
        }

        if(liquidColor != lastLiquidColor || fillLevel != lastFillLevel)
        {
            if(fillLevel == 0)
            {
                liquidSprite.Texture = null;
            }
            else if(liquidColor == CauldronLiquidColor.Dry) 
            {
                liquidSprite.Texture = dryLiquidPrefab;
            }
            else if(liquidColor == CauldronLiquidColor.Blue)
            {
                liquidSprite.Texture = blueLiquidPrefabs[fillLevel];
            }
            else
            {
                liquidSprite.Texture = greenLiquidPrefabs[fillLevel];
            }

            lastLiquidColor = liquidColor;
            lastFillLevel = fillLevel;
        }

        flameControl.UpdateFlameRate(cauldron);
        bubbleControl.UpdateBubbles(cauldron);

        GD.Print($"Cauldron Heat: {cauldron.HeatLevel}, Temp: {cauldron.Temperature}");
    }

    public void UpdateOutputItems(Cauldron cauldron)
    {
        foreach(var gridSquare in itemGrid.GetChildren().AsEnumerable<Control>().ToArray())
        {
            itemGrid.RemoveChild(gridSquare);
            gridSquare.QueueFree();
        }

        foreach(var item in cauldron.Products)
        {
            var newSquare = gridItemPrefab.Instance<InventoryIconControl>();
            itemGrid.AddChild(newSquare);
            newSquare.SetItem(item);
            
            newSquare.Connect(nameof(InventoryIconControl.OnLeftPress), this, nameof(HandleItemClicked), new Godot.Collections.Array{newSquare});
        }
    }

    public void Init(Cauldron cauldron)
    {
        UpdateOutputItems(cauldron);
        UpdateVisuals(cauldron, 0);
    }

    void HandleInputEvent(Node node, InputEvent iEvent, int shapeIdx)
    {
        if(iEvent is InputEventMouseButton mEvent && mEvent.Pressed)
        {
            switch((ButtonList)mEvent.ButtonIndex)
            {
                case ButtonList.Left:
                    EmitSignal(nameof(OnClickCauldron), true);
                    break;
                case ButtonList.Right:
                    EmitSignal(nameof(OnClickCauldron), false);
                    break;
                default:
                    break;
            }
        }
    }

    void HandleItemClicked(Node2D square)
    {
        EmitSignal(nameof(OnClickCauldronProduct), square.GetIndex());
    }
}
