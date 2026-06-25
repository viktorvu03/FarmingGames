using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager instance;

    [Header("UI References")]
    public Text tooltipText;
    
    private void Awake()
    {
        // Khởi tạo Singleton
        if (instance == null)
        {
            instance = this;
        }
        // Đảm bảo tooltip ẩn khi mới chạy game
        gameObject.SetActive(false); 
    }

    // private void Update()
    // {
    //     // Làm cho Tooltip di chuyển theo con trỏ chuột
    //     // Nếu bạn muốn Tooltip đứng im, hãy xóa hàm Update này đi
    //     transform.position = Input.mousePosition;
    // }

    public void ShowTooltip(string message)
    {
        gameObject.SetActive(true);
        tooltipText.text = message;
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
        tooltipText.text = "";
    }
}
