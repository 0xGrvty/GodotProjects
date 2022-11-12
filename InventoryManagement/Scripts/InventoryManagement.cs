using Godot;
using Godot.Collections;
using System;

public class InventoryManagement : Control {
    private Vector2 tileSize = new Vector2(32, 32);
    private Vector2 inventoryDimensions = new Vector2(8, 8); // 256 / 32 = 8

    private ColorRect inventoryPanel;
    private Sprite inventoryGrids;
    private Area2D inventory;

    private bool isItemSelected = false;
    private Control selectedItem;
    private Vector2 itemPrevPosition;

    private bool isDraggingItem = false;

    private int selectedItemZIndex = 1000;

    private Godot.Collections.Array overlappingItems = new Godot.Collections.Array();

    private Color invalidColor = new Color(1f, 0.36f, 0.36f, 1f);
    private Color validColor = new Color(1f, 1f, 1f, 1f);

    private bool isSelectedItemInsideInventory = false;

    private Vector2 cursorItemDragOffset = new Vector2(-8, -8);

    private Dictionary inventoryItems = new Dictionary();
    private Dictionary inventoryItemSlots = new Dictionary();

    public override void _Ready() {
        inventoryPanel = GetNode<ColorRect>("InventoryPanel");
        inventoryGrids = GetNode<Sprite>("InventoryGrid");
        inventory = GetNode<Area2D>("InventoryGrid/Inventory");

        // Connect inventory signal connections
        inventory.Connect("area_entered", this, nameof(ItemInsideInventory));
        inventory.Connect("area_exited", this, nameof(ItemGoesOutsideInventory));

        // Add Signal connections to item prefabs

        foreach (Control item in GetTree().GetNodesInGroup("Item")) {
            AddSignalConnections(item);
        }
    }

    // Connect the signals of each item to a function in this script
    private void AddSignalConnections(Control item) {
        item.Connect("gui_input", this, "CursorInItem", new Godot.Collections.Array(item));
        item.GetNode<Area2D>("Sprite/Area2D").Connect("area_entered", this, nameof(OverlappingWithOtherItem), new Godot.Collections.Array(item));
        item.GetNode<Area2D>("Sprite/Area2D").Connect("area_exited", this, nameof(NotOverlappingWithOtherItem), new Godot.Collections.Array(item));


    }

    // Fires off the moment our cursor is in an item
    private void CursorInItem(InputEvent e, Control item) {

        if (e.IsActionPressed("select_item")) {
            isItemSelected = true;
            selectedItem = item;
            itemPrevPosition = selectedItem.RectPosition;
            selectedItem.GetNode<Sprite>("Sprite").ZIndex = selectedItemZIndex;
        }

        if (e is InputEventMouseMotion) {
            if (isItemSelected) {
                isDraggingItem = true;
            }
        }

        if (e.IsActionReleased("select_item")) {
            selectedItem.GetNode<Sprite>("Sprite").ZIndex = 0;

            if (overlappingItems.Count > 0) {
                selectedItem.RectPosition = itemPrevPosition;
                selectedItem.GetNode<Sprite>("Sprite").Modulate = validColor;
            } else {
                if (isSelectedItemInsideInventory) {
                    if (!AddItemToInventory(selectedItem)) {
                        selectedItem.RectPosition = itemPrevPosition;
                    }
                }
            }

            isItemSelected = false;
            isDraggingItem = false;
            selectedItem = null;
        }
    }

    // Fires off the moment when we are overlapping with other items while an item is selected.  Inclusive with selected item.
    private void OverlappingWithOtherItem(Area2D area, Control item) {
        if (area.GetParent().GetParent() == selectedItem) {
            return;
        }

        if (area == inventory) {
            return;
        }
        overlappingItems.Add(item);

        if (item == selectedItem) {
            selectedItem.GetNode<Sprite>("Sprite").Modulate = invalidColor;
        }
    }

    // Fires off the moment when we are not overlapping other items while an item is selected.  Inclusive with selected item.
    private void NotOverlappingWithOtherItem(Area2D area, Control item) {
        if (area.GetParent().GetParent() == selectedItem) {
            return;
        }

        if (area == inventory) {
            return;
        }
        overlappingItems.Remove(item);

        if (overlappingItems.Count == 0 && isItemSelected) {
            selectedItem.GetNode<Sprite>("Sprite").Modulate = validColor;
        }
    }

    public override void _Process(float delta) {
        if (isDraggingItem) {
            selectedItem.RectPosition = (this.GetGlobalMousePosition() + cursorItemDragOffset);
        }
    }

    // area_entered and area_exited take in an Area2D as an argument.  Need it when we connect
    // this signal even though we don't do anything with it.
    private void ItemInsideInventory(Area2D a) {
        isSelectedItemInsideInventory = true;
    }

    private void ItemGoesOutsideInventory(Area2D a) {
        isSelectedItemInsideInventory = false;
    }

    private bool AddItemToInventory(Control item) {
        var slotID = new Vector2(Mathf.Floor(item.RectPosition.x / tileSize.x), Mathf.Floor(item.RectPosition.y / tileSize.y));
        var itemSlotSize = new Vector2(item.RectSize / tileSize);
        GD.Print("slotID: " + slotID + "item.RectPosition: " + item.RectPosition);

        var itemMaxSlotID = new Vector2(slotID + itemSlotSize - new Vector2(1, 1));

        var inventorySlotBounds = new Vector2(inventoryDimensions - new Vector2(1, 1));

        if (itemMaxSlotID.x > inventorySlotBounds.x) {
            return false;
        }
        if (itemMaxSlotID.y > inventorySlotBounds.y) {
            return false;
        }
        item.RectPosition = new Vector2(slotID * tileSize);
        if (inventoryItems.Contains(item)) {
            RemoveItemInInventorySlot(item, (Vector2)inventoryItems[item]);
        }

        for (int i = 0; i < itemSlotSize.y; i++) {
            for (int j = 0; j < itemSlotSize.x; j++) {
                inventoryItemSlots[new Vector2(slotID.x + j, slotID.y + i)] = item;
            }
        }

        // upper left-most tile ID of the item
        inventoryItems[item] = slotID;

        return true;
    }

    private void RemoveItemInInventorySlot(Control item, Vector2 existingSlotID) {
        var itemSlotSize = new Vector2(item.RectSize / tileSize);
        for (int i = 0; i < itemSlotSize.y; i++) {
            for (int j = 0; j < itemSlotSize.x; j++) {
                if (inventoryItemSlots.Contains(new Vector2(existingSlotID.x + j, existingSlotID.y + i))) {
                    inventoryItemSlots.Remove(new Vector2(existingSlotID.x + j, existingSlotID.y + i));
                }
            }
        }
    }
}



