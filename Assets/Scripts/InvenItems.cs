using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class InvenItems
{
    public string Name {get; set;}
    
    public int Quantity {get; set;}

    public InvenItems()
    {
    }

    public InvenItems(string name, int quantity)
    {
        Name = name;
        Quantity = quantity;
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
