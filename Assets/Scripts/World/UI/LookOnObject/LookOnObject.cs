using System;
using TMPro;
using UnityEngine;

namespace World.UI.LookOnObject
{
    public abstract class LookOnObject : MonoBehaviour, IInteraction
    {
        public CanvasGroup canvasGroup;
        public TMP_Text lookText;
        public Color defaultTextColor;

        public bool isInteracting;
        public abstract void StartInteract();
        public abstract void StopInteract();
    }
}