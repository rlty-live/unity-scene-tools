using RLTY.Customisation;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ReflectionProbe))]
public class UpdateProbe : RLTYMonoBehaviour
{
    ReflectionProbe probe;
    static List<UpdateProbe> customProbes;
    [ReadOnly]
    public bool updated;

    public void RenderProbe()
    {
        if(correctSetup)
        {
            probe.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.ViaScripting;
            probe.RenderProbe();
        }

    }

    public override void CheckSetup()
    {
        base.CheckSetup();

        if (TryGetComponent<ReflectionProbe>(out ReflectionProbe _probe))
        {
            probe = _probe;
            updated = true;
            correctSetup = true;
        }


        else
            if (debug) 
            Debug.Log("Reflection Probe missing, can't update", this);
    }

    public void ActivateRealTimeProbe()
    {
        //If player is close enough activate reflection rendering

        //Set Layer Masks to Players only
    }

    public override void EventHandlerRegister()
    {
        CustomisationManagerHandlerData.OnSceneCustomized += RenderProbe;
    }

    public override void EventHandlerUnRegister()
    {
        CustomisationManagerHandlerData.OnSceneCustomized -= RenderProbe;
    }
}
