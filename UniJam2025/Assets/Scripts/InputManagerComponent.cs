using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputManagerComponent : MonoBehaviour
{
    public UnityEvent onClap;
    public UnityEvent onHiFive;
    public UnityEvent onRiseHands;

    private InputManager inputManager;

    // handlers stockés pour pouvoir se désabonner proprement
    private System.Action<InputAction.CallbackContext> _clapHandler;
    private System.Action<InputAction.CallbackContext> _hiFiveHandler;
    private System.Action<InputAction.CallbackContext> _riseHandsHandler;

    void Awake()
    {
        if (onClap == null) onClap = new UnityEvent();
        if (onHiFive == null) onHiFive = new UnityEvent();
        if (onRiseHands == null) onRiseHands = new UnityEvent();

        inputManager = new InputManager();
        inputManager.player.Enable();

        _clapHandler = ctx => onClap.Invoke();
        _hiFiveHandler = ctx => onHiFive.Invoke();
        _riseHandsHandler = ctx => onRiseHands.Invoke();

        inputManager.player.Clap.performed += _clapHandler;
        inputManager.player.HiFive.performed += _hiFiveHandler;
        inputManager.player.RiseHands.performed += _riseHandsHandler;
    }

    void OnDestroy()
    {
        if (inputManager != null)
        {
            inputManager.player.Clap.performed -= _clapHandler;
            inputManager.player.HiFive.performed -= _hiFiveHandler;
            inputManager.player.RiseHands.performed -= _riseHandsHandler;

            inputManager.Dispose();
            inputManager = null;
        }
    }

    // Méthodes publiques à lier dans Button.OnClick() depuis l'éditeur
    public void OnClapButtonClicked()
    {
        onClap.Invoke();
    }

    public void OnHiFiveButtonClicked()
    {
        onHiFive.Invoke();
    }

    public void OnRiseHandsButtonClicked()
    {
        onRiseHands.Invoke();
    }
}