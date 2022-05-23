using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShieldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] UiManager _uiManager;
    public void OnPointerDown(PointerEventData eventData)
    {
        _uiManager.PressedShieldButton();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _uiManager.UnPressedShieldButton();
    }
}
