

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
    public bool FollorCursor { get; set; } = true;
    RectTransform _rectTransfrom;
    Vector2 _startPos = new Vector2(0, 0);
    Transform _startParent;
    Canvas _canvas;
    public RuntimeItem RuntimeItem;
    public bool CanDrag { get; set; } = true;
    public object Addressable { get; private set; }


    void OnValidate()
    {
        if (gameObject.GetComponent<RectTransform>() != null)
        {
            float size = GetComponentInParent<InventoryView>().ItemStandardSize;
            _rectTransfrom = gameObject.GetComponent<RectTransform>();
            _rectTransfrom.sizeDelta = new Vector2(size, size);
            _canvas = GetComponentInParent<Canvas>();
        }
        else
        {
            Debug.Log("No Rect Transform on object m8");
            return;
        }


        if (RuntimeItem != null)
        {
            gameObject.name = RuntimeItem.Item.Name + "-Item";
        }

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!CanDrag)
            return;

        _startParent = transform.parent;
        transform.SetParent(transform.parent.parent.parent.parent);
        OnBeginDragHandler?.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!CanDrag)
            return;

        OnDragHandler?.Invoke(eventData);

        if (!FollorCursor)
            return;

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
        _rectTransfrom.transform.SetParent(_startParent);
        _rectTransfrom.anchoredPosition = _startPos;
        OnEndDragHandler?.Invoke(eventData, false);
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        _startPos = _rectTransfrom.anchoredPosition;
    }
}
