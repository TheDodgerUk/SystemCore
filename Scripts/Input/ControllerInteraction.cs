using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct ControllerInteraction
{
    public  List<InteractionSubscription> Subscriptions;
    public  List<ControllerData> Controllers;

    public ControllerInteraction(ControllerData controller, InteractionSubscription subscription)
    {
        Subscriptions = new List<InteractionSubscription> { subscription };
        Controllers = new List<ControllerData> { controller };
    }


    public ControllerInteraction(List<ControllerData> controllers, InteractionSubscription subscription)
    {
        Subscriptions = new List<InteractionSubscription> { subscription };
        Controllers = controllers;
    }


    public void UpdateControllerInteraction(List<ControllerData> controllers, InteractionSubscription subscription)
    {
        Subscriptions = new List<InteractionSubscription> { subscription };
        Controllers = controllers;
    }
    public void Invoke()
    {
        foreach (var sub in Subscriptions)
        {
            ProcessButton(i => i.Hover, sub.Hover);
            ProcessButton(i => i.BtnGrab, sub.Grab);
            ProcessButton(i => i.BtnStart, sub.BtnStart);
            ProcessButton(i => i.Interact, sub.Interact);
            ProcessButton(i => i.BtnPrimary, sub.BtnPrimary);
            ProcessButton(i => i.BtnSecondary, sub.BtnSecondary);
            ProcessButton(i => i.BtnStick, sub.ClickStick);

            ProcessVector1(ControllerInput.InputStateEnums.PullTrigger, sub.PullTrigger);
            ProcessVector1(ControllerInput.InputStateEnums.PullGrip, sub.PullGrip);
            ProcessVector2(ControllerInput.InputStateEnums.Stick, sub.Stick);

            ProcessButton(i => i.TchTrigger, sub.TchTrigger);
            ProcessButton(i => i.TchGrab, sub.TchGrab);
            ProcessButton(i => i.TchPrimary, sub.TchPrimary);
            ProcessButton(i => i.TchSecondary, sub.TchSecondary);
            ProcessButton(i => i.TchStick, sub.TchStick);
        }
    }

    private void ProcessButton(Func<ControllerInput, InputState> getState, InputButtonStateHandler handler)
    {
        var grouped = Controllers.GroupBy(c => getState(c.Inputs).State);
        var dict = grouped.ToDictionary(e => e.Key, e => e.ToList());
        dict.Remove(TriggerState.None);
        if (dict.Count > 0)
        {
            var down = dict.Get(TriggerState.Down);
            var held = dict.Get(TriggerState.Held);
            var up = dict.Get(TriggerState.Up);
            if (up != null)
            {
                handler.End?.Invoke(GetStateInteraction(up, held, getState));
                handler.BeginHandlerPressed = null;

                if (down != null)
                {
                    handler.Begin?.Invoke(GetStateInteraction(down, null, getState));
                }
            }
            else if (down != null)
            {
                handler.Begin?.Invoke(GetStateInteraction(down, held, getState));
                handler.BeginHandlerPressed = handler;
            }
            else if (held != null)
            {
                if (handler.BeginHandlerPressed == handler && handler != null)
                {
                    handler.Update?.Invoke(GetStateInteraction(held, null, getState));
                }
            }
            else
            {
                Debug.LogWarning($"Unexpected state! Down is {down}, Held is {held} and Up is {up}\n");
            }
        }
    }


    private void ProcessVector1(ControllerInput.InputStateEnums pull, InputVector1StateHandler handler)
    {
        foreach (var item in Controllers)
        {
            Vector1State data = null;
            switch (pull)
            {
                case ControllerInput.InputStateEnums.PullGrip:
                    data = item.Inputs.PullGrip;
                    break;
                case ControllerInput.InputStateEnums.PullTrigger:
                    data = item.Inputs.PullTrigger;
                    break;
                default:
                    Debug.LogError($"pull  {pull}");
                    break;
            }

            var sendData =  new ControllerStateInteraction(item, Controllers, Controllers);
            handler.Update?.Invoke(data.Value, sendData);
            if(data.Value == 0 && data.PrevValue != 0)
            {
                handler.Begin?.Invoke(data.Value, sendData);
            }

            if (data.Value == 1 && data.PrevValue != 1)
            {
                handler.End?.Invoke(data.Value, sendData);
            }
        }
    }

    private void ProcessVector2(ControllerInput.InputStateEnums pull, InputVector2StateHandler handler)
    {
        foreach (var item in Controllers)
        {
            Vector2State data = null;
            switch (pull)
            {
                case ControllerInput.InputStateEnums.Stick:
                    data = item.Inputs.Stick;
                    break;
                default:
                    Debug.LogError($"pull  {pull}");
                    break;
            }

            var sendData = new ControllerStateInteraction(item, Controllers, Controllers);
            handler.Update?.Invoke(data.Value2, sendData);
            if (data.Value2 == Vector2.zero && data.PrevValue2 != Vector2.zero)
            {
                handler.Begin?.Invoke(Vector2.zero, sendData);
            }

            if (data.Value2.sqrMagnitude == 1f && data.PrevValue2.sqrMagnitude != 1f)
            {
                handler.End?.Invoke(Vector2.zero, sendData);
            }
        }
    }



    private ControllerStateInteraction GetStateInteraction(List<ControllerData> active, List<ControllerData> additional, Func<ControllerInput, InputState> getState)
    {
        var main = active.OrderBy(c => getState(c.Inputs).Time.Start).First();
        if (additional != null)
        {
            additional.ForEach(active.Add);
        }
        return new ControllerStateInteraction(main, active, Controllers);
    }
}

public struct ControllerStateInteraction
{
    public readonly ControllerData Main;
    public readonly List<ControllerData> Active;
    public readonly List<ControllerData> Hovering;

    private Dictionary<string, ControllerData> m_ActiveById;

    public ControllerStateInteraction(ControllerData main, List<ControllerData> active, List<ControllerData> hovering)
    {
        Main = main;
        Active = active;
        Hovering = hovering;

        m_ActiveById = active.ToDictionary(c => c.UniqueId);
    }

    public ControllerData GetActive(string uniqueId) => m_ActiveById.Get(uniqueId);

    public ControllerStateInteraction Clone(ControllerData main, params ControllerData[] excluding)
    {
        var active = Active.Clone();
        active.Remove(main);
        active.Insert(0, main);

        var hovering = Hovering.Clone();
        hovering.Remove(main);
        hovering.Insert(0, main);

        foreach (var controller in excluding)
        {
            hovering.Remove(controller);
            active.Remove(controller);
        }
        return new ControllerStateInteraction(main, active, hovering);
    }
}