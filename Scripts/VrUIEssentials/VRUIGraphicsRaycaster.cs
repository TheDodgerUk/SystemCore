using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class VRUIGraphicsRaycaster : GraphicRaycaster
{
    protected Canvas currentCanvas;
    protected Vector2 lastKnownPosition;
    protected const float UI_CONTROL_OFFSET = 0.00001f;
    private VRUICanvas m_VRCanvas;

    public void Init(VRUICanvas canvasController)
    {
        m_VRCanvas = canvasController;
    }

    [NonSerialized]
    // Use a static to prevent list reallocation. We only need one of these globally (single main thread), and only to hold temporary data
    private static List<RaycastResult> s_RaycastResults = new List<RaycastResult>();

    public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
    {
        if (canvas == null || eventCamera == null)
        {
            return;
        }

        Ray ray = new Ray(eventData.pointerCurrentRaycast.worldPosition, eventData.pointerCurrentRaycast.worldNormal);
        Raycast(canvas, eventCamera, eventData, ray, ref s_RaycastResults);
        SetNearestRaycast(ref eventData, ref resultAppendList, ref s_RaycastResults);
        s_RaycastResults.Clear();
    }

    //[Pure]
    protected virtual void SetNearestRaycast(ref PointerEventData eventData, ref List<RaycastResult> resultAppendList, ref List<RaycastResult> raycastResults)
    {
        RaycastResult? nearestRaycast = null;
        for (int index = 0; index < raycastResults.Count; index++)
        {
            RaycastResult castResult = raycastResults[index];
            castResult.index = resultAppendList.Count;
            if (!nearestRaycast.HasValue || castResult.distance < nearestRaycast.Value.distance)
            {
                nearestRaycast = castResult;
            }
            AddListValue(resultAppendList, castResult);
        }

        if (nearestRaycast.HasValue)
        {
            eventData.position = nearestRaycast.Value.screenPosition;
            eventData.delta = eventData.position - lastKnownPosition;
            lastKnownPosition = eventData.position;
            eventData.pointerCurrentRaycast = nearestRaycast.Value;
        }
    }

    //[Pure]
    protected virtual float GetHitDistance(Ray ray, float hitDistance)
    {
        if (canvas.renderMode != RenderMode.ScreenSpaceOverlay && blockingObjects != BlockingObjects.None)
        {
            float maxDistance = Vector3.Distance(ray.origin, canvas.transform.position);

            if (blockingObjects == BlockingObjects.ThreeD || blockingObjects == BlockingObjects.All)
            {
                RaycastHit hit;
                Physics.Raycast(ray, out hit, maxDistance, m_BlockingMask);
                if (hit.collider != null)
                {
                    hitDistance = hit.distance;
                }
            }

            if (blockingObjects == BlockingObjects.TwoD || blockingObjects == BlockingObjects.All)
            {
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, maxDistance);

                if (hit.collider != null)
                {
                    hitDistance = hit.fraction * maxDistance;
                }
            }
        }
        return hitDistance;
    }

    //[Pure]
    protected virtual void Raycast(Canvas canvas, Camera eventCamera, PointerEventData eventData, Ray ray, ref List<RaycastResult> results)
    {
        float hitDistance = GetHitDistance(ray, VRUIPointer.PointerLength);
        IList<Graphic> canvasGraphics = GraphicRegistry.GetGraphicsForCanvas(canvas);
        bool bHasValidSurface = false;
        Vector3 worldPosition = Vector3.zero;

        for (int i = 0; i < canvasGraphics.Count; ++i)
        {
            Graphic graphic = canvasGraphics[i];

            if (graphic.depth == -1 || !graphic.raycastTarget)
            {
                continue;
            }

            Transform graphicTransform = graphic.transform;
            Vector3 graphicForward = graphicTransform.forward;
            float distance = Vector3.Dot(graphicForward, graphicTransform.position - ray.origin) / Vector3.Dot(graphicForward, ray.direction);

            if (distance < 0)
            {
                continue;
            }

            //Prevents "flickering hover" on items near canvas center.
            if ((distance - UI_CONTROL_OFFSET) > hitDistance)
            {
                continue;
            }

            Vector3 position = ray.GetPoint(distance);
            Vector2 pointerPosition = eventCamera.WorldToScreenPoint(position);

            if (!RectTransformUtility.RectangleContainsScreenPoint(graphic.rectTransform, pointerPosition, eventCamera))
            {
                continue;
            }
            
            if (graphic.Raycast(pointerPosition, eventCamera))
            {
                RaycastResult result = new RaycastResult()
                {
                    gameObject = graphic.gameObject,
                    module = this,
                    distance = distance,
                    screenPosition = pointerPosition,
                    worldPosition = position,
                    depth = graphic.depth,
                    sortingLayer = canvas.sortingLayerID,
                    sortingOrder = canvas.sortingOrder,
                };
                AddListValue(results, result);
            }

            bHasValidSurface = true;
            worldPosition = position;
        }


        
        if (null != m_VRCanvas)
        {
            m_VRCanvas.SetReticle(bHasValidSurface, worldPosition);
        }
        results.Sort((g1, g2) => g2.depth.CompareTo(g1.depth));


    }

    protected virtual Canvas canvas
    {
        get
        {
            if (currentCanvas != null)
            {
                return currentCanvas;
            }

            currentCanvas = gameObject.GetComponent<Canvas>();
            return currentCanvas;
        }
    }

    private static bool AddListValue<TValue>(List<TValue> list, TValue value, bool preventDuplicates = false)
    {
        if (list != null && (!preventDuplicates || !list.Contains(value)))
        {
            list.Add(value);
            return true;
        }
        return false;
    }
}