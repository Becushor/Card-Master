using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 originalPosition;

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = transform.position;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!GameController.instance.isPlayable)
            return;

        transform.position += (Vector3)eventData.delta;

        Card card = GetComponent<Card>();

        foreach (GameObject hover in eventData.hovered)
        {
            BurnZone burnZone = hover.GetComponent<BurnZone>();

            if (burnZone != null)
            {
                card.burnImage.gameObject.SetActive(true);
            }
            else
                card.burnImage.gameObject.SetActive(false);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.position = originalPosition;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}
