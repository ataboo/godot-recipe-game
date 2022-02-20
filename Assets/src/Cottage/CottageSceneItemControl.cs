using Godot;
using RecipeGame.Helpers;
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
    [Export]
    public NodePath prepBenchPanelPath;
    [Export]
    public NodePath audioLibraryPlayerPath;
    [Export]
    public NodePath cauldronNoisesPath;
    [Export]
    public NodePath decisionAchievementPath;

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
    private PrepTablePanel prepBenchPanel;
    private AudioLibraryPlayer audioPlayer;
    private CauldronNoisesControl cauldronNoises;
    private TextureRect decisionAchievement;

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
        prepBenchPanel = GetNode<PrepTablePanel>(prepBenchPanelPath) ?? throw new NullReferenceException();
        audioPlayer = this.MustGetNode<AudioLibraryPlayer>(audioLibraryPlayerPath);
        cauldronNoises = this.MustGetNode<CauldronNoisesControl>(cauldronNoisesPath);
        decisionAchievement = this.MustGetNode<TextureRect>(decisionAchievementPath);

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

        prepBenchPanel.LeaveButton.Connect("pressed", this, nameof(HandlePrepBenchLeaveClick));
        prepBenchPanel.ProcessButton.Connect("pressed", this, nameof(HandlePrepBenchProcess));
        prepBenchPanel.TakeAllButton.Connect("pressed", this, nameof(HandlePrepBenchTakeAll));

        prepBenchPanel.InputSpot.IconControl.Connect(
            nameof(InventoryIconControl.OnLeftPress), 
            this, 
            nameof(HandleInventoryItemClick), 
            new Godot.Collections.Array{0, "prep_bench_input", true}
        );
        prepBenchPanel.InputSpot.IconControl.Connect(
            nameof(InventoryIconControl.OnRightPress), 
            this, 
            nameof(HandleInventoryItemClick), 
            new Godot.Collections.Array{0, "prep_bench_input", false}
        );
        prepBenchPanel.OutputSpot.IconControl.Connect(
            nameof(InventoryIconControl.OnLeftPress), 
            this, 
            nameof(HandleInventoryItemClick), 
            new Godot.Collections.Array{0, "prep_bench_output", true}
        );
        prepBenchPanel.OutputSpot.IconControl.Connect(
            nameof(InventoryIconControl.OnRightPress), 
            this, 
            nameof(HandleInventoryItemClick), 
            new Godot.Collections.Array{0, "prep_bench_output", false}
        );
    }

    private async void ShowDecisionAchievement()
    {
        audioPlayer.PlaySound("decisionsound");
        decisionAchievement.Visible = true;
        await ToSignal(GetTree().CreateTimer(3), "timeout");

        decisionAchievement.Visible = false;
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
                audioPlayer.PlaySound("potionbloop");
                cauldronInventory.UpdateOutputItems(playerData.Cauldron);
                break;
            case CauldronState.JustStartedRecipe:
            case CauldronState.RecipeInProgress:
            case CauldronState.RecipeInterrupted:
            case CauldronState.WaitingForProductRemoval:
            case CauldronState.WaitingToCook:
                break;
        }

        cauldronNoises.UpdateCauldron(playerData.Cauldron);

        if(cauldronPanel.Visible)
        {
            cauldronInventory.UpdateVisuals(playerData.Cauldron, delta);
        }

        if(Input.IsActionJustPressed("close_panel"))
        {
            if(storagePanel.Visible)
            {
                HandleStorageLeaveClick();
            }
            else if(cauldronPanel.Visible)
            {
                HandleCauldronLeaveButtonClick();
            }
            else if(prepBenchPanel.Visible)
            {
                HandlePrepBenchLeaveClick();
            }
        }

        if(prepBenchPanel.Visible && Input.IsActionJustPressed("alternate_action"))
        {
            HandlePrepBenchProcess();
        }

        if(prepBenchPanel.Visible && Input.IsActionJustPressed("take_all"))
        {
            HandlePrepBenchTakeAll();
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

    public void ShowPrepBench()
    {
        prepBenchPanel.SetItems(playerData.PrepBench.InputItem, playerData.PrepBench.OutputItem, inventoryService.PrepBenchCanProcess(playerData.PrepBench));
        satchel.SetItems(playerData.Inventory.Items);
        cursorIcon.SetHeldItem(playerData.HeldItem, playerData.HeldTool);

        prepBenchPanel.Visible = true;
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
                else if(prepBenchPanel.Visible)
                {
                    destInventory = playerData.PrepBench.InputItems;
                }
                break;
            case "prep_bench_input":
                sourceInventory = playerData.PrepBench.InputItems;
                destInventory = playerData.Inventory;
                break;
            case "prep_bench_output":
                sourceInventory = playerData.PrepBench.OutputItems;
                destInventory = playerData.Inventory;
                break;
            default:
                throw new NotSupportedException();
        }

        int moveAmount = 0;
        if(Input.IsKeyPressed((int)KeyList.Control))
        {
            if(destInventory == null)
            {
                return;
            }

            moveAmount = inventoryService.MoveStack(sourceInventory, destInventory, index);
        }
        else 
        {
            moveAmount = inventoryService.ClickedOnInventoryItem(sourceInventory, playerData, index, leftClick);
        }

        if(moveAmount != 0)
        {
            audioPlayer.PlaySound(moveAmount > 0 ? "jugpickup" : "jugdrop");
            if(storagePanel.Visible)
            {
                storagePanel.InventoryGrid.SetItems(playerData.Storage.Items);
            }
            if(satchel.Visible)
            {
                satchel.SetItems(playerData.Inventory.Items);   
            }
            if(prepBenchPanel.Visible)
            {
                prepBenchPanel.SetItems(playerData.PrepBench.InputItem, playerData.PrepBench.OutputItem, inventoryService.PrepBenchCanProcess(playerData.PrepBench));
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

        audioPlayer.PlaySound(playerData.HeldTool == HeldTool.Empty ? "jugpickup" : "jugdrop");
        playerData.HeldTool = playerData.HeldTool == HeldTool.Empty ? tool : HeldTool.Empty;
        toolTable.SetHeldTool(playerData.HeldTool);
        cursorIcon.SetHeldItem(playerData.HeldItem, playerData.HeldTool);
    }

    void HandleClickCauldron(bool leftClick)
    {
        var movedItems = false;
        if(cauldronService.ClickedOnFire(playerData))
        {
            movedItems = true;
            audioPlayer.PlaySound("firewhoosh");
        }  
        else if (cauldronService.ClickedOnCauldron(playerData, leftClick))
        {
            movedItems = true;
            audioPlayer.PlaySound("cauldronplop");
        }

        if(movedItems)
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
            
            audioPlayer.PlaySound("jugpickup");
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

        
        audioPlayer.PlaySound("click1");
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
        
        audioPlayer.PlaySound("click1");
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

    void HandlePrepBenchLeaveClick()
    {
        prepBenchPanel.Visible = false;
        satchel.Visible = false;
        inventoryService.EmptyPlayerHand(playerData);
        cursorIcon.SetHeldItem(playerData.HeldItem, playerData.HeldTool);

        EmitSignal(nameof(OnLeaveUIPanels));
    }

    void HandlePrepBenchProcess()
    {
        if(inventoryService.ProcessPrepBench(playerData.PrepBench))
        {
            if(playerData.PrepBench.OutputItem?.Stats.ItemType == ItemType.AmphibianCharm || playerData.PrepBench.OutputItem?.Stats.ItemType == ItemType.GlowCharm)
            {
                ShowDecisionAchievement();
            }
            prepBenchPanel.SetItems(playerData.PrepBench.InputItem, playerData.PrepBench.OutputItem, inventoryService.PrepBenchCanProcess(playerData.PrepBench));
            audioPlayer.PlaySound("chop1");
        }
        else
        {
            audioPlayer.PlaySound("fingersaw");
        }
    }

    void HandlePrepBenchTakeAll()
    {
        inventoryService.TakeAllItems(playerData.PrepBench.OutputItems, playerData.Inventory);
        satchel.SetItems(playerData.Inventory.Items);
        prepBenchPanel.SetItems(playerData.PrepBench.InputItem, playerData.PrepBench.OutputItem, inventoryService.PrepBenchCanProcess(playerData.PrepBench));
        audioPlayer.PlaySound("jugpickup");
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
