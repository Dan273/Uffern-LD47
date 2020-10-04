using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//This defines the parts you can find in the rooms, which will be used to craft the final item
[System.Serializable]
public class Part
{
    public string name;
    public GameObject gameObject;
    public Sprite icon;

    public Part(string n, GameObject g, Sprite i)
    {
        name = n;
        gameObject = g;
        icon = i;
    }
}

[System.Serializable]
public struct Recipe
{
    public Part[] parts;

    public Recipe(Part[] p)
    {
        parts = p;
    }
}

[System.Serializable]
public class Item
{
    public string name;
    public GameObject gameObject;
    public Sprite icon;
    public Recipe recipe;

    public Item(string n, GameObject g, Sprite i, Recipe r)
    {
        name = n;
        gameObject = g;
        icon = i;
        recipe = r;
    }
}

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase instance;

    public Part[] partList;
    public Item[] itemList;

    void Awake()
    {
        instance = this;

        partList = GetPartList();
        itemList = GetItemList();
    }

    Item[] GetItemList()
    {
        Item[] items = new Item[]
        {
           new Item("Axe", null, null, new Recipe(new Part[]{GetPart(null), GetPart("Axe Head"), GetPart(null), 
                                                            GetPart(null), GetPart("Tape"), GetPart(null), 
                                                            GetPart(null), GetPart("Handle"), GetPart(null) })),

           new Item("Axe", null, null, new Recipe(new Part[]{GetPart(null), GetPart(null), GetPart(null),
                                                            GetPart("Handle"), GetPart("Tape"), GetPart("Axe Head"),
                                                            GetPart(null), GetPart(null), GetPart(null) })),

           new Item("Axe", null, null, new Recipe(new Part[]{GetPart(null), GetPart(null), GetPart(null),
                                                            GetPart("Axe Head"), GetPart("Tape"), GetPart("Handle"),
                                                            GetPart(null), GetPart(null), GetPart(null) })),

           new Item("Hammer", null, null, new Recipe(new Part[]{GetPart(null), GetPart("Hammer Head"), GetPart(null),
                                                            GetPart(null), GetPart("Tape"), GetPart(null),
                                                            GetPart(null), GetPart("Handle"), GetPart(null) })),

           new Item("Hammer", null, null, new Recipe(new Part[]{GetPart(null), GetPart(null), GetPart(null),
                                                            GetPart("Handle"), GetPart("Tape"), GetPart("Hammer Head"),
                                                            GetPart(null), GetPart(null), GetPart(null) })),

           new Item("Hammer", null, null, new Recipe(new Part[]{GetPart(null), GetPart(null), GetPart(null),
                                                            GetPart("Hammer Head"), GetPart("Tape"), GetPart("Handle"),
                                                            GetPart(null), GetPart(null), GetPart(null) })),



           //Leave last
           new Item("Phaser", null, null, new Recipe(new Part[]{GetPart(null), GetPart("Phaser Part"), GetPart("Phaser Part"),
                                                            GetPart(null), GetPart("Phaser Part"), GetPart(null),
                                                            GetPart(null), GetPart("Phaser Part"), GetPart(null) })),
    };
        return items;
    }

    Part[] GetPartList()
    {
        Part[] parts = new Part[]
        {
            new Part("Handle", GetGameObject("Handle"), null),
            new Part("Tape", GetGameObject("Tape"), null),
            new Part("Axe Head", GetGameObject("Axe Head"), null),
            new Part("Plank", GetGameObject("Plank"), null),
            new Part("Hammer Head", GetGameObject("Hammer Head"), null),
            new Part("Cube", GetGameObject("Cube"), null),
            new Part("Ball", GetGameObject("Ball"), null),
            new Part("Brick", GetGameObject("Brick"), null),

            //Keep last
            new Part("Phaser Part", GetGameObject("Phaser Part"), null)
        };

        return parts;
    }

    GameObject GetGameObject(string name)
    {
        GameObject theObject = Resources.Load<GameObject>("Objects/Parts/" + name);
        return theObject;
    }

    public Part GetPart(string name)
    {
        Part thePart = null;
        for (int i = 0; i < partList.Length; i++)
        {
            if (partList[i].name == name)
            {
                thePart = partList[i];
                break;
            }
        }
        if (thePart == null)
        {
            thePart = new Part("Blank", null, null);
        }

        return thePart;
    }
}
