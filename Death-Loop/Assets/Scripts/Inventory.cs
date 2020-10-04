using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Slot
{
    public bool isEmpty = true;
    public int slot;
    public Text slotText;
    public Image slotImage;
    public Part part;
    public Vector2 position;

    public Slot(bool iE, int s, Text t, Image i, Part p, Vector2 pos)
    {
        isEmpty = iE;
        slot = s;
        slotText = t;
        slotImage = i;
        part = p;
        position = pos;
    }
}

public class Inventory : MonoBehaviour
{
    public Button buttonCraft;
    public Text textPart;
    public GameObject phaserObject;
    public GameObject slotPrefab;
    public List<Slot> slots;
    public List<Slot> craftingSlots;
    public Part partHolding;
    bool hasPart;

    Canvas canvasInv;
    bool isOpen;
    Item itemToCraft;

    void Start()
    {
        //phaserObject.SetActive(false);

        canvasInv = GameObject.Find("UI_Crafting").GetComponent<Canvas>();
        canvasInv.enabled = false;

        //Inventory
        GenerateSlots(3, 2, -90, 40, GameObject.Find("Inventory").transform, true);

        //Crafting Grid
        GenerateSlots(3, 3, -75, 75, GameObject.Find("Crafting").transform, false);

        phaserObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            OnInventory();
        }

