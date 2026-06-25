using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class User
{
    public string Name { get; set; }
    public int Gold { get; set; }
    public int Diamond { get; set; }
    public Map MapInGame { get; set; }
    public List<InvenItems>  inventoryItems { get; set; }

    public User()
    {
    }

    public User(string name, int gold, int diamond, Map mapInGame, List<InvenItems> inventoryItems)
    {
        Name = name;
        Gold = gold;
        Diamond = diamond;
        MapInGame = mapInGame;
        inventoryItems = inventoryItems;
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
