using System.Collections.Generic;

public class InteractionSubscription
{
    public readonly InputButtonStateHandler Hover = new InputButtonStateHandler();
    public readonly InputButtonStateHandler Grab = new InputButtonStateHandler();

    public readonly InputButtonStateHandler BtnStart = new InputButtonStateHandler();
    public readonly InputButtonStateHandler Interact = new InputButtonStateHandler();
    public readonly InputButtonStateHandler BtnPrimary = new InputButtonStateHandler();
    public readonly InputButtonStateHandler BtnSecondary = new InputButtonStateHandler();
    public readonly InputVector1StateHandler PullTrigger = new InputVector1StateHandler();
    public readonly InputVector1StateHandler PullGrip = new InputVector1StateHandler();
    public readonly InputVector2StateHandler Stick = new InputVector2StateHandler();
    public readonly InputButtonStateHandler ClickStick = new InputButtonStateHandler();

    public readonly InputButtonStateHandler TchTrigger = new InputButtonStateHandler();
    public readonly InputButtonStateHandler TchGrab = new InputButtonStateHandler();
    public readonly InputButtonStateHandler TchPrimary = new InputButtonStateHandler();
    public readonly InputButtonStateHandler TchSecondary = new InputButtonStateHandler();
    public readonly InputButtonStateHandler TchStick = new InputButtonStateHandler();



    private readonly List<InputButtonStateHandler> m_ButtonHandlers;
    private readonly List<InputVector1StateHandler> m_Vector1Handlers;
    private readonly List<InputVector2StateHandler> m_Vector2Handlers;

    public InteractionSubscription()
    {
        m_ButtonHandlers = new List<InputButtonStateHandler>
        {
            Hover,
            Grab,
            BtnStart,
            Interact,
            BtnPrimary,
            BtnSecondary,
            ClickStick,

            TchTrigger,
            TchGrab,
            TchPrimary,
            TchSecondary,
            TchStick,
        };

        m_Vector1Handlers = new List<InputVector1StateHandler>
        {
            PullTrigger,
            PullGrip,
        };

        m_Vector2Handlers = new List<InputVector2StateHandler>
        {
            Stick,
        };
    }

    public InteractionSubscription Clone()
    {
        var subscription = new InteractionSubscription();
        for (int i = 0; i < m_ButtonHandlers.Count; ++i)
        {
            subscription.m_ButtonHandlers[i].Append(m_ButtonHandlers[i]);
        }
        for (int i = 0; i < m_Vector1Handlers.Count; ++i)
        {
            subscription.m_Vector1Handlers[i].Append(m_Vector1Handlers[i]);
        }
        for (int i = 0; i < m_Vector2Handlers.Count; ++i)
        {
            subscription.m_Vector2Handlers[i].Append(m_Vector2Handlers[i]);
        }
        return subscription;
    }

    public void Clear()
    {
        m_ButtonHandlers.ForEach(h => h.Clear());
        m_Vector1Handlers.ForEach(h => h.Clear());
        m_Vector2Handlers.ForEach(h => h.Clear());
    }
}

public delegate void InputButtonHandler(ControllerStateInteraction interaction, bool sendPhotonMessage = true);

public class InputButtonStateHandler
{
    public InputButtonHandler Begin = null;
    public InputButtonHandler Update = null;
    public InputButtonHandler End = null;
    public InputButtonStateHandler BeginHandlerPressed = null;

    public void Append(InputButtonStateHandler rhs)
    {
        Begin += rhs.Begin;
        Update += rhs.Update;
        End += rhs.End;
    }

    public void Clear()
    {
        Begin = null;
        Update = null;
        End = null;
    }
}


public delegate void InputVector1Handler(float amount, ControllerStateInteraction interaction, bool sendPhotonMessage = true);

public class InputVector1StateHandler
{
    public InputVector1Handler Begin = null;
    public InputVector1Handler Update = null;
    public InputVector1Handler End = null;
    public InputVector1StateHandler BeginHandlerPressed = null;

    public void Append(InputVector1StateHandler rhs)
    {
        Begin += rhs.Begin;
        Update += rhs.Update;
        End += rhs.End;
    }

    public void Clear()
    {
        Begin = null;
        Update = null;
        End = null;
    }
}

public delegate void InputVector2Handler(UnityEngine.Vector2 amount, ControllerStateInteraction interaction, bool sendPhotonMessage = true);

public class InputVector2StateHandler
{
    public InputVector2Handler Begin = null;
    public InputVector2Handler Update = null;
    public InputVector2Handler End = null;

    public InputVector2StateHandler BeginHandlerPressed = null;

    public void Append(InputVector2StateHandler rhs)
    {
        Begin += rhs.Begin;
        Update += rhs.Update;
        End += rhs.End;
    }

    public void Clear()
    {
        Begin = null;
        Update = null;
        End = null;
    }
}