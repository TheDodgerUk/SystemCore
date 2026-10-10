using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportData
{
#if VR_INTERACTION

    private Autohand.Teleporter m_Teleporter;


    private bool m_IsTeleporting = false;
    public TeleportData(GameObject root)
    {

        m_Teleporter = root.GetComponentInChildren<Autohand.Teleporter>();
        if (m_Teleporter != null)
        {
            m_Teleporter.layer = Layers.TeleportMask;
            Autohand.Demo.XRTeleporterLink link = m_Teleporter.GetComponent<Autohand.Demo.XRTeleporterLink>();
            if (link != null)
            {
                Debug.LogError($"Needs to remove XRTeleporterLink", link.gameObject);
            }

            Autohand.Demo.OVRTeleporterLink ovrLink = m_Teleporter.GetComponent<Autohand.Demo.OVRTeleporterLink>();
            if (ovrLink != null)
            {
                Debug.LogError($"Needs to remove OVRTeleporterLink", ovrLink.gameObject);
            }
        }
        else
        {
            Debug.LogError("NOT FOUND Autohand.Teleporter");
        }
    }


    public void Add(InputButtonStateHandler state)
    {
        state.Begin += BeginTeleporter;
        state.End += EndTeleporter;
    }

    public void Remove(InputButtonStateHandler state)
    {
        state.Begin -= BeginTeleporter;
        state.End -= EndTeleporter;
    }

    private void BeginTeleporter(ControllerStateInteraction interaction, bool sendPhotonMessage)
    {
        if (interaction.Main.Hand == Handedness.Right)
        {
            if (m_IsTeleporting == false)
            {
                m_Teleporter?.StartTeleport();
                m_IsTeleporting = true;
            }
        }

    }
    private void EndTeleporter(ControllerStateInteraction interaction, bool sendPhotonMessage)
    {
        if (interaction.Main.Hand == Handedness.Right)
        {
            if (m_IsTeleporting == true)
            {
                m_Teleporter?.Teleport();
                m_IsTeleporting = false;
            }
        }

    }
#endif
}
