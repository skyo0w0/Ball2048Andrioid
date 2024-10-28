
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class PointerInputManager : MonoBehaviour
{
    /// <summary>
    /// Event fired when the user presses on the screen.
    /// </summary>
    public event Action<PointerInput, double> Pressed;

    /// <summary>
    /// Event fired as the user drags along the screen.
    /// </summary>
    public event Action<PointerInput, double> Dragged;

    /// <summary>
    /// Event fired when the user releases a press.
    /// </summary>
    public event Action<PointerInput, double> Released;

    private bool m_Dragging;
    private TouchInput m_Controls;

    // These are useful for debugging, especially when touch simulation is on.
    [SerializeField] private bool m_UseMouse;
    [SerializeField] private bool m_UsePen;
    [SerializeField] private bool m_UseTouch;




    protected virtual void Awake()
    {
        m_Controls = new TouchInput();

        m_Controls.pointer.point.performed += OnAction;
        // The action isn't likely to actually cancel as we've bound it to all kinds of inputs but we still
        // hook this up so in case the entire thing resets, we do get a call.
        m_Controls.pointer.point.canceled += OnAction;

        SyncBindingMask();
    }

    protected virtual void OnEnable()
    {
        m_Controls?.Enable();
    }

    protected virtual void OnDisable()
    {
        m_Controls?.Disable();
    }

    protected void OnAction(InputAction.CallbackContext context)
    {
        var control = context.control;
        var device = control.device;

        var isMouseInput = device is Mouse;
        var isPenInput = !isMouseInput && device is Pen;

        // Read our current pointer values.
        var drag = context.ReadValue<PointerInput>();
        if (isMouseInput)
            drag.InputId = Helpers.LeftMouseInputId;
        else if (isPenInput)
            drag.InputId = Helpers.PenInputId;

        if (drag.Contact && !m_Dragging)
        {
            Pressed?.Invoke(drag, context.time);
            m_Dragging = true;
        }
        else if (drag.Contact && m_Dragging)
        {
            Dragged?.Invoke(drag, context.time);
        }
        else
        {
            Released?.Invoke(drag, context.time);
            m_Dragging = false;
        }
    }

    private void SyncBindingMask()
    {
        if (m_Controls == null)
            return;

        if (m_UseMouse && m_UsePen && m_UseTouch)
        {
            m_Controls.bindingMask = null;
            return;
        }

        m_Controls.bindingMask = InputBinding.MaskByGroups(new[]
        {
                m_UseMouse ? "Mouse" : null,
                m_UsePen ? "Pen" : null,
                m_UseTouch ? "Touch" : null
            });
    }

    private void OnValidate()
    {
        SyncBindingMask();
    }
}
