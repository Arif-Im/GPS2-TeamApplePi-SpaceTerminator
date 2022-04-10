using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChoosenUnits : MonoBehaviour, IDropHandler
{

    public bool isUsed;
    public int unitCode;
    public Sprite pic;
    public GameObject unit;
    [SerializeField] GameObject standByMenu;

    public void OnDrop(PointerEventData eventData)
    {

        //Debug.Log("OnDrop");
        if(eventData.pointerDrag != null && isUsed == false)
        {
<<<<<<< Updated upstream
            eventData.pointerDrag.GetComponent<RectTransform>().position = GetComponent<RectTransform>().position;
=======
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
            
>>>>>>> Stashed changes
            unitCode = eventData.pointerDrag.GetComponent<UnitCode>().UNITCODE;
            pic = eventData.pointerDrag.GetComponent<UnitCode>().PIC;
            unit = eventData.pointerDrag.GetComponent<UnitCode>().PREFAB;
        }

        if (eventData.pointerDrag.GetComponent<DragnDropUnit>())
        {
            isUsed = true;
        }

    }

}
