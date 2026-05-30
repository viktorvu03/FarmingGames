using System.Collections;
using System.Collections.Generic;
using PolyAndCode.UI;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerFarmController : MonoBehaviour
{
    public Tilemap tm_Ground;
    public Tilemap tm_Grass;
    public Tilemap tm_Forest;

    public TileBase tb_Ground;
    public TileBase tb_Grass;
    public TileBase tb_Forest;
    
    public TitleMapManager tilemapManager;
    
    [SerializeField] 
    private RecyclableInventoryManager _recyclableScrollRect;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleFarmAction();
    }

    public void HandleFarmAction()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Vector3Int cellPos = tm_Ground.WorldToCell(transform.position);
            TileBase title = tm_Grass.GetTile(cellPos);
            if (title == tb_Grass)
            {
                tm_Grass.SetTile(cellPos, null);
                tilemapManager.SetStateForTilemapDetail(cellPos.x, cellPos.y, State.Ground);
            }
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            Vector3Int cellPos = tm_Ground.WorldToCell(transform.position);
            TileBase title = tm_Grass.GetTile(cellPos);
            if (title ==null)
            {
                tm_Forest.SetTile(cellPos, tb_Forest);
                tilemapManager.SetStateForTilemapDetail(cellPos.x, cellPos.y, State.Forest);
            }
        }
		
        if (Input.GetKeyDown(KeyCode.M))
        {
            Vector3Int cellPos = tm_Ground.WorldToCell(transform.position);
            TileBase title = tm_Forest.GetTile(cellPos);
            if (title !=null)
            {
                tm_Grass.SetTile(cellPos, tb_Grass);
                tm_Forest.SetTile(cellPos, null);
                
                //Lấy item và thêm vào túi đồ
                InvenItems itemFlower = new InvenItems();
                itemFlower.Name = "Flower";
                itemFlower.Description = "Hoa trang tri";
                _recyclableScrollRect.AddInventory(itemFlower);
            }
        }
    }
}
