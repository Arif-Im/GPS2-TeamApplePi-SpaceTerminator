using UnityEngine;
using UnityEngine.EventSystems;

public class ChoosenUnits : MonoBehaviour, IDropHandler, IPointerExitHandler, IPointerUpHandler
{

    public bool isUsed;
    public int unitCode;
    public Sprite pic;
    public GameObject unit;
    [SerializeField] GameObject standByMenu;

    public void OnDrop(PointerEventData eventData)
    {

        //Debug.Log("OnDrop");
        if (eventData.pointerDrag != null && isUsed == false)
        {
            eventData.pointerDrag.GetComponent<RectTransform>().position = GetComponent<RectTransform>().position;

            unitCode = eventData.pointerDrag.GetComponent<UnitCode>().UNITCODE;
            pic = eventData.pointerDrag.GetComponent<UnitCode>().PIC;
            unit = eventData.pointerDrag.GetComponent<UnitCode>().PREFAB;
        }
        else// if(eventData.pointerDrag.GetComponent<RectTransform>().position != gameObject.transform.position)
        {
          
        }

        if (eventData.pointerDrag != null)
        {
            eventData.pointerDrag.GetComponent<RectTransform>().position = GetComponent<RectTransform>().position;
        }

        if (eventData.pointerDrag.GetComponent<DragnDropUnit>())
        {
            isUsed = true;
        }
        else
        {
            isUsed = false;
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {

        if (eventData.pointerDrag != null)
        {
            isUsed = false;
            unitCode = 0;
            pic = null;
            unit = null;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {

        if (eventData.pointerDrag != null)
        {
            isUsed = false;
            unitCode = 0;
            pic = null;
            unit = null;
        }

    }
}

