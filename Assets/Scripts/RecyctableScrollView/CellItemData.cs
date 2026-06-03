using System.Collections;
using System.Collections.Generic;
using PolyAndCode.UI;
using UnityEngine;
using UnityEngine.UI;

public class CellItemData : MonoBehaviour, ICell
{
    //UI 
    public Text nameLabel;
    public Text quantity;
    
    public Image Image;
    public Sprite SpriteRice;
    public Sprite SpriteSprout;

    //Model 
    private InvenItems _contactInfo;
    private int _cellIndex;
    //This is called from the SetCell method in DataSource 
    public void ConfigureCell(InvenItems invenItems, int cellIndex)
    {
        _cellIndex = cellIndex;
        _contactInfo = invenItems;
        nameLabel.text = invenItems.Name;
        quantity.text = invenItems.Quantity.ToString();
        if (invenItems.Id==1)
        {
            Image.sprite = SpriteRice;
        }
        else
        {
            Image.sprite = SpriteSprout;
        }
        
    }
}
