using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public InputManager()
    {
        Instance = this;
    }
    [SerializeField] InputActionReference move, aim, reload;
    [SerializeField] UtilityButton useButton;
    [SerializeField] KeyCode reloadKey, inventoryKey, m_interactKey;
    public KeyCode interactKey => m_interactKey;
    public static Vector2 MoveInput() => Instance.move.action.ReadValue<Vector2>();
    public static Vector2 AimInput(Vector2 origin)
    {
        if(DeviceManager.IsMobile())
        {
            return Instance.aim.action.ReadValue<Vector2>();
        }
        else
        {
            return (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - origin;
        }
    }
    public static bool UseButton()
    {
        if (UIManager.Instance.isTabOpen) return false;
        if(DeviceManager.IsMobile())
        {
            return Instance.useButton.IsButton();
        }
        else
        {
            return Input.GetMouseButton(0);
        }
    }
    public static bool UseButtonDown()
    {
        if (UIManager.Instance.isTabOpen) return false;
        if (DeviceManager.IsMobile())
        {
            return Instance.useButton.IsButtonUp();
        }
        else
        {
            return Input.GetMouseButtonDown(0);
        }
    }
    public static bool ReloadButtonDown()
    {
        if (UIManager.Instance.isTabOpen) return false;
        return Input.GetKeyDown(Instance.reloadKey);
    }
    public static bool InventoryButtonDown()
    {
        return Input.GetKeyDown(Instance.inventoryKey);
    }
    public static bool InteractButtonDown()
    {
        return Input.GetKeyDown(Instance.interactKey);
    }
}