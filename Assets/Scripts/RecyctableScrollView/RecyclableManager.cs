using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PolyAndCode.UI;
using TMPro;
using UnityEngine;
using Firebase.Database;
using Firebase;

public class RecyclableInventoryManager : MonoBehaviour, IRecyclableScrollRectDataSource
{
    [Header("Global Selection Data")]
    // Biến toàn cục lưu trữ ID của vật phẩm vừa được bấm chọn
    // Mặc định ban đầu chưa chọn gì thì để là -1
    public int selectedItemId = -1;
    
    [SerializeField] 
    RecyclableScrollRect _recyclableScrollRect; 
 
    [SerializeField] 
    private int _dataLength;

    public GameObject inventory;

    private FirebaseDatabaseManager dataDatabaseManager;

    public UsernameWizard usernameWizard;
    
    public RecyclableInventoryManager(List<InvenItems> _invenItems)
    {
        this._invenItems = _invenItems;
    }
    public RecyclableInventoryManager()
    {
        
    }
    // Biến Singleton toàn cục để mọi script khác đều gọi được
    public static RecyclableInventoryManager Instance { get; private set; }
    
    public List<InvenItems> GetInventoryDataForSave()
    {
        return _invenItems;
    }
    
    private DatabaseReference _reference;
    
    //Dummy data List 
    public List<InvenItems> _invenItems = new List<InvenItems>(); 
 
