using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;
    private InputControls _controls;
    public InputControls controls
    {
        get
        {
            if (_controls == null)
            {
                InitializeControls();
            }
            return _controls;
        }
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            if (_controls == null)
            {
                InitializeControls();
            }
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        
    }

    private void InitializeControls()
    {
        _controls = new InputControls();
        _controls.Player.Interact.performed += interaction => Interacting();
        _controls.Enable();
    }

    void OnDestroy()
    {
        if (instance == this && _controls != null)
        {
            _controls.Player.Interact.performed -= interaction => Interacting();
            _controls.Disable();
        }
    }

    public void Interacting()
    {
        Debug.Log("Not implemented yet Interact.");
    }
}
