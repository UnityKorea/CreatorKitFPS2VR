using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;


[System.Serializable]
public enum HandHeldType
{
    Left,
    Right
}

public enum ButtonType
{
    Primary,
    Secondary,
    Trigger,
    Grip,
    Menu
}

[System.Serializable]
public class ButtonData
{
    public ButtonType type;
    public GameObject button;
    public Vector3 buttonDirection;
    [System.NonSerialized]
    public Vector3 buttonOriginPosition;
}

public class ControllerButtonVisualReaction : MonoBehaviour
{
    [SerializeField]
    HandHeldType handHeldType;

    // There is no possibility of having multiple joysticks on the controller.
    [SerializeField] GameObject joystick;
    [SerializeField] Vector2 joystickAngle;
    // One controller has multiple buttons
    [SerializeField] ButtonData [] buttons;

    private List<InputDevice> devices;
   

    private void Awake()
    {
        devices = new List<InputDevice>();
    }

    private void Start()
    {
        StoreOriginButtonPosition();
    }

    void OnEnable()
    {
        List<InputDevice> allDevices = new List<InputDevice>();
        InputDevices.GetDevices(allDevices);
        foreach (InputDevice device in allDevices)
            InputDevices_deviceConnected(device);

        InputDevices.deviceConnected += InputDevices_deviceConnected;
        InputDevices.deviceDisconnected += InputDevices_deviceDisconnected;
    }

    private void OnDisable()
    {
        InputDevices.deviceConnected -= InputDevices_deviceConnected;
        InputDevices.deviceDisconnected -= InputDevices_deviceDisconnected;
        devices.Clear();
    }

    private void InputDevices_deviceConnected(InputDevice device)
    { 
        devices.Add(device);
        Debug.Log("device added : " + device.name);   
    }

    private void InputDevices_deviceDisconnected(InputDevice device)
    {
        if (devices.Contains(device))
            devices.Remove(device);
    }

    Vector2 GetStickState(InputDevice device, InputFeatureUsage<Vector2> usage)
    {
        Vector2 buttonState;
        device.TryGetFeatureValue(usage, out buttonState); // did get a value
        return buttonState;
    }


    bool GetButtonState(InputDevice device, InputFeatureUsage<bool> usage)
    {
        bool buttonState = false;
        device.TryGetFeatureValue(usage, out buttonState);
        return buttonState;
    }


    void StoreOriginButtonPosition()
    {
        foreach( ButtonData btn in buttons)
        {
            if (btn.button)
                btn.buttonOriginPosition = btn.button.transform.localPosition;
        }
    }

    
    public InputFeatureUsage<bool> GetButtonUsages(ButtonType type) => type switch
    {
        ButtonType.Grip => CommonUsages.gripButton,
        ButtonType.Menu => CommonUsages.menuButton,
        ButtonType.Primary => CommonUsages.primaryButton,
        ButtonType.Secondary => CommonUsages.secondaryButton,
        ButtonType.Trigger => CommonUsages.triggerButton,
        _ => throw new ArgumentOutOfRangeException(nameof(type), $"Not expected direction value: {type}"),
    };
    

    void UpdateButtonState(InputDevice device, ButtonData btn)
    {
        InputFeatureUsage<bool> usage = GetButtonUsages(btn.type);
        bool flag = GetButtonState(device, usage);
        //Debug.Log(btn.type + " Button(" + usage + ") : " + flag);

        if (btn.button)
        {
            if (flag)
                btn.button.transform.localPosition = btn.buttonOriginPosition + btn.buttonDirection;
            else
                btn.button.transform.localPosition = btn.buttonOriginPosition;
        }
    }

    void UpdateJoystickState(InputDevice device)
    {
        Vector2 rot = GetStickState(device, CommonUsages.primary2DAxis);
        //Debug.Log("Joystick : " + rot);

        if(joystick != null)
            joystick.transform.localRotation = Quaternion.Euler(
                rot.y* joystickAngle.x,
                joystick.transform.localRotation.y * Mathf.Rad2Deg, 
                rot.x*joystickAngle.y);
    }

    void Update()
    {
        foreach (var device in devices)
        {
            InputDeviceCharacteristics desiredCharacteristics;
            if (handHeldType == HandHeldType.Left)
                desiredCharacteristics = UnityEngine.XR.InputDeviceCharacteristics.HeldInHand | UnityEngine.XR.InputDeviceCharacteristics.Left | UnityEngine.XR.InputDeviceCharacteristics.Controller;
            else
                desiredCharacteristics = UnityEngine.XR.InputDeviceCharacteristics.HeldInHand | UnityEngine.XR.InputDeviceCharacteristics.Right | UnityEngine.XR.InputDeviceCharacteristics.Controller;

            if ((device.characteristics & desiredCharacteristics) == desiredCharacteristics)
            {
                foreach (var btn in buttons)
                {
                    UpdateJoystickState(device);
                    UpdateButtonState(device, btn);
                }
            }
         
        }
           
    }
}