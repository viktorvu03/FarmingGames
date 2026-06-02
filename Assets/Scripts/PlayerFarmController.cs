using System.Collections;
using System.Collections.Generic;
using PolyAndCode.UI;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class PlayerFarmController : MonoBehaviour
{
    public Tilemap tm_Ground;
    public Tilemap tm_Grass;
    public Tilemap tm_GroundOutSide;
    public Tilemap tm_Forest;

    public TileBase tb_Ground;
    public TileBase tb_Grass;
    public TileBase tb_Forest;
    
    public TitleMapManager tilemapManager;
    
    public List<TileBase> lstTb_Rice;

    public Button Sell;
    public InputField SellInput;
    public Button Accept;
    public GameObject QuanlitySell;
    public GameObject SellStore;
    
    
    [SerializeField] 
    private RecyclableInventoryManager _recyclableScrollRect;
    // Start is called before the first frame update
    void Start()
    {
        Sell.onClick.AddListener(()=>
        {
            QuanlitySell.SetActive(true);
        });

        Accept.onClick.AddListener(() =>
            {
                string inputText = SellInput.text;
                if (int.TryParse(inputText, out int quanlitySell))
                {
                    if (quanlitySell > 0)
                    {
                        _recyclableScrollRect.SellItem(quanlitySell);
                    }
                }
                else
                {
                    
                }
                QuanlitySell.SetActive(false);
            }
        );
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
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (SellStore.activeSelf == false)
            {
                SellStore.SetActive(true);
            }
            else
            {
                SellStore.SetActive(false);
            }
            
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            Vector3Int cellPos = tm_Ground.WorldToCell(transform.position);
            TileBase title = tm_Grass.GetTile(cellPos);
            TileBase titleOutSide = tm_GroundOutSide.GetTile(cellPos);
            if (title ==null && titleOutSide ==null)
            {
                //tm_Forest.SetTile(cellPos, tb_Forest);
                StartCoroutine(GrowPlant(cellPos,tm_Forest,lstTb_Rice));
                tilemapManager.SetStateForTilemapDetail(cellPos.x, cellPos.y, State.Forest);
            }
        }
		
        if (Input.GetKeyDown(KeyCode.M))
        {
            Vector3Int cellPos = tm_Ground.WorldToCell(transform.position);
            TileBase title = tm_Forest.GetTile(cellPos);
            if (title == lstTb_Rice[4])
            {
                tm_Grass.SetTile(cellPos, tb_Grass);
                tm_Forest.SetTile(cellPos, null);
                
                //Lấy item và thêm vào túi đồ
                InvenItems itemRice = new InvenItems();
                itemRice.Name = "Thóc";
                itemRice.Quantity = 1;
                _recyclableScrollRect.AddInventory(itemRice);
                tilemapManager.SetStateForTilemapDetail(cellPos.x, cellPos.y, State.Grass);
                
            }
        }
    }
    public IEnumerator GrowPlant(Vector3Int cellPos,Tilemap tilemap, List<TileBase> lstTileBase)
    {
        int crrStage = 0;
        while (crrStage<lstTileBase.Count)
        {
            tilemap.SetTile(cellPos, lstTileBase[crrStage]);
            yield return new WaitForSeconds(5);
            crrStage++;
        }
    }
    
}

