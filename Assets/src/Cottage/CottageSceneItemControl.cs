using Godot;
using RecipeGame.Inventory;
using RecipeGame.Models;
using System;
using static RecipeGame.Helpers.Enums;
using static RecipeGame.Helpers.InventoryExtensions;

public class CottageSceneItemControl : Node
{
    [Signal]
    public delegate void OnLeaveUIPanels();

    [Export]
    public NodePath satchelPath;
    [Export]
    public NodePath storagePath;
    [Export]
    public NodePath cursorIconPath;
    [Export]
    public NodePath cauldronPath;
    [Export]
    public NodePath toolTablePath;

    private InventoryGridControl satchel;
    private StoragePanelControl storagePanel;
    private CursorItemIcon cursorIcon;
    private InventoryService inventoryService;
    private PlayerData playerData;
    private ToolTableControl toolTable;
    private CauldronVisualsControl cauldronVisuals;

    public override void _Ready()
    {
        inventoryService = new InventoryService();

        storagePanel = GetNode<StoragePanelControl>(storagePath) ?? throw new NullReferenceException();
        satchel = GetNode<InventoryGridControl>(satchelPath) ?? throw new NullReferenceException();
        cursorIcon = GetNode<CursorItemIcon>(cursorIconPath) ?? throw new NullReferenceException();
        toolTable = GetNode<ToolTableControl>(toolTablePath) ?? throw new NullReferenceException();
        cauldronVisuals = GetNode<CauldronVisualsControl>(cauldronPath) ?? throw new NullReferenceException();

        storagePanel.InventoryGrid.Connect(nameof(InventoryGridControl.OnItemLeftPress), this, nameof(HandleInventoryItemClick), new Godot.Collections.Array{nameof(StorageInventory), true});
        storagePanel.InventoryGrid.Connect(nameof(InventoryGridControl.OnItemRightPress), this, nameof(HandleInventoryItemClick), new Godot.Collections.Array{nameof(StorageInventory), false});

        satchel.Connect(nameof(InventoryGridControl.OnItemLeftPress), this, nameof(HandleInventoryItemClick), new Godot.Collections.Array{nameof(PlayerInventory), true});
        satchel.Connect(nameof(InventoryGridControl.OnItemRightPress), this, nameof(HandleInventoryItemClick), new Godot.Collections.Array{nameof(PlayerInventory), false});

        storagePanel.LeaveButton.Connect("pressed", this, nameof(HandleStorageLeaveClick));
        storagePanel.InventoryGrid.SortButton.Connect("pressed", this, nameof(HandleStorageSort));
        satchel.SortButton.Connect("pressed", this, nameof(HandleSatchelSort));

        toolTable.Connect(nameof(ToolTableControl.OnToolClicked), this, nameof(HandleToolTableClick));
    }

    public override void _Process(float delta)
    {
        // if(PlayerData?.Cauldron == null) {
        //     GD.PushError("No cauldron to update!");
        //     return;
        // }

        // cauldronService.Update(PlayerData.Cauldron, delta);
    }

    public void Init(PlayerData playerData)
    {
        this.playerData = playerData;
    }

    public void ShowStoragePanel()
    {
        storagePanel.InventoryGrid.SetItems(playerData.Storage.Items);
        satchel.SetItems(playerData.Inventory.Items);
        cursorIcon.SetHeldItem(playerData.HeldItem, playerData.HeldTool);

        storagePanel.Visible = true;
        satchel.Visible = true;
    }

    public void ShowCauldron()
    {
        //cauldronVisuals.SetItems(playerData.Cauldron);
        satchel.SetItems(playerData.Inventory.Items);
        toolTable.SetHeldTool(playerData.HeldTool);
        cursorIcon.SetHeldItem(playerData.HeldItem, playerData.HeldTool);

        cauldronVisuals.Visible = true;
        satchel.Visible = true;
    }

    void HandleInventoryItemClick(int index, string panelName, bool leftClick)
    {
        IInventory targetInventory;
        switch(panelName)
        {
            case nameof(StorageInventory):
                targetInventory = playerData.Storage;
                break;
            case nameof(PlayerInventory):
                targetInventory = playerData.Inventory;
                break;
            default:
                throw new NotSupportedException();
        }

        var stacksChanged = inventoryService.ClickedOnInventoryItem(targetInventory, playerData, index, leftClick);
        if(stacksChanged)
        {
            if(storagePanel.Visible)
            {
                storagePanel.InventoryGrid.SetItems(playerData.Storage.Items);
            }
            if(satchel.Visible)
            {
                satchel.SetItems(playerData.Inventory.Items);   
            }

            cursorIcon.SetHeldItem(playerData.HeldItem, playerData.HeldTool);
        }
    }

    void HandleToolTableClick(HeldTool tool, bool leftClick)
    {
        if(playerData.HeldItem != null)
        {
            return;
        }

        playerData.HeldTool = playerData.HeldTool == HeldTool.Empty ? tool : HeldTool.Empty;
        toolTable.SetHeldTool(playerData.HeldTool);
        cursorIcon.SetHeldItem(playerData.HeldItem, playerData.HeldTool);
    }

    void HandleStorageLeaveClick()
    {
        storagePanel.Visible = false;
        satchel.Visible = false;
        inventoryService.EmptyPlayerHand(playerData);
        cursorIcon.SetHeldItem(playerData.HeldItem, playerData.HeldTool);

        EmitSignal(nameof(OnLeaveUIPanels));
    }

    void HandleStorageSort()
    {
        playerData.ForageStorage.Items = playerData.ForageStorage.Items.AccumulateItemStacks(InventoryService.StorageItemSlotCount);
        storagePanel.InventoryGrid.SetItems(playerData.ForageStorage.Items);
    }

    void HandleSatchelSort()
    {
        playerData.Inventory.Items = playerData.Inventory.Items.AccumulateItemStacks(InventoryService.PlayerItemSlotCount);
        satchel.SetItems(playerData.Inventory.Items);
    }
}
