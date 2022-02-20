using Godot;
using RecipeGame.Helpers;
using RecipeGame.Inventory;
using RecipeGame.Models;
using System;
using System.Linq;
using static RecipeGame.Helpers.Enums;

public class MarketPanelControl : PanelContainer
{
    [Signal]
    public delegate void OnBoughtRecipe(ItemType type);

    [Export]
    public PackedScene buyRowPrefab;
    [Export]
    public NodePath buySellTabsPath;
    [Export]
    public NodePath leaveButtonPath;
    [Export]
    public NodePath sellGridPath;
    [Export]
    public NodePath sellButtonPath;
    [Export]
    public NodePath buyRowHolderPath;
    [Export]
    public NodePath coinBalancePath;
    [Export]
    public NodePath sellValuePath;
    
    private TabContainer buySellTabs;
    private VBoxContainer buyRowHolder;
    private Label coinBalanceLabel;
    private Label sellValueLabel;
    
    private Button _sellButton;
    public Button SellButton => _sellButton ?? (_sellButton = GetNode<Button>(sellButtonPath));

    private Button _leaveButton;
    public Button LeaveButton => _leaveButton ?? (_leaveButton = GetNode<Button>(leaveButtonPath));

    private InventoryGridControl _marketSellGrid;
    public InventoryGridControl MarketSellGrid => _marketSellGrid ?? (_marketSellGrid = GetNode<InventoryGridControl>(sellGridPath));

    public override void _Ready()
    {
        buySellTabs = GetNode<TabContainer>(buySellTabsPath) ?? throw new NullReferenceException();
        buyRowHolder = GetNode<VBoxContainer>(buyRowHolderPath) ?? throw new NullReferenceException();
        coinBalanceLabel = GetNode<Label>(coinBalancePath) ?? throw new NullReferenceException();
        sellValueLabel = GetNode<Label>(sellValuePath) ?? throw new NullReferenceException();
    }

    public void SetRecipeRows(PlayerData playerData)
    {
        var children = buyRowHolder.GetChildren().AsEnumerable<Control>().ToArray();
        foreach(var child in children)
        {
            buyRowHolder.RemoveChild(child);
            child.QueueFree();
        }

        var unpurchasedRecipeTypes = playerData.PurchasedRecipes.Where(kvp => !kvp.Value).Select(kvp => kvp.Key);
        foreach(var recipeType in unpurchasedRecipeTypes)
        {
            var productStats = StatsDefinitions.MappedItemStats.Value[recipeType];
            var recipeStats = StatsDefinitions.MappedRecipeStats.Value[recipeType];

            var buyRow = buyRowPrefab.Instance<MarketBuyRowControl>();
            buyRowHolder.AddChild(buyRow);
            buyRow.SetItem(productStats.Name, recipeStats.Price.Value);
            buyRow.BuyButton.Disabled = playerData.CoinBalance < recipeStats.Price.Value;

            buyRow.BuyButton.Connect("pressed", this, nameof(HandleBuyItemPress), new Godot.Collections.Array{recipeType});
        }
    }

    public void SetSellGridItems(InventoryItem[] items, int sellValue)
    {
        MarketSellGrid.SetItems(items);
        SellButton.Disabled = !items.OfType<InventoryItem>().Any();
        sellValueLabel.Text = $"Sell Price: {sellValue}";
    }

    public void SetCoinBalance(int coinBalance)
    {
        coinBalanceLabel.Text = $"Purse: {coinBalance}";
    }

    void HandleBuyItemPress(ItemType recipeType)
    {
        EmitSignal(nameof(OnBoughtRecipe), recipeType);
    }
}
