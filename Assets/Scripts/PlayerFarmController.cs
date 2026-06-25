using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PolyAndCode.UI;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;


enum ButtonState
{
    none,
    normal,
    good,
    house
    
}

public class PlayerFarmController : MonoBehaviour
{
    public Tilemap tm_Ground;
    public Tilemap tm_Grass;
    public Tilemap tm_GroundOutSide;
    public Tilemap tm_Forest;
    public Tilemap tm_Sprout;
    public Tilemap tm_House;

    public TileBase tb_Ground;
    public TileBase tb_Grass;
    public TileBase tb_Forest;
    public TileBase tb_Sprout;
    public TileBase tb_House;
    
    public TileMapManager tilemapManager;
    
    public List<TileBase> lstTb_Rice;

    private ButtonState state = ButtonState.none;
    
    public Button Sell;
    public InputField SellInput;
    public Button Accept;
    public GameObject QuanlitySell;
    public GameObject SellStore;
    
    public Button BuyNormal;
    public Button BuyGood;
    public Button BuyHouse;
    
    public InputField BuyInput;
    public Button AcceptBuy;
    public GameObject QuanlityBuy;

    [SerializeField] private RecyclableInventoryManager _recyclableScrollRect;
    
    [SerializeField] 
    private MessageBox _messageBox;
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
                        bool isSuccess;
                        _recyclableScrollRect.SellItem(quanlitySell, out isSuccess);
                        if (isSuccess)
                        {
                            _messageBox.ShowPopup("Bán thành công");
                        }
                        else
                        {
                            
                            _messageBox.ShowPopup("Bán thất bại");

                        }
                            
                    }
                }
                
                QuanlitySell.SetActive(false);
            }
        );
        
        BuyNormal.onClick.AddListener(()=>
        {
            QuanlityBuy.SetActive(true);
            state =  ButtonState.normal;
        });

        BuyGood.onClick.AddListener(()=>
        {
            QuanlityBuy.SetActive(true);
            state =  ButtonState.good;
            
        });
        
        BuyHouse.onClick.AddListener(()=>
        {
            QuanlityBuy.SetActive(true);
            state =  ButtonState.house;
            
        });
        
        AcceptBuy.onClick.AddListener(() =>
            {
                string inputText = BuyInput.text;
                if (int.TryParse(inputText, out int quanlityBuy))
                {
                    if (quanlityBuy > 0)
                    {
                        int sproutId = 0;
                        if (state == ButtonState.normal)
                        {
                            sproutId = 2;
                        }
                        else if (state == ButtonState.good)
                        {
                            sproutId = 3;
                        }
                        else
                        {
                            sproutId = 5;
                        }
                        
                        bool isSuccess;
                        _recyclableScrollRect.BuyItem(quanlityBuy, sproutId, out isSuccess);
                        if (isSuccess)
                        {
                            _messageBox.ShowPopup("Mua thành công");
                        }
                        else
                        {
                            _messageBox.ShowPopup("Mua thất bại");
                        }
                    }
                }
                state = ButtonState.none;
                QuanlityBuy.SetActive(false);
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
                TileMapDetail tileMapDetail = LoadDataManager.userInGame.MapInGame.lstmap.FirstOrDefault(e =>e.x == cellPos.x && e.y == cellPos.y);
                if (tileMapDetail == null)
                {
                    tilemapManager.AddStateForTilemapDetail(cellPos.x, cellPos.y, State.Ground);
                }
                else
                {
                    tilemapManager.SetStateForTilemapDetail(cellPos.x, cellPos.y, State.Ground);
                }
                
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
            int selectedId = _recyclableScrollRect.selectedItemId;
            if (selectedId == -1)
            {
                _messageBox.ShowPopup("Hãy chọn hạt giống trong túi");
                return;
            }

            Vector3Int cellPos = tm_Ground.WorldToCell(transform.position);
            TileBase title1 = tm_Forest.GetTile(cellPos);
            if(title1 != null)
                return;
            InvenItems selectedSeed = _recyclableScrollRect._invenItems.FirstOrDefault(x => x.Id == _recyclableScrollRect.selectedItemId);
            if (selectedSeed == null)
            {
                _messageBox.ShowPopup("Chọn hạt giống khác");
                return;
            }
            TileBase title = tm_Grass.GetTile(cellPos);
            TileBase titleOutSide = tm_GroundOutSide.GetTile(cellPos);
            TileBase titleHouse= tm_House.GetTile(cellPos);
            if (selectedId == 5 && titleHouse ==null)
            {
                _recyclableScrollRect.PlantSproud(selectedSeed.Id);
                tm_House.SetTile(cellPos, tb_House);
                tilemapManager.AddStateForTilemapDetail(cellPos.x, cellPos.y, State.House);
                return;
            }
            if (title == null && titleOutSide == null)
            {
                _recyclableScrollRect.PlantSproud(selectedSeed.Id);
                // Khởi chạy Coroutine trồng cây
                StartCoroutine(GrowPlant(cellPos, tm_Forest, lstTb_Rice, selectedSeed.GrowthSpeed));
                tilemapManager.SetStateForTilemapDetail(cellPos.x, cellPos.y, State.Plants);
            }
            
        }
		
        if (Input.GetKeyDown(KeyCode.M))
        {
            Vector3Int cellPos = tm_Ground.WorldToCell(transform.position);
            TileBase titleSprout = tm_Sprout.GetTile(cellPos);
            if (titleSprout == tb_Sprout){
                tm_Grass.SetTile(cellPos, tb_Grass);
                tm_Sprout.SetTile(cellPos, null);
                    
                
                InvenItems itemSprout = new InvenItems();
                float lootOffset = 9999f; 
                float lootScale = 0.15f;

                float xCoord = (cellPos.x + lootOffset) * lootScale;
                float yCoord = (cellPos.y + lootOffset) * lootScale;

                // Tính toán "Chất lượng đất" tại ĐÚNG ô tọa độ đó
                float qualityRoll = Mathf.PerlinNoise(xCoord, yCoord);
                string qualityName = "";
    
                if (qualityRoll > 0.5f) {
                    itemSprout.Id = 4;
                    itemSprout.GrowthSpeed = 3;
                    qualityName = "Hạt giống đẹp";
                    itemSprout.Name = qualityName; 
                    itemSprout.Quantity = 1;
                    _recyclableScrollRect.AddInventory(itemSprout);
                    // 1. RANDOM CHẤT LƯỢNG (Ví dụ: 40% đẹp)
                    //tilemapManager.SetStateForTilemapDetail(cellPos.x, cellPos.y, State.Grass);
                }
                TileMapDetail removeSprout = LoadDataManager.userInGame.MapInGame.lstmap.FirstOrDefault(e =>e.x == cellPos.x && e.y == cellPos.y);
                tilemapManager.RemoveTilemapDetail(removeSprout);
                return;
            }
            
            TileBase title = tm_Forest.GetTile(cellPos);
            if (title == lstTb_Rice[4])
            {
                tm_Grass.SetTile(cellPos, tb_Grass);
                tm_Forest.SetTile(cellPos, null);
                
                //Lấy item và thêm vào túi đồ
                InvenItems itemRice = new InvenItems();
                itemRice.Id = 1;
                itemRice.Name = "Lúa";
                itemRice.Quantity = 1;
                itemRice.GrowthSpeed = 1;
                _recyclableScrollRect.AddInventory(itemRice);
                tilemapManager.SetStateForTilemapDetail(cellPos.x, cellPos.y, State.Grass);
                
            }
        }
    }
    public IEnumerator GrowPlant(Vector3Int cellPos,Tilemap tilemap, List<TileBase> lstTileBase, int multiplier)
    {
        int crrStage = 0;
        while (crrStage<lstTileBase.Count)
        {
            tilemap.SetTile(cellPos, lstTileBase[crrStage]);
            yield return new WaitForSeconds(5f / multiplier);
            crrStage++;
        }
    }
    
}

