using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Audience : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameManager gameManager;

    [Header("Triggers d'animations")]
    [SerializeField] private string clapSuccessTrigger = "Clap";
    [SerializeField] private string hiFiveSuccessTrigger = "HiFive";
    [SerializeField] private string riseHandsSuccessTrigger = "RiseHands";

    [SerializeField] private string failBoolParameter = "Fail";

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip failClip;
    [SerializeField] private AudioClip clapClip;
    [SerializeField] private AudioClip hiFiveClip;
    [SerializeField] private AudioClip riseHandsClip;

    private UnityAction<Rule.ActionType> _successListener;
    private UnityAction<bool> _failListener;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
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
        if (animator != null)
        {
            switch (action)
            {
                case Rule.ActionType.Clap:
                    animator.SetTrigger(clapSuccessTrigger);
                    PlayClip(clapClip);
                    break;
                case Rule.ActionType.HighFive:
                    animator.SetTrigger(hiFiveSuccessTrigger);
                    PlayClip(hiFiveClip);
                    break;
                case Rule.ActionType.RaiseHands:
                    animator.SetTrigger(riseHandsSuccessTrigger);
                    PlayClip(riseHandsClip);
                    break;
                default:
                    Debug.LogWarning("[AudienceManager] Action inconnue reçue pour success: " + action);
                    break;
            }
            return;
        }
        else
        {
            Debug.LogWarning("[AudienceManager] Animator manquant — impossible d'afficher l'animation de succès.");
            switch (action)
            {
                case Rule.ActionType.Clap:
                    PlayClip(clapClip);
                    break;
                case Rule.ActionType.HighFive:
                    PlayClip(hiFiveClip);
                    break;
                case Rule.ActionType.RaiseHands:
                    PlayClip(riseHandsClip);
                    break;
            }
        }
    }

    private void HandleSlideFail(bool isAnimActive)
    {
        if (animator != null)
        {
            animator.SetBool(failBoolParameter, isAnimActive);
            if (isAnimActive)
                PlayClip(failClip);
            return;
        }

        if (isAnimActive)
            PlayClip(failClip);
    }

    private void PlayClip(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;
        audioSource.PlayOneShot(clip);
    }
}
