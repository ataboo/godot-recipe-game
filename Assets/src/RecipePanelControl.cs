using Godot;
using RecipeGame.Helpers;
using RecipeGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;

public class RecipePanelControl : MarginContainer
{
    [Export]
    public float panelClosedYPos = 920f;
    [Export]
    public float panelMoveSpeed = 5f;
    [Export]
    public NodePath recipeHolderPath;
    [Export]
    public NodePath leftButtonPath;
    [Export]
    public NodePath rightButtonPath;
    [Export]
    public NodePath centerButtonPath;

    private Control recipeHolder;
    private float clickCooldown;

    private TextureButton leftButton;
    private TextureButton rightButton;
    private TextureButton centerButton;

    private RandomNumberGenerator rand;

    private float targetYPos;
    public override void _Ready()
    {
        rand = new RandomNumberGenerator();
        rand.Randomize();

        leftButton = this.MustGetNode<TextureButton>(leftButtonPath);
        rightButton = this.MustGetNode<TextureButton>(rightButtonPath);
        recipeHolder = this.MustGetNode<MarginContainer>(recipeHolderPath);
        centerButton = this.MustGetNode<TextureButton>(centerButtonPath);

        this.RectPosition = new Vector2(0, panelClosedYPos);
        targetYPos = panelClosedYPos;

        leftButton.Connect("pressed", this, nameof(HandleLeftButtonClick));
        rightButton.Connect("pressed", this, nameof(HandleRightButtonClick));
        centerButton.Connect("pressed", this, nameof(HandleClickPanel));

        UpdateRecipeMargins();
    }

    public override void _Process(float delta)
    {
        if(Math.Abs(this.RectPosition.y - targetYPos) > 1e-4)
        {
            this.RectPosition = this.RectPosition.LinearInterpolate(new Vector2(0, targetYPos), panelMoveSpeed * delta);
        }

        clickCooldown += delta;
    }

    public void UpdateUnlockedRecipes(PlayerData playerData)
    {
        foreach(RecipeCardControl child in recipeHolder.GetChildren())
        {
            child.Visible = !child.Purchasable || playerData.PurchasedRecipes[child.itemType];
        }
    }

    void HandleClickPanel()
    {
        if(clickCooldown < 0.5f)
        {
            return;
        }

        targetYPos = targetYPos == 0 ? panelClosedYPos : 0;
        clickCooldown = 0;

        UpdateRecipeMargins();
    }
    
    void HandleRightButtonClick()
    {
        ShiftRecipes(true);
    }

    void HandleLeftButtonClick()
    {
        ShiftRecipes(false);
    }

    private void ShiftRecipes(bool forward)
    {
        var visibleRecipes = VisibleRecipes().ToArray();
        if(visibleRecipes.Length < 2)
        {
            return;
        }

        var targetRecipe = forward ? visibleRecipes[visibleRecipes.Length-1] : visibleRecipes[1];
        while(recipeHolder.GetChild(0) != targetRecipe)
        {
            if(forward)
            {
                // Move last index to front, i.e physicall taking front recipe to back.
                recipeHolder.MoveChild(recipeHolder.GetChild(recipeHolder.GetChildCount()-1), 0);
            }
            else
            {
                // Move first index to back, i.e physically taking back recipe to front.
                recipeHolder.MoveChild(recipeHolder.GetChild(0), recipeHolder.GetChildCount()-1);
            }
        }

        UpdateRecipeMargins();
    }
    
    private IEnumerable<Control> VisibleRecipes()
    {
        return recipeHolder.GetChildren().AsEnumerable<Control>().Where(c => c.Visible);
    }

    private void UpdateRecipeMargins()
    {
        foreach(RecipeCardControl child in recipeHolder.GetChildren())
        {
            var xOffset = rand.RandiRange(0, 16);
            var yOffset = rand.RandiRange(0, 16);

            child.AddConstantOverride("margin_top", -yOffset);
            child.AddConstantOverride("margin_left", -xOffset);
            child.AddConstantOverride("margin_bottom", yOffset);
            child.AddConstantOverride("margin_right", xOffset);
        }
    }
}
