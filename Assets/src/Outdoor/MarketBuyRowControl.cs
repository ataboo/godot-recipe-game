using Godot;
using System;

public class MarketBuyRowControl : PanelContainer
{
    [Export]
    public NodePath buyButtonPath;
    [Export]
    public NodePath nameLabelPath;
    [Export]
    public NodePath priceLabelPath;

    private Button _buyButton;
    public Button BuyButton => _buyButton ?? (_buyButton = GetNode<Button>(buyButtonPath));
    private Label nameLabel;
    private Label priceLabel;

    public override void _Ready()
    {
        nameLabel = GetNode<Label>(nameLabelPath) ?? throw new NullReferenceException();
        priceLabel = GetNode<Label>(priceLabelPath) ?? throw new NullReferenceException();
    }

    public void SetItem(string name, int price)
    {
        nameLabel.Text = $"Recipe: {name}";
        priceLabel.Text = price.ToString();
    }
}
