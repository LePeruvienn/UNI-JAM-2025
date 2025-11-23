using System.Collections;
using System.Collections.Generic;
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

    // audio management
    private Coroutine _audioRestoreRoutine;
    private List<AudioSource> _pausedSources = new List<AudioSource>();

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

        // restore any paused audio if we are destroyed/disabled while holding them
        if (_audioRestoreRoutine != null)
        {
            StopCoroutine(_audioRestoreRoutine);
            _audioRestoreRoutine = null;
        }
        RestorePausedSources();
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

        // If a previous restore coroutine is running, stop it and restore before starting new pause
        if (_audioRestoreRoutine != null)
        {
            StopCoroutine(_audioRestoreRoutine);
            _audioRestoreRoutine = null;
            RestorePausedSources();
        }

        // Pause (not Stop) other AudioSources so we can resume them later
        _pausedSources.Clear();
        var allSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (var src in allSources)
        {
            if (src == audioSource) continue;
            if (src.isPlaying)
            {
                try
                {
                    src.Pause();
                    _pausedSources.Add(src);
                }
                catch
                {
                    // ignore sources that cannot be paused for any reason
                }
            }
        }

        // Play requested clip
        audioSource.PlayOneShot(clip);

        // schedule resume of paused sources after clip duration
        _audioRestoreRoutine = StartCoroutine(RestorePausedAfterDelay(clip.length));
    }

    private IEnumerator RestorePausedAfterDelay(float delay)
    {
        // wait clip length (if 0 or negative, restore immediately)
        if (delay > 0f)
            yield return new WaitForSeconds(delay);
        RestorePausedSources();
        _audioRestoreRoutine = null;
    }

    private void RestorePausedSources()
    {
        if (_pausedSources == null || _pausedSources.Count == 0) return;

        foreach (var src in _pausedSources)
        {
            if (src == null) continue;
            try
            {
                src.UnPause();
            }
            catch
            {
                // fallback to Play if UnPause fails
                try { src.Play(); } catch { }
            }
        }
        _pausedSources.Clear();
    }
}
