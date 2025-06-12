using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

[Serializable]
public class CustomColorList
{
    [SerializeField] private List<CustomColor> colors = new List<CustomColor>();

    public void AddTile(CustomColor tile)
    {
        colors.Add(tile);
    }

    public CustomColor GetTile(int id)
    {
        return colors.Find(tile => tile.id == id);
    }

    public CustomColor GetTile(Color32 color)
    {
        return colors.Find(tile => tile.color.Equals(color));
    }

    public CustomColor FirstWithoutColor()
    {
        return colors.Find(tile => tile.color.Equals(Color.clear));
    }

    public bool ContainsColor(Color32 color)
    {
        return colors.Any(tile => tile.color.Equals(color));
    }

    public int Count
    {
        get { return colors.Count; }
    }
}

[Serializable]
public class CustomColor
{
    public CustomColor(int id)
    {
        this.id = id;
        color = Color.clear;
        count = 0;
        maxCount = 0;
    }

    public int id { get; }
    public Vector3Int pos { get; set; }
    public Color32 color { get; set; }
    public Color32 grayColor { get; set; }
    public int count { get; set; }
    public int maxCount { get; set; }

    public void AddCount()
    {
        count++;
        if (count > maxCount)
        {
            maxCount = count;
        }
    }
}
