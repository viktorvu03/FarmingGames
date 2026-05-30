using System.Collections;
using System.Collections.Generic;
using PolyAndCode.UI;
using TMPro;
using UnityEngine;

public class RecyclableInventoryManager : MonoBehaviour, IRecyclableScrollRectDataSource
{
    [SerializeField] 
    RecyclableScrollRect _recyclableScrollRect; 
 
    [SerializeField] 
    private int _dataLength;

    public GameObject inventory;
 
    //Dummy data List 
    private List<InvenItems> _invenItems = new List<InvenItems>(); 
 
    //Recyclable scroll rect's data source must be assigned in Awake. 
    private void Awake() 
    { 
        //InitData(); 
        _recyclableScrollRect.DataSource = this; 
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
        for (int i = 0; i < 50; i++)
        {
            InvenItems item = new InvenItems();
            item.Name = $"InvenItem_{i}";
            item.Description = $"InvenItem_{i}";
            listItems.Add(item);
        }
        _invenItems = listItems;
        _recyclableScrollRect.ReloadData();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            InvenItems item = new InvenItems("InvenItem_Demo","InvenDesc_Demo");
            _invenItems.Add(item);
            _recyclableScrollRect.ReloadData();
        }
        
        if (Input.GetKeyDown(KeyCode.B))
        {
           // inventory.SetActive(!inventory.activeSelf);
           Vector3 crrPosInven = inventory.GetComponent<RectTransform>().anchoredPosition;
           inventory.GetComponent<RectTransform>().anchoredPosition = crrPosInven.y == 1000 ? Vector3.zero : new Vector3(0,1000,0);
        }
        
    }
    
    public void AddInventory(InvenItems item)
    {
        _invenItems.Add(item);
        _recyclableScrollRect.ReloadData();
    }

}
