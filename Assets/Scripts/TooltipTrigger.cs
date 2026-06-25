using UnityEngine;
using UnityEngine.EventSystems;

// Implement IPointerEnterHandler (khi chuột đưa vào) và IPointerExitHandler (khi chuột đi ra)
public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea(3, 5)] // Mở rộng ô nhập text trong Inspector cho dễ gõ
    public string tooltipMessage = "Ghi hướng dẫn sử dụng nút ở đây...";

    // Hàm này tự động gọi khi bạn rê chuột VÀO UI
    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipManager.instance.ShowTooltip(tooltipMessage);
    }

    // Hàm này tự động gọi khi bạn đưa chuột RA KHỎI UI
    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.instance.HideTooltip();
    }
}