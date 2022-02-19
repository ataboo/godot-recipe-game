using Godot;
using RecipeGame.Inventory;
using RecipeGame.Models;
using System;
using System.Linq;
using static RecipeGame.Helpers.Enums;
using static RecipeGame.Helpers.InventoryExtensions;

public class OutdoorSceneItemControl : Node
{
    [Signal]
    public delegate void OnLeaveForage();

    [Export]
    public NodePath satchelPath;
    [Export]
    public NodePath foragePath;
    [Export]
    public NodePath cursorIconPath;

    private InventoryGridControl satchel;
    
    private ForagePanelControl foragePanel;

    private CursorItemIcon cursorIcon;

    private ForagingService forageService;

    private InventoryService inventoryService;

    private PlayerData playerData;

    private BiomeType biome;

    public override void _Ready()
    {
        forageService = new ForagingService();
        inventoryService = new InventoryService();

        foragePanel = GetNode<ForagePanelControl>(foragePath) ?? throw new NullReferenceException();
        satchel = GetNode<InventoryGridControl>(satchelPath) ?? throw new NullReferenceException();
        cursorIcon = GetNode<CursorItemIcon>(cursorIconPath) ?? throw new NullReferenceException();

        foragePanel.InventoryGrid.Connect(nameof(InventoryGridControl.OnItemLeftPress), this, nameof(HandleInventoryItemClick), new Godot.Collections.Array{"forage", true});
        foragePanel.InventoryGrid.Connect(nameof(InventoryGridControl.OnItemRightPress), this, nameof(HandleInventoryItemClick), new Godot.Collections.Array{"forage", false});

        satchel.Connect(nameof(InventoryGridControl.OnItemLeftPress), this, nameof(HandleInventoryItemClick), new Godot.Collections.Array{"satchel", true});
        satchel.Connect(nameof(InventoryGridControl.OnItemRightPress), this, nameof(HandleInventoryItemClick), new Godot.Collections.Array{"satchel", false});

        foragePanel.LeaveButton.Connect("pressed", this, nameof(HandleForageLeaveClick));
        foragePanel.InventoryGrid.SortButton.Connect("pressed", this, nameof(HandleForageSort));
        foragePanel.ForageButton.Connect("pressed", this, nameof(HandleForageButtonClick));

        satchel.SortButton.Connect("pressed", this, nameof(HandleSatchelSort));
    }

    public void Init(PlayerData playerData)
    {
        this.playerData = playerData;
    }

    public void ShowForageBiome(BiomeType biome)
    {
        playerData.ForageStorage.Items = new InventoryItem[playerData.ForageStorage.Items.Length];

        this.biome = biome;
        foragePanel.InitBiome(biome);
        foragePanel.Visible = true;
        foragePanel.InventoryGrid.SetItems(playerData.ForageStorage.Items);

        satchel.SetItems(playerData.Inventory.Items);
        satchel.Visible = true;

        cursorIcon.SetHeldItem(playerData.HeldItem, playerData.HeldTool);
        cursorIcon.Visible = true;
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
                destInventory = playerData.ForageStorage;
                break;
            default:
                throw new NotSupportedException();
        }

        var stacksChanged = false;
        if(Input.IsKeyPressed((int)KeyList.Control))
        {
            stacksChanged = inventoryService.MoveStack(sourceInventory, destInventory, index);
        } 
        else
        {
            stacksChanged = inventoryService.ClickedOnInventoryItem(sourceInventory, playerData, index, leftClick);
        }

        if(stacksChanged)
        {
            foragePanel.InventoryGrid.SetItems(playerData.ForageStorage.Items);
            satchel.SetItems(playerData.Inventory.Items);
            cursorIcon.SetHeldItem(playerData.HeldItem, playerData.HeldTool);
        }
    }

    void HandleForageLeaveClick()
    {
        foragePanel.Visible = false;
        satchel.Visible = false;
        cursorIcon.Visible = false;
        inventoryService.EmptyPlayerHand(playerData);
        cursorIcon.SetHeldItem(playerData.HeldItem, playerData.HeldTool);

        EmitSignal(nameof(OnLeaveForage));
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
