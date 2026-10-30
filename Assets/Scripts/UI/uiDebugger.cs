using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class uiDebugger : MonoBehaviour
{
    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (EventSystem.current == null)
            {
                Debug.LogError("No EventSystem in scene! This is likely your problem.");
                return;
            }

            var pointerData = new PointerEventData(EventSystem.current)
            {
                position = Mouse.current.position.ReadValue()
            };

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            if (results.Count == 0)
            {
                Debug.Log("Raycast hit NOTHING at " + pointerData.position);
            }
            else
            {
                Debug.Log($"--- Raycast hit {results.Count} object(s), top to bottom ---");
                foreach (var r in results)
                {
                    Debug.Log($"{r.gameObject.name}  (layer: {LayerMask.LayerToName(r.gameObject.layer)}, sortingOrder: {GetSortOrder(r)})");
                }
            }
        }
    }

    private string GetSortOrder(RaycastResult r)
    {
        var canvas = r.gameObject.GetComponentInParent<Canvas>();
        return canvas != null ? canvas.sortingOrder.ToString() : "n/a";
    }
}