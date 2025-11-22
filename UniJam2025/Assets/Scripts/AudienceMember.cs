using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class AudienceMember : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameManager gameManager;

    [Header("Triggers d'animations")]
    [SerializeField] private string clapSuccessTrigger = "Clap";
    [SerializeField] private string hiFiveSuccessTrigger = "HiFive";
    [SerializeField] private string riseHandsSuccessTrigger = "RiseHands";

    [SerializeField] private string failBoolParameter = "Fail";

    private UnityAction<Rule.ActionType> _successListener;
    private UnityAction<bool> _failListener;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if (gameManager == null)
            gameManager = FindAnyObjectByType<GameManager>();

        if (gameManager == null)
        {
            Debug.LogWarning("[AudienceManager] GameManager introuvable : impossible de s'abonner aux événements.");
            return;
        }

        // Préparer listeners pour pouvoir les retirer proprement
        _successListener = new UnityAction<Rule.ActionType>(HandleSlideSuccess);
        _failListener = new UnityAction<bool>(HandleSlideFail);

        gameManager.OnSlideSuccess?.AddListener(_successListener);
        gameManager.OnSlideFail?.AddListener(_failListener);
    }

    private void OnDisable()
    {
        if (gameManager == null) return;

        if (_successListener != null)
            gameManager.OnSlideSuccess?.RemoveListener(_successListener);
        if (_failListener != null)
            gameManager.OnSlideFail?.RemoveListener(_failListener);
    }

    private void HandleSlideSuccess(Rule.ActionType action)
    {
        if (animator == null)
        {
            Debug.LogWarning("[AudienceManager] Animator non défini — impossible de jouer l'animation de succès.");
            return;
        }

        switch (action)
        {
            case Rule.ActionType.Clap:
                animator.SetTrigger(clapSuccessTrigger);
                break;
            case Rule.ActionType.HighFive:
                animator.SetTrigger(hiFiveSuccessTrigger);
                break;
            case Rule.ActionType.RaiseHands:
                animator.SetTrigger(riseHandsSuccessTrigger);
                break;
            default:
                Debug.LogWarning("[AudienceManager] Action inconnue reçue pour success: " + action);
                break;
        }
    }

    private void HandleSlideFail(bool isAnimActive)
    {
        if (animator == null)
        {
            Debug.LogWarning("[AudienceManager] Animator non défini — impossible de jouer l'animation d'échec.");
            return;
        }

        animator.SetBool(failBoolParameter, isAnimActive);
    }
}
