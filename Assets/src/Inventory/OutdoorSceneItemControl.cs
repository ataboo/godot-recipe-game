using Godot;
using RecipeGame.Helpers;
using RecipeGame.Inventory;
using RecipeGame.Models;
using System;
using System.Linq;
using static RecipeGame.Helpers.Enums;
using static RecipeGame.Helpers.InventoryExtensions;

public class OutdoorSceneItemControl : Node
{
    [Signal]
    public delegate void OnLeavePanel();
    [Signal]
    public delegate void OnPurchasedRecipe();

    [Export]
    public NodePath satchelPath;
    [Export]
    public NodePath foragePath;
    [Export]
    public NodePath cursorIconPath;
    [Export]
    public NodePath marketPanelPath;
    [Export]
    public NodePath victoryPanelPath;

    private InventoryGridControl satchel;
    
    private ForagePanelControl foragePanel;

    private CursorItemIcon cursorIcon;

    private ForagingService forageService;

    private InventoryService inventoryService;

    private PlayerData playerData;

    private BiomeType biome;

    private MarketPanelControl marketPanel;
    private VictoryPanelControl victoryPanel;

    public override void _Ready()
    {
        forageService = new ForagingService();
        inventoryService = new InventoryService();

        foragePanel = GetNode<ForagePanelControl>(foragePath) ?? throw new NullReferenceException();
        satchel = GetNode<InventoryGridControl>(satchelPath) ?? throw new NullReferenceException();
        cursorIcon = GetNode<CursorItemIcon>(cursorIconPath) ?? throw new NullReferenceException();
        marketPanel = GetNode<MarketPanelControl>(marketPanelPath) ?? throw new NullReferenceException();
        victoryPanel = this.MustGetNode<VictoryPanelControl>(victoryPanelPath);

        foragePanel.InventoryGrid.Connect(nameof(InventoryGridControl.OnItemLeftPress), this, nameof(HandleInventoryItemClick), new Godot.Collections.Array{"forage", true});
        foragePanel.InventoryGrid.Connect(nameof(InventoryGridControl.OnItemRightPress), this, nameof(HandleInventoryItemClick), new Godot.Collections.Array{"forage", false});

        satchel.Connect(nameof(InventoryGridControl.OnItemLeftPress), this, nameof(HandleInventoryItemClick), new Godot.Collections.Array{"satchel", true});
        satchel.Connect(nameof(InventoryGridControl.OnItemRightPress), this, nameof(HandleInventoryItemClick), new Godot.Collections.Array{"satchel", false});

        marketPanel.MarketSellGrid.Connect(nameof(InventoryGridControl.OnItemLeftPress), this, nameof(HandleInventoryItemClick), new Godot.Collections.Array{"market", true});
        marketPanel.MarketSellGrid.Connect(nameof(InventoryGridControl.OnItemRightPress), this, nameof(HandleInventoryItemClick), new Godot.Collections.Array{"market", false});

        foragePanel.LeaveButton.Connect("pressed", this, nameof(HandleForageLeaveClick));
        foragePanel.InventoryGrid.SortButton.Connect("pressed", this, nameof(HandleForageSort));
        foragePanel.ForageButton.Connect("pressed", this, nameof(HandleForageButtonClick));

        satchel.SortButton.Connect("pressed", this, nameof(HandleSatchelSort));

        marketPanel.Connect(nameof(MarketPanelControl.OnBoughtRecipe), this, nameof(HandleMarketBoughtRecipe));
        marketPanel.LeaveButton.Connect("pressed", this, nameof(HandleMarketLeaveClick));
        marketPanel.SellButton.Connect("pressed", this, nameof(HandleMarketSellClick));

        victoryPanel.CloseButton.Connect("pressed", this, nameof(HandleVictoryLeaveClick));
    }

    public void Init(PlayerData playerData)
    {
        this.playerData = playerData;
    }

    public void ShowVictoryPanel()
    {

    }

    public void ShowMarketPanel()
    {
        RefreshMarketVisuals();
        RefreshSatchelAndCursorVisuals();

        satchel.Visible = true;
        cursorIcon.Visible = true;
        marketPanel.Visible = true;
    }

    override public void  _Process(float delta)
    {
        if(Input.IsActionJustPressed("close_panel"))
        {
            if(marketPanel.Visible)
            {
                HandleMarketLeaveClick();
            }
            else if(foragePanel.Visible)
            {
                HandleForageLeaveClick();
            }
        }

        if(foragePanel.Visible && Input.IsActionJustPressed("alternate_action"))
        {
            HandleForageButtonClick();
        }
    }

