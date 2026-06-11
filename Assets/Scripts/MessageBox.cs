using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
    [Header("UI References")]
    public GameObject popupPanel;       
    public Text popupText;   

    /// <summary>
    /// Hàm này để gọi khi muốn hiện thông báo
    /// </summary>
    /// <param name="message">Nội dung muốn hiển thị</param>
    public void ShowPopup(string message)
    {
        // 1. Gán chữ
        popupText.text = message;
        
        // 2. Bật Popup lên
        popupPanel.SetActive(true);

        // 3. Reset lại Coroutine (Đề phòng người chơi bấm mua 2 lần liên tục)
        StopAllCoroutines(); 
        
        // 4. Bắt đầu đếm ngược 1 giây để tắt
        StartCoroutine(HidePopupRoutine(1f)); 
    }

    // Tiến trình đếm ngược thời gian
    private IEnumerator HidePopupRoutine(float delay)
    {
        // Đợi 1 khoảng thời gian (delay)
        yield return new WaitForSeconds(delay);
        
        // Hết thời gian thì tắt Popup đi
        popupPanel.SetActive(false);
    }
}
