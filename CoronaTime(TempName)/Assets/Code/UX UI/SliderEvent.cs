using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class SliderEvent : MonoBehaviour, IPointerUpHandler {
    [HideInInspector] public UiElement uiElement;
    public void OnPointerUp(PointerEventData eventData) {
        AudioManager.PlaySound(uiElement.audioClip, uiElement.audioGroup);
    }
}