using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class InvenItems
{
    public int Id { get; set; }
    public string Name {get; set;}
    public int Quantity {get; set;}
    public int GrowthSpeed  {get; set;}

    public InvenItems()
    {
    }

    public InvenItems(int id ,string name, int quantity, int growthSpeed)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        GrowthSpeed = growthSpeed;
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
