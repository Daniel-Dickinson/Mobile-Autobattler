using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TwoBears.Shop
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(Button))]
    public abstract class CondionalButton : MonoBehaviour
    {
        [Header("States")]
        [SerializeField] private Color active = new Color(0.16f, 0.16f, 0.16f, 1.0f);
        [SerializeField] private Color inactive = new Color(0.12f, 0.12f, 0.12f, 1.0f);

        [Header("Icon")]
        [SerializeField] private CanvasGroup icon;

        private Image image;
        private Button button;

        //Mono
        protected virtual void Awake()
        {
            image = GetComponent<Image>();
            button = GetComponent<Button>();
        }

        //Core
        protected virtual void SetState(bool value)
        {
            //Set color
            image.color = value? active : inactive;

            //Set icon state
            if (icon != null) icon.alpha = value ? 1.0f : 0.4f;

            //Set button state
            button.enabled = value;
        }
    }
}