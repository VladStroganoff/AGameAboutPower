

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ItemView : MonoBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public event Action<PointerEventData> OnBeginDragHandler;
    public event Action<PointerEventData> OnDragHandler;
    public event Action<PointerEventData, bool> OnEndDragHandler;
    RectTransform _rectTransfrom;
    Vector2 _startPos = new Vector2(0, 0);
    public Transform currentParent { get; set; }
    Canvas _canvas;
    public RuntimeItem RuntimeItem;
    public bool CanDrag { get; set; } = true;
    public object Addressable { get; private set; }


    void Start()
    {
        currentParent = transform.parent;

        if (gameObject.GetComponent<RectTransform>() != null)
        {
            _rectTransfrom = gameObject.GetComponent<RectTransform>();
            _canvas = GetComponentInParent<Canvas>();
            
            if (transform.parent.GetComponent<WearableSlot>() != null)
                CanDrag = false;
        }
        else
        {
            FDebug.Log.Message("No Rect Transform on object m8");
            return;
        }

    }
    void OnValidate()
    {
        if (RuntimeItem != null)
        {
            gameObject.name = RuntimeItem.Item.Name + "-Item";
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!CanDrag)
            return;

        currentParent = transform.parent;
        transform.SetParent(transform.parent.parent.parent.parent);
        OnBeginDragHandler?.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!CanDrag)
            return;
        OnDragHandler?.Invoke(eventData);
        _rectTransfrom.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!CanDrag)
            return;

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        DropArea slot = null;

        foreach(var result in results)
        {
            slot = result.gameObject.GetComponent<DropArea>();

            if (slot != null)
                break;
        }

        if (slot != null)
        {
            if (slot.Accepts(this))
            {
                slot.Drop(this);
                OnEndDragHandler?.Invoke(eventData, true);
                return;
            }
        }
        _rectTransfrom.transform.SetParent(currentParent); // revert back to old parent
        _rectTransfrom.anchoredPosition = _startPos;
        


        OnEndDragHandler?.Invoke(eventData, false);
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        _startPos = _rectTransfrom.anchoredPosition;
    }
}
