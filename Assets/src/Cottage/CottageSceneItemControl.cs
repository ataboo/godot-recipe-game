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
    public NodePath cauldronPanelPath;
    [Export]
    public NodePath toolTablePath;
    [Export]
    public NodePath cauldronInventoryPath;
    [Export]
    public NodePath garbageControlPath;

    private InventoryGridControl satchel;
    private StoragePanelControl storagePanel;
    private CursorItemIcon cursorIcon;
    private InventoryService inventoryService;
    private CauldronService cauldronService;
    private PlayerData playerData;
    private ToolTableControl toolTable;
    private CauldronPanelControl cauldronPanel;
    private CauldronInventoryControl cauldronInventory;
    private GarbageAreaControl garbageControl;

    public override void _Ready()
    {
        inventoryService = new InventoryService();
        cauldronService = new CauldronService();

        storagePanel = GetNode<StoragePanelControl>(storagePath) ?? throw new NullReferenceException();
        satchel = GetNode<InventoryGridControl>(satchelPath) ?? throw new NullReferenceException();
        cursorIcon = GetNode<CursorItemIcon>(cursorIconPath) ?? throw new NullReferenceException();
        toolTable = GetNode<ToolTableControl>(toolTablePath) ?? throw new NullReferenceException();
        cauldronPanel = GetNode<CauldronPanelControl>(cauldronPanelPath) ?? throw new NullReferenceException();
        cauldronInventory = GetNode<CauldronInventoryControl>(cauldronInventoryPath) ?? throw new NullReferenceException();
        garbageControl = GetNode<GarbageAreaControl>(garbageControlPath) ?? throw new NullReferenceException();

        storagePanel.InventoryGrid.Connect(nameof(InventoryGridControl.OnItemLeftPress), this, nameof(HandleInventoryItemClick), new Godot.Collections.Array{nameof(StorageInventory), true});
        storagePanel.InventoryGrid.Connect(nameof(InventoryGridControl.OnItemRightPress), this, nameof(HandleInventoryItemClick), new Godot.Collections.Array{nameof(StorageInventory), false});

        satchel.Connect(nameof(InventoryGridControl.OnItemLeftPress), this, nameof(HandleInventoryItemClick), new Godot.Collections.Array{nameof(PlayerInventory), true});
        satchel.Connect(nameof(InventoryGridControl.OnItemRightPress), this, nameof(HandleInventoryItemClick), new Godot.Collections.Array{nameof(PlayerInventory), false});

        storagePanel.LeaveButton.Connect("pressed", this, nameof(HandleStorageLeaveClick));
        storagePanel.InventoryGrid.SortButton.Connect("pressed", this, nameof(HandleStorageSort));
        satchel.SortButton.Connect("pressed", this, nameof(HandleSatchelSort));

        toolTable.Connect(nameof(ToolTableControl.OnToolClicked), this, nameof(HandleToolTableClick));

        cauldronInventory.Connect(nameof(CauldronInventoryControl.OnClickCauldron), this, nameof(HandleClickCauldron));
        cauldronInventory.Connect(nameof(CauldronInventoryControl.OnClickCauldronProduct), this, nameof(HandleClickCauldronProduct));

        cauldronPanel.LeaveButton.Connect("pressed", this, nameof(HandleCauldronLeaveButtonClick));
        
        garbageControl.Connect(nameof(GarbageAreaControl.OnClickGarbage), this, nameof(HandleClickGarbage));
        garbageControl.Connect(nameof(GarbageAreaControl.OnConfirm), this, nameof(HandleConfirmGarbage));
    }

    public override void _Process(float delta)
    {
        if(playerData?.Cauldron == null) {
            GD.PushError("No cauldron to update!");
            return;
        }

        var updateResult = cauldronService.Update(playerData.Cauldron, delta);
        switch(updateResult)
        {
            case CauldronState.JustFinishedRecipe:
                cauldronInventory.UpdateOutputItems(playerData.Cauldron);
                break;
            case CauldronState.JustStartedRecipe:
            case CauldronState.RecipeInProgress:
            case CauldronState.RecipeInterrupted:
            case CauldronState.WaitingForProductRemoval:
            case CauldronState.WaitingToCook:
                break;
        }

        if(cauldronPanel.Visible)
        {
            cauldronInventory.UpdateVisuals(playerData.Cauldron, delta);
        }
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
        cauldronInventory.Init(playerData.Cauldron);
        satchel.SetItems(playerData.Inventory.Items);
        toolTable.SetHeldTool(playerData.HeldTool);
        cursorIcon.SetHeldItem(playerData.HeldItem, playerData.HeldTool);

        cauldronPanel.Visible = true;
        satchel.Visible = true;
    }

    void HandleInventoryItemClick(int index, string panelName, bool leftClick)
    {
        IInventory sourceInventory;
        IInventory destInventory = null;
        switch(panelName)
        {
            case nameof(StorageInventory):
                sourceInventory = playerData.Storage;
                destInventory = playerData.Inventory;
                break;
            case nameof(PlayerInventory):
                sourceInventory = playerData.Inventory;
                if(storagePanel.Visible)
                {
                    destInventory = playerData.Storage;
                }
                // else if(propBenchPanel.Visible)
                // {
                //     destInventory = playerData.PrepBench;
                // }
                break;
            case nameof(PrepBench):
                sourceInventory = playerData.PrepBench;
                destInventory = playerData.Inventory;
                break;
            default:
                throw new NotSupportedException();
        }

        bool stacksChanged = false;
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
            if(storagePanel.Visible)
            {
                storagePanel.InventoryGrid.SetItems(playerData.Storage.Items);
            }
            if(satchel.Visible)
            {
                satchel.SetItems(playerData.Inventory.Items);   
            }
            // if(prepBenchPanel.Visible)
            // {
            //     prepBenchPanel.SetItems(playerData.PrepBench.Items);
            // }

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

    void HandleClickCauldron(bool leftClick)
    {
        if(cauldronService.ClickedOnFire(playerData) || cauldronService.ClickedOnCauldron(playerData, leftClick))
        {
            cursorIcon.SetHeldItem(playerData.HeldItem, playerData.HeldTool);
        }
    }

    void HandleCauldronLeaveButtonClick()
    {
        cauldronPanel.Visible = false;
        satchel.Visible = false;
        inventoryService.EmptyPlayerHand(playerData);
        cursorIcon.SetHeldItem(playerData.HeldItem, playerData.HeldTool);

        EmitSignal(nameof(OnLeaveUIPanels));
    }

    void HandleClickCauldronProduct(int index)
    {
        if(cauldronService.ClickedOnCauldronProduct(playerData, index))
        {
            cursorIcon.SetHeldItem(playerData.HeldItem, playerData.HeldTool);
            cauldronInventory.UpdateOutputItems(playerData.Cauldron);
        }
    }

    void HandleClickGarbage(bool leftClick)
    {
        if(playerData.HeldItem == null)
        {
            if(playerData.Cauldron.Ingredients.Count == 0)
            {
                return;
            }

            garbageControl.ShowConfirm(true, "Empty Cauldron?");
            return;
        }

        garbageControl.ShowConfirm(true, "Destroy Item?");
    }

    void HandleConfirmGarbage(bool yes)
    {
        if(yes)
        {
            if(playerData.HeldItem != null)
            {
                playerData.HeldItem = null;
                cursorIcon.SetHeldItem(playerData.HeldItem, playerData.HeldTool);
            } 
            else 
            {
                cauldronService.EmptyCauldron(playerData.Cauldron);
                cauldronInventory.UpdateOutputItems(playerData.Cauldron);
            }
        }
        
        garbageControl.ShowConfirm(false, "");
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
