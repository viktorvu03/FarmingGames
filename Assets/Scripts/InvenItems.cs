using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class InvenItems
{
    public string Name {get; set;}
    public string Description {get; set;}

    public InvenItems()
    {
    }

    public InvenItems(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