        //Cast a constant raycast to check if there is an part we can pick up
        Transform cam = Camera.main.transform;
        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, 2f))
        {
            if (hit.transform != null)
            {
                if (ItemDatabase.instance.GetPart(hit.transform.name).name != "Blank")
                {
                    textPart.text = hit.transform.name;
                    if (Input.GetButtonDown("Interact"))
                    {
                        PickupItem(hit.transform.name, hit.transform.gameObject);
                    }
                }
                else
                {
                    textPart.text = "";
                }
            }
        }
        else
        {
            textPart.text = "";
        }
    }

    void SelectSlot(int slot, bool isInv)
    {
        //Check if we are holding an item, if we are then check if the slot we clicked is empty
        if (hasPart)
        {
            if (isInv)
            {
                if (slots[slot].isEmpty)
                {
                    slots[slot].part = ItemDatabase.instance.GetPart(partHolding.name);
                    slots[slot].slotText.text = partHolding.name;
                    slots[slot].isEmpty = false;

                    partHolding = null;
                    hasPart = false;
                }
            }
            else
            {
                if (craftingSlots[slot].isEmpty)
                {
                    craftingSlots[slot].part = ItemDatabase.instance.GetPart(partHolding.name);
                    craftingSlots[slot].slotText.text = partHolding.name;
                    craftingSlots[slot].isEmpty = false;

                    partHolding = null;
                    hasPart = false;
                }
            }
        }
        else
        {
            //If we aren't holding anything and the slot we click is not empty, then that part becomes the part holding
            if (isInv)
            {
                if (!slots[slot].isEmpty)
                {
                    partHolding = slots[slot].part;
                    EmptySlot(slot, isInv);
                    hasPart = true;
                }
            }
            else
            {
                if (!craftingSlots[slot].isEmpty)
                {
                    partHolding = craftingSlots[slot].part;
                    EmptySlot(slot, isInv);
                    hasPart = true;
                }
            }
        }
        CheckCraftable();
    }

    void EmptySlot(int slot, bool isInv)
    {
        if (isInv)
        {
            slots[slot].slotText.text = "";
            slots[slot].slotImage.enabled = false;
            slots[slot].part = new Part("Blank", null, null);
            slots[slot].isEmpty = true;
        }
        else
        {
            craftingSlots[slot].slotText.text = "";
            craftingSlots[slot].slotImage.enabled = false;
            craftingSlots[slot].part = new Part("Blank", null, null);
            craftingSlots[slot].isEmpty = true;
        }
    }

    void OnInventory()
    {
        if (isOpen)
        {
            canvasInv.enabled = false;
            isOpen = false;
        }
        else
        {
            canvasInv.enabled = true;
            isOpen = true;
        }

        GameManager.instance.CallPause(isOpen, false);
    }

    void GenerateSlots(int xSlots, int ySlots, int realX, int realY, Transform parent, bool isInv)
    {
        int slot = 0;
        int startX = realX, startY = realY;

        for (int y = 0; y < ySlots; y++)
        {
            for (int x = 0; x < xSlots; x++)
            {
                GameObject newSlot = Instantiate(slotPrefab, parent);
                newSlot.transform.localPosition = new Vector2(realX, realY);

                Text text = newSlot.GetComponentInChildren<Text>();
                text.text = "";
                Image img = newSlot.transform.GetChild(0).GetComponent<Image>();
                img.enabled = false;

                Button btn = newSlot.GetComponent<Button>();
                int s = slot;
                bool iInv = isInv;
                btn.onClick.AddListener(() => SelectSlot(s, iInv));

                if (isInv)
                {
                    slots.Add(new Slot(true, slot, text, img, new Part("Blank", null, null), new Vector2(x, y)));
                }
                else
                {
                    craftingSlots.Add(new Slot(true, slot, text, img, new Part("Blank", null, null), new Vector2(x, y)));
                }
                
                slot++;

                realX += (startX * -1);
            }
            if (ySlots < 3)
                realY -= startY * 2;
            else
                realY -= startY;

            realX = startX;
        }
    }

    void PickupItem(string name, GameObject theObject)
    {
        //Loop through the slots to find the first empty one
        for (int i = 0; i < slots.Count; i++)
        {
            //If the part is null, then the slot is empty
            if (slots[i].part.name == "Blank")
            {
                print("Finding part");
                slots[i].part = ItemDatabase.instance.GetPart(name);
                slots[i].slotText.text = slots[i].part.name;
                slots[i].isEmpty = false;
                if(theObject != null)
                    Destroy(theObject);
                break;
            }
        }
    }

    void CheckCraftable()
    {
        bool flag = true;

        Item[] itemList = ItemDatabase.instance.itemList;

        print("Checking Recipe...");

        //Check through every craftable item in the database
        for (int i = 0; i < itemList.Length; i++)
        {
            flag = true;

            //Go through each slot on the crafting grid
            //We need to go through each crafting slot, and confirm that the part in that slot, is the part that is needed for a recipe
            for (int c = 0; c < craftingSlots.Count; c++)
            {
                //Compare the part in the recipe to the part on the current crafting slot
                //If the two parts are the same, then we are on the right track
                //If one of the parts are not the same, then this cannot be the recipe
                if (itemList[i].recipe.parts[c].name != craftingSlots[c].part.name)
                {
                    //Break out of this loop, it's not this recipe
                    flag = false;
                    buttonCraft.interactable = false;
                    break;
                }
            }

            if (flag)
            {
                //We have the recipe
                print("Item Craftable: " + ItemDatabase.instance.itemList[i].name);
                itemToCraft = ItemDatabase.instance.itemList[i];
                buttonCraft.interactable = true;
                break;
            }
        }
    }

    void ClearCraftingGrid()
    {
        for (int i = 0; i < craftingSlots.Count; i++)
        {
            craftingSlots[i].slotText.text = "";
            craftingSlots[i].slotImage.enabled = false;
            craftingSlots[i].part = new Part("Blank", null, null);
            craftingSlots[i].isEmpty = true;
        }
    }

    public void CraftItem()
    {
        if (itemToCraft != null)
        {
            ClearCraftingGrid();
            TaskSystem.instance.CompleteTask();
            buttonCraft.interactable = false;

            if (itemToCraft.name == "Phaser")
            {
                PhaserController.instance.textCharge.text = "Phaser: " + 100 + "%";
                phaserObject.SetActive(true);
            }
        }
    }
}
