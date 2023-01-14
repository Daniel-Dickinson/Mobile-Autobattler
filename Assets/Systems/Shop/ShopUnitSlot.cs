using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TwoBears.Shop
{
    [RequireComponent(typeof(AudioSource))]
    public class ShopUnitSlot : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
    {
        [Header("Parent")]
        public Transform iconParent;

        [Header("Hover")]
        public CanvasGroup hover;

        [Header("Behaviour")]
        public bool sellable = false;
        public bool stackable = false;

        //Audio
        private AudioSource aud;

        //Contents
        public ShopUnitIcon Icon
        {
            get { return icon; }
        }
        protected ShopUnitIcon icon;

        //Drag operation
        protected static DragOperation drag;    //Tracks drags
        protected static bool dragged = false;  //Tacks if dragged since last mouse down

        //Mono
        private void Awake()
        {
            //Hover defaults to off
            hover.alpha = 0.0f;

            //Grab audio source
            aud = GetComponent<AudioSource>();
        }

        //Checks
        public virtual bool CanRemoveUnit()
        {
            return true;
        }
        public virtual bool CanPlaceUnit(ShopUnitIcon icon)
        {
            //Check if icon is valid
            if (icon == null) return false;

            //Check if slot is free
            if (this.icon == null) return true;

            //Check if unit can stack
            if (stackable && this.icon.Unit.Stackable(icon.Unit.sublevel + 1) && this.icon.Unit.id == icon.Unit.id && this.icon.Unit.level == icon.Unit.level) return true;

            //Otherwise cannot
            return false;
        }

        //Remove & Place
        public virtual bool RemoveUnit()
        {
            //Unit must be takeable
            if (!CanRemoveUnit()) return false;

            //Icon is now null
            icon = null;

            //Take successful
            return true;
        }
        public virtual bool PlaceUnit(ShopUnitIcon icon)
        {
            //Unit must be placeable
            if (!CanPlaceUnit(icon)) return false;

            //Stack or place
            if (stackable && this.icon != null)
            {
                //Stack existing unit
                this.icon.Unit.Stack(icon.Unit.sublevel + 1);

                //Destroy icon
                Destroy(icon.gameObject);
            }
            else
            {
                //Place icon
                this.icon = icon;

                //Grab transform
                RectTransform rect = icon.transform as RectTransform;

                //Reset scale
                rect.localScale = Vector3.one;

                //Set parent -- must be after scale
                rect.SetParent(iconParent, false);

                //Reset position
                rect.anchoredPosition = Vector2.zero;
            }

            //Placement successful
            return true;
        }

        //Hover Events
        public virtual void OnPointerDown(PointerEventData eventData)
        {
            //Must be a valid drag
            if (icon == null || !CanRemoveUnit()) return;

            //Play on pointer down
            if (aud != null) aud.Play();

            //Not yet dragging
            dragged = false;

            //Highlight
            hover.alpha = 1.0f;
        }
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            //Highlight
            hover.alpha = 1.0f;
        }
        public virtual void OnPointerExit(PointerEventData eventData)
        {
            //Clear highlight
            hover.alpha = 0.0f;
        }
        public virtual void OnPointerUp(PointerEventData eventData)
        {
            
        }
        
        //Drag Events
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            //Must be a valid drag
            if (icon == null) return;

            //Now dragging on this mouse down
            dragged = true;

            //Check if unit can be moved
            if (!CanRemoveUnit()) return;

            //Grab pre-requisites
            drag = new DragOperation(this, icon);

            //Sell
            if (sellable) Shop.SetSellState(true);
        }
        public virtual void OnDrag(PointerEventData eventData)
        {
            //Ignore if not dragging
            if (drag == null) return;

            //Update drag
            drag.Update(this, eventData.delta);
        }
        public virtual void OnEndDrag(PointerEventData eventData)
        {
            //Ignore if not dragging
            if (drag == null) return;

            //Play on clear
            if (aud != null) aud.Play();

            //Clear sell state
            Shop.SetSellState(false);

            //Clear drag
            drag.Clear(this);
            drag = null;
        }

        //Drop Event
        public void OnDrop(PointerEventData eventData)
        {
            //Ignore if not dragging
            if (drag == null) return;

            //Check if drop is valid
            if (!CanPlaceUnit(drag.Icon)) return;

            //Attempt to remove from previous slot
            if (!drag.Source.RemoveUnit()) return;

            //Place in new slot
            PlaceUnit(drag.Icon);

            //Play on drop
            if (aud != null) aud.Play();

            //Clear sell state
            Shop.SetSellState(false);

            //Clear drag
            drag = null;
        }

        //Drag operation
        protected class DragOperation
        {
            //Canvas
            private Canvas canvas;

            //Icon
            private ShopUnitIcon icon;
            private RectTransform iconTransform;
            private RectTransform iconParent;

            //Source
            private ShopUnitSlot source;

            //Access
            public ShopUnitIcon Icon
            {
                get { return icon; }
            }
            public ShopUnitSlot Source
            {
                get { return source; }
            }

            //Constructor
            public DragOperation(ShopUnitSlot source, ShopUnitIcon icon)
            {
                //Source
                this.source = source;

                //Canvas
                canvas = source.GetComponentInParent<Canvas>();

                //Icon
                this.icon = icon;
                iconTransform = icon.GetComponent<RectTransform>();
                iconParent = iconTransform.parent as RectTransform;

                //Parent to canvas (Appears on top)
                iconTransform.SetParent(canvas.transform, true);
            }

            //Operation
            public void Update(ShopUnitSlot source, Vector2 delta)
            {
                //Validate source
                if (this.source != source) return;

                //Move icon to follow drag
                iconTransform.anchoredPosition += delta / canvas.scaleFactor;
            }
            public void Clear(ShopUnitSlot source)
            {
                //Validate source
                if (this.source != source) return;

                //Move icon back to original position
                iconTransform.anchoredPosition = Vector2.zero;

                //Reset scale
                iconTransform.localScale = Vector3.one;

                //Parent back to original transform
                iconTransform.SetParent(iconParent, false);
            }
        }
    }
}