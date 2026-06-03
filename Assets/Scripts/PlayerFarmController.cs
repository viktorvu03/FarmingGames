using System.Collections;
using System.Collections.Generic;
using PolyAndCode.UI;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;


enum ButtonState
{
    none,
    normal,
    good,
    nice
    
}

public class PlayerFarmController : MonoBehaviour
{
    public Tilemap tm_Ground;
    public Tilemap tm_Grass;
    public Tilemap tm_GroundOutSide;
    public Tilemap tm_Forest;
    public Tilemap tm_Sprout;

    public TileBase tb_Ground;
    public TileBase tb_Grass;
    public TileBase tb_Forest;
    public TileBase tb_Sprout;
    
    public TitleMapManager tilemapManager;
    
    public List<TileBase> lstTb_Rice;

    private ButtonState state = ButtonState.none;
    
    public Button Sell;
    public InputField SellInput;
    public Button Accept;
    public GameObject QuanlitySell;
    public GameObject SellStore;
    
    public Button BuyNormal;
    public Button BuyGood;
    public Button BuyNice;
    
    public InputField BuyInput;
    public Button AcceptBuy;
    public GameObject QuanlityBuy;
    
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
        
        BuyNice.onClick.AddListener(()=>
        {
            QuanlityBuy.SetActive(true);
            state =  ButtonState.nice;
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

                QuanlitySell.SetActive(false);
            }
        );
        
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
                            sproutId = 4;
                        }
                        _recyclableScrollRect.BuyItem(quanlityBuy, sproutId);
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
            Vector3Int cellPos1 = tm_Ground.WorldToCell(transform.position);
            TileBase title1 = tm_Forest.GetTile(cellPos1);
            if(title1 != null)
                return;
            // 1. Tạo một danh sách tạm để chứa các hạt giống hợp lệ ĐANG CÓ trong túi đồv
            List<InvenItems> validSeedsInBag = new List<InvenItems>();

            foreach (InvenItems invenItems in _recyclableScrollRect._invenItems)
            {
                // Kiểm tra nếu đúng ID là 2, 3, hoặc 4
                if (invenItems.Id == 2 || invenItems.Id == 3 || invenItems.Id == 4)
                {
                    // BẮT BUỘC: Kiểm tra thêm điều kiện số lượng hạt đó phải lớn hơn 0
                    // (Giả sử biến số lượng của bạn là Quantity hoặc số lượng tương tự)
                    if (invenItems.Quantity > 0)
                    {
                        validSeedsInBag.Add(invenItems);
                    }
                }
            }

            if (validSeedsInBag.Count > 0)
            {
                // Tiến hành ngẫu nhiên chọn ra 1 phần tử trong danh sách hạt giống đang có
                int randomIndex = Random.Range(0, validSeedsInBag.Count);
                InvenItems selectedSeed = validSeedsInBag[randomIndex];
                
                // 3. Thực hiện logic kiểm tra vị trí đất và trồng cây bằng hạt giống đã chọn
                Vector3Int cellPos = tm_Ground.WorldToCell(transform.position);
                TileBase title = tm_Grass.GetTile(cellPos);
                TileBase titleOutSide = tm_GroundOutSide.GetTile(cellPos);

                if (title == null && titleOutSide == null)
                {
                    _recyclableScrollRect.PlantSproud(selectedSeed.Id);
                    // Khởi chạy Coroutine trồng cây
                    StartCoroutine(GrowPlant(cellPos, tm_Forest, lstTb_Rice, selectedSeed.GrowthSpeed));
                    tilemapManager.SetStateForTilemapDetail(cellPos.x, cellPos.y, State.Forest);
                }
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
                float qualityRoll = UnityEngine.Random.value;
                string qualityName = "";
    
                if (qualityRoll < 0.1f) {
                    itemSprout.Id = 4;
                    itemSprout.GrowthSpeed = 3;
                    qualityName = "Hạt giống đẹp";
                }
                else if (qualityRoll < 0.3f) {
                    itemSprout.Id = 3;
                    itemSprout.GrowthSpeed = 2;
                    qualityName = "Hạt giống tốt";
                }
                else
                {
                    itemSprout.Id = 2;
                    itemSprout.GrowthSpeed = 1;
                    qualityName = "Hạt giống thường";
                }

                itemSprout.Name = qualityName; 
                itemSprout.Quantity = 1;
                _recyclableScrollRect.AddInventory(itemSprout,1);
                // 1. RANDOM CHẤT LƯỢNG (Ví dụ: 70% Thường, 20% Tốt, 10% Thượng Hạng)
               
                
                tilemapManager.SetStateForTilemapDetail(cellPos.x, cellPos.y, State.Grass);
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
                _recyclableScrollRect.AddInventory(itemRice,1 );
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

