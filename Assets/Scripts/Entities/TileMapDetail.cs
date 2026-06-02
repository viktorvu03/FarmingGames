using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum State
{
    Ground,
    Grass,
    Forest,
}

public class TileMapDetail
{
    public int x { get; set; }
    public int y { get; set; }
    public State titlemapState { get; set; }
    
    public DateTime growTime { get; set; }

    public TileMapDetail(int x, int y, State titlemapState, DateTime growTime)
    {
        this.x = x;
        this.y = y;
        this.titlemapState = titlemapState;
        this.growTime = growTime;
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
