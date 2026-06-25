using System.Collections;
using System.Collections.Generic;
using PolyAndCode.UI;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CellItemData : MonoBehaviour, ICell
{
    //UI 
    public Text nameLabel;
    public Text quantity;
    
    public Image Image;
    public Sprite SpriteRice;
    public Sprite SpriteSprout;
    public Sprite SpriteHouse;

    public Button _button;
    
    private void Awake()
    {
        _button.onClick.AddListener(OnCellClicked);
    }
    
    private void OnCellClicked()
    {
        RecyclableInventoryManager.Instance.SelectItem(_contactInfo.Id);
    }
    
    //Model 
    private InvenItems _contactInfo;
    private int _cellIndex;
    //This is called from the SetCell method in DataSource 
    public void ConfigureCell(InvenItems inventoryItems, int cellIndex)
    {
        _cellIndex = cellIndex;
        _contactInfo = inventoryItems;
        nameLabel.text = inventoryItems.Name;
        quantity.text = inventoryItems.Quantity.ToString();
        if (inventoryItems.Id==1)
        {
            Image.sprite = SpriteRice;
        }
        else if (inventoryItems.Id == 5)
        {
            Image.sprite = SpriteHouse;
        }
        else
        {
            Image.sprite = SpriteSprout;
        }
        
    }
}
