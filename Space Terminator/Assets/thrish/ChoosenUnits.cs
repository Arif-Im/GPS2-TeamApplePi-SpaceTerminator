using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChoosenUnits : MonoBehaviour, IDropHandler
{

    public bool isUsed;
    public int unitCode;

    public void OnDrop(PointerEventData eventData)
    {

        Debug.Log("OnDrop");
        if(eventData.pointerDrag != null && isUsed == false)
        {
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
            unitCode = eventData.pointerDrag.GetComponent<UnitCode>().UNITCODE;
        }

        if (eventData.pointerDrag.GetComponent<DragnDropUnit>())
        {
            isUsed = true;
        }

    }

}
