using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    public class InputController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {

        public Action<PointerEventData> OnBeginDragging;
        public Action<PointerEventData> OnEndDragging;
        public Action<PointerEventData> OnDragging;
        public Action<PointerEventData> OnClick;

        public void OnBeginDrag(PointerEventData eventData)
        {
            OnBeginDragging?.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            OnDragging?.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnEndDragging?.Invoke(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick?.Invoke(eventData);
        }
    }
}