    //Recyclable scroll rect's data source must be assigned in Awake. 
    private void Awake() 
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        dataDatabaseManager = GameObject.Find("DatabaseManager").GetComponent<FirebaseDatabaseManager>();
        FirebaseApp app = FirebaseApp.DefaultInstance;
        _reference = FirebaseDatabase.DefaultInstance.RootReference;
        //InitData(); 
        _recyclableScrollRect.DataSource = this; 
        Vector3 crrPosInven = inventory.GetComponent<RectTransform>().anchoredPosition;
        inventory.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 1000, 0);
    } 
 
    #region DATA-SOURCE 
 
    /// <summary> 
    /// Data source method. return the list length. 
    /// </summary> 
    public int GetItemCount() 
    { 
        return _invenItems.Count; 
    } 
    /// <summary> 
    /// Called for a cell every time it is recycled 
    /// Implement this method to do the necessary cell configuration. 
    /// </summary> 
    public void SetCell(ICell cell,int index) 
    { 
        //Casting to the implemented Cell 
        var item = cell as CellItemData; 
        item.ConfigureCell(_invenItems[index],index); 
    }

    
    #endregion 
    public void Start()
    {
        List<InvenItems> listItems = new List<InvenItems>();
        // for (int i = 0; i < 50; i++)
        // {
        //     InvenItems item = new InvenItems();
        //     item.Name = $"InvenItem_{i}";
        //     item.Description = $"InvenItem_{i}";
        //     listItems.Add(item);
        // }
        if (LoadDataManager.userInGame.inventoryItems != null)
        {
            listItems = LoadDataManager.userInGame.inventoryItems;
        }
        _invenItems = listItems;
        _recyclableScrollRect.ReloadData();
    }

    public void Update()
    {
        // if (Input.GetKeyDown(KeyCode.L))
        // {
        //     InvenItems item = new InvenItems("InvenItem_Demo","InvenDesc_Demo");
        //     _invenItems.Add(item);
        //     _recyclableScrollRect.ReloadData();
        // }
        
        if (Input.GetKeyDown(KeyCode.B))
        {
           // inventory.SetActive(!inventory.activeSelf);
           Vector3 crrPosInven = inventory.GetComponent<RectTransform>().anchoredPosition;
           inventory.GetComponent<RectTransform>().anchoredPosition = crrPosInven.y == 1000 ? Vector3.zero : new Vector3(0,1000,0);
        }
        
    }
    
    public void SelectItem(int itemId)
    {
        if (itemId == -1 || itemId == 1 || _invenItems.FirstOrDefault(x => x.Id == itemId) == null)
        {
            
            return;
        }
        selectedItemId = itemId;
    }
    
    public void AddInventory(InvenItems item)
    {
        foreach (InvenItems invItems in _invenItems)
        {
            if (invItems.Id == item.Id)
            {
                invItems.Quantity += item.Quantity;
                _recyclableScrollRect.ReloadData();
                LoadDataManager.userInGame.inventoryItems = _invenItems;
                dataDatabaseManager.WriteDatabase("Users/" + LoadDataManager.firebaseUser.UserId,LoadDataManager.userInGame.ToString());
                return;
            }
        }
        _invenItems.Add(item);
        LoadDataManager.userInGame.inventoryItems = _invenItems;
        dataDatabaseManager.WriteDatabase("Users/" + LoadDataManager.firebaseUser.UserId,LoadDataManager.userInGame.ToString());
        _recyclableScrollRect.ReloadData();
    }
    
    public void SellItem(int quanlity, out bool isSuccess)
    {
        
            foreach (InvenItems invItems in _invenItems)
            {
                // Kiểm tra đúng tên vật phẩm và số lượng bán hợp lệ
                if (invItems.Id == 1 && quanlity <= invItems.Quantity)
                {
                    isSuccess = true;
                    // 1. Cộng tiền cho người chơi (cả 2 trường hợp đều được cộng tiền như nhau)
                    LoadDataManager.userInGame.Gold += quanlity * 50;
                    // 2. Xử lý số lượng trong túi đồ
                    if (quanlity < invItems.Quantity)
                    {
                        invItems.Quantity -= quanlity;
                    }
                    else if (quanlity == invItems.Quantity)
                    {
                        _invenItems.Remove(invItems);
                    }
                    LoadDataManager.userInGame.inventoryItems = _invenItems;
                    dataDatabaseManager.WriteDatabase("Users/" + LoadDataManager.firebaseUser.UserId, LoadDataManager.userInGame.ToString());
        
                    _recyclableScrollRect.ReloadData(); // Cập nhật lại UI vì số lượng list đã thay đổi
                    usernameWizard.ReloadUI();
                    return; 
                }
            }
            isSuccess = false;
    }
    
    public void BuyItem(int quanlity, int sproutId, out bool isSuccess)
    {
        int price = 0;
        if (sproutId == 2)
        {
            price = 10;
        }
        else if(sproutId == 3)
        {
            price = 20;
        }
        else
        {
            price = 200;
        }
        
        
        int moneyCanBuy = price * quanlity;
        int userGold = LoadDataManager.userInGame.Gold;
        if(moneyCanBuy <= userGold)
        {
            InvenItems itemSprout = new InvenItems();
            LoadDataManager.userInGame.Gold -= moneyCanBuy;
            if (sproutId == 3) {
                itemSprout.Id = 3;
                itemSprout.GrowthSpeed = 2;
                itemSprout.Name = "Hạt giống tốt";
                itemSprout.Quantity = quanlity;
            }
            else if(sproutId == 2)
            {
                itemSprout.Id = 2;
                itemSprout.GrowthSpeed = 1;
                itemSprout.Name = "Hạt giống thường";
                itemSprout.Quantity = quanlity;
            }
            else
            {
                itemSprout.Id = 5;
                itemSprout.GrowthSpeed = 1;
                itemSprout.Name = "Nhà";
                itemSprout.Quantity = quanlity;
            }
            isSuccess = true;
            AddInventory(itemSprout);
        }
        else
        {
            isSuccess = false;
        }
        usernameWizard.ReloadUI();
        
    }
    
    public void PlantSproud(int id)
    {
        foreach (InvenItems invItems in _invenItems)
        {
            // Kiểm tra đúng tên vật phẩm và số lượng bán hợp lệ
            if (invItems.Id == id)
            {
                // 2. Xử lý số lượng trong túi đồ
                if (invItems.Quantity == 1)
                {
                    _invenItems.Remove(invItems);
                }
                else
                {
                    invItems.Quantity -= 1;
                }
                LoadDataManager.userInGame.inventoryItems = _invenItems;
                dataDatabaseManager.WriteDatabase("Users/" + LoadDataManager.firebaseUser.UserId, LoadDataManager.userInGame.ToString());
        
                _recyclableScrollRect.ReloadData(); // Cập nhật lại UI vì số lượng list đã thay đổi
                usernameWizard.ReloadUI();
                return; 
            }
        }
    }
    
    

    public int GetLength()
    {
        if (_invenItems == null) return 0;
        return _invenItems.Count;
    }
}