    public void ShowForageBiome(BiomeType biome)
    {
        playerData.ForageStorage.Items = new InventoryItem[playerData.ForageStorage.Items.Length];

        this.biome = biome;
        foragePanel.InitBiome(biome);
        foragePanel.Visible = true;
        foragePanel.InventoryGrid.SetItems(playerData.ForageStorage.Items);

        RefreshSatchelAndCursorVisuals();
        satchel.Visible = true;
        cursorIcon.Visible = true;
    }

    private void RefreshSatchelAndCursorVisuals()
    {
        satchel.SetItems(playerData.Inventory.Items);
        cursorIcon.SetHeldItem(playerData.HeldItem, playerData.HeldTool);
    }

    void HandleInventoryItemClick(int index, string panelName, bool leftClick)
    {
        IInventory sourceInventory;
        IInventory destInventory;
        switch(panelName)
        {
            case "forage":
                sourceInventory = playerData.ForageStorage;
                destInventory = playerData.Inventory;
                break;
            case "satchel":
                sourceInventory = playerData.Inventory;
                destInventory = foragePanel.Visible ? playerData.ForageStorage : marketPanel.Visible ? playerData.MarketSell : null;
                break;
            case "market":
                sourceInventory = playerData.MarketSell;
                destInventory = playerData.Inventory;
                break;
            default:
                throw new NotSupportedException();
        }

        var stacksChanged = false;
        if(Input.IsKeyPressed((int)KeyList.Control))
        {
            if(destInventory == null)
            {
                return;
            }

            stacksChanged = inventoryService.MoveStack(sourceInventory, destInventory, index);
        } 
        else
        {
            stacksChanged = inventoryService.ClickedOnInventoryItem(sourceInventory, playerData, index, leftClick);
        }

        if(stacksChanged)
        {
            if(foragePanel.Visible)
            {
                foragePanel.InventoryGrid.SetItems(playerData.ForageStorage.Items);
            }
            if(marketPanel.Visible)
            {
                RefreshMarketVisuals();
            }
            RefreshSatchelAndCursorVisuals();
        }
    }

    void HandleMarketLeaveClick()
    {
        marketPanel.Visible = false;
        satchel.Visible = false;
        cursorIcon.Visible = false;
        inventoryService.EmptyPlayerHand(playerData);
        cursorIcon.SetHeldItem(playerData.HeldItem, playerData.HeldTool);

        EmitSignal(nameof(OnLeavePanel));
    }

    void HandleVictoryLeaveClick()
    {
        victoryPanel.Visible = false;
        EmitSignal(nameof(OnLeavePanel));
    }

    void HandleMarketBoughtRecipe(ItemType itemType)
    {
        if(inventoryService.BuyRecipe(playerData, itemType))
        {
            RefreshMarketVisuals();
            EmitSignal(nameof(OnPurchasedRecipe));
        }
    }

    void HandleMarketSellClick()
    {
        inventoryService.SellMarketItems(playerData);
        RefreshMarketVisuals();
    }

    private void RefreshMarketVisuals()
    {
        marketPanel.SetSellGridItems(playerData.MarketSell.Items, inventoryService.GetSellMarketValue(playerData));
        marketPanel.SetRecipeRows(playerData);
        marketPanel.SetCoinBalance(playerData.CoinBalance);
    }

    void HandleForageLeaveClick()
    {
        foragePanel.Visible = false;
        satchel.Visible = false;
        cursorIcon.Visible = false;
        inventoryService.EmptyPlayerHand(playerData);
        cursorIcon.SetHeldItem(playerData.HeldItem, playerData.HeldTool);

        EmitSignal(nameof(OnLeavePanel));
    }

    void HandleForageButtonClick()
    {
        playerData.ForageStorage.Items = forageService.ForageForItems(biome, 8, playerData.Inventory).ToArray();

        foragePanel.InventoryGrid.SetItems(playerData.ForageStorage.Items);
    }

    void HandleForageSort()
    {
        playerData.ForageStorage.Items = playerData.ForageStorage.Items.AccumulateItemStacks(72);
        foragePanel.InventoryGrid.SetItems(playerData.ForageStorage.Items);
    }

    void HandleSatchelSort()
    {
        playerData.Inventory.Items = playerData.Inventory.Items.AccumulateItemStacks(32);
        satchel.SetItems(playerData.Inventory.Items);
    }
}
