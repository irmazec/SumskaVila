using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UiMouseoverTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject tooltip;

    public void OnPointerEnter(PointerEventData data)
    {
        tooltip.SetActive(true);
    }

    public void OnPointerExit(PointerEventData data)
    {
        tooltip.SetActive(false);
    }
}
