using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRUICanvas : MonoBehaviour
{
    [SerializeField]
    private UIReticle m_Reticle;

    private Canvas canvas;

    public void SetReticle(bool active, Vector3 position)
    {
        if (null != m_Reticle)
        {
            m_Reticle.SetReticle(active, position);
        }
    }

    private void Awake()
    {
        canvas = gameObject.GetComponent<Canvas>();
        bool found = false;
        if (null != canvas)
        {
            if(canvas.renderMode == RenderMode.WorldSpace)
            {
                canvas.worldCamera = VRUIPointer.EventCamera;
                var gfxCaster = gameObject.ForceComponent<VRUIGraphicsRaycaster>();
                gfxCaster.Init(this);
                gameObject.DestroyComponent<GraphicRaycaster>();

                var reticle = transform.Search("Reticle");

                if (null == reticle)
                {
                    reticle = transform.Search("Reticle_VRICanvas");
                }
                if (null != reticle)
                {
                    m_Reticle = reticle.gameObject.ForceComponent<UIReticle>();
                    m_Reticle.Init();
                    found = true;
                }
                else
                {
                    Debug.LogError($"Cannot find Reticle {this.gameObject.GetGameObjectPath()}", this);
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.Beep();
#endif
                }
            }
            else
            {
                DebugBeep.LogError($"Wrong canvas {canvas.renderMode}", DebugBeep.MessageLevel.High,  this.gameObject);
            }
        }
        else
        {
            DebugBeep.LogError("Canvas was not found", DebugBeep.MessageLevel.High, this.gameObject);
        }

        Debug.Log("Added VrInteractionCanvas");
        if (found == true)
        {
            // collider needs to be added first , to stop error messages 
            var rec = this.gameObject.GetComponent<RectTransform>();
            var box = this.gameObject.ForceComponent<BoxCollider>();
            box.size = new Vector3(rec.sizeDelta.x, rec.sizeDelta.y, 0.1f);
            if(rec.sizeDelta == Vector2.zero)
            {
                Core.Mono.WaitUntil(10, () => rec.sizeDelta != Vector2.zero, () =>
                {
                    box.size = new Vector3(rec.sizeDelta.x, rec.sizeDelta.y, 0.1f);
                });
                // needs extra time to find it all 
                Core.Mono.WaitFor(10, () =>
                {
                    if(rec.sizeDelta == Vector2.zero)
                    {
                        DebugBeep.LogError($"Add canvas is not correct, its still rec.sizeDelta == Vector2.zero", DebugBeep.MessageLevel.High, this.gameObject);
                    }
                });
            }
            box.center = new Vector3(0f, 0f, (box.size.z /2f));
            box.material = PhysicMaterials.UIPhysicMaterial;

            this.gameObject.ForceComponent<VrInteractionCanvas>();
        }
        

        //VRUIPointer.cs and VRInputModule.cs
    }
}