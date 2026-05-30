using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class Map 
{
    public List<TileMapDetail> lstmap { get; set; }

    public Map()
    {
        
    }
    
    public Map(List<TileMapDetail> lstmap)
    {
        this.lstmap = lstmap;
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
    
    public int GetLength()
    {
        if (lstmap == null) return 0;
        return lstmap.Count;
    }
}
