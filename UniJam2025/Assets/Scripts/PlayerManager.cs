using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour
{
    [Header("Vie")]
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private int currentHealth;

    [Header("Animation / Sprite")]
    [SerializeField] private Animator animator;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clapClip;
    [SerializeField] private AudioClip hiFiveClip;
    [SerializeField] private AudioClip riseHandsClip;

    [Header("Réponse aux slides")]
    [SerializeField] private GameManager gameManager;

    [Header("Evénements")]
    public UnityEvent onDamaged;
    public UnityEvent onHealed;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        currentHealth = Mathf.Clamp(currentHealth > 0 ? currentHealth : maxHealth, 1, maxHealth);
    }

    private void OnEnable()
    {
        if (gameManager == null)
            gameManager = FindAnyObjectByType<GameManager>();

        if (gameManager != null)
        {
            gameManager.OnSlideFail?.AddListener(HandleSlideFail);
            gameManager.OnSlideSuccess?.AddListener(HandleSlideSuccess);
        }
        else
        {
            Debug.LogWarning("[PlayerManager] GameManager introuvable : les événements OnSlideSuccess/OnSlideFail ne seront pas reçus.");
        }
    }

    private void OnDisable()
    {
        if (gameManager != null)
        {
            gameManager.OnSlideFail?.RemoveListener(HandleSlideFail);
            gameManager.OnSlideSuccess?.RemoveListener(HandleSlideSuccess);
        }
    }

    public void OnClap()
    {
        PlayAction("Clap", clapClip);
    }

    public void OnHiFive()
    {
        PlayAction("HiFive", hiFiveClip);
    }

    public void OnRiseHands()
    {
        PlayAction("RiseHands", riseHandsClip);
    }

    private void PlayAction(string animatorTrigger, AudioClip clip)
    {
        if (animator != null)
        {
            animator.ResetTrigger(animatorTrigger);
            animator.SetTrigger(animatorTrigger);
        }
        else
        {
            Debug.LogWarning($"[PlayerManager] Animator manquant : impossible de jouer l'animation '{animatorTrigger}'.");
        }

        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    public void ApplyDamage()
    {
        currentHealth -= 1;
        currentHealth = Mathf.Max(0, currentHealth);
        Debug.Log($"[PlayerManager] Dégâts: 1 -> HP = {currentHealth}/{maxHealth}");
        onDamaged?.Invoke();

        if (currentHealth == 0)
            Die();
    }

    public void ApplyHeal()
    {
        int prev = currentHealth;
        currentHealth += 1;
        currentHealth = Mathf.Min(maxHealth, currentHealth);
        Debug.Log($"[PlayerManager] Soin: 1 -> HP = {currentHealth}/{maxHealth} (was {prev})");
        onHealed?.Invoke();
    }

    private void Die()
    {
        Debug.Log("[PlayerManager] Mort détectée.");
        GameObject.FindAnyObjectByType<ChangeScene>()?.Goto("Main Menu");

        gameObject.SetActive(false);
    }

    private void HandleSlideFail(bool value)
    {
        if(value)
            ApplyDamage();
    }

    private void HandleSlideSuccess(Rule.ActionType type)
    {
        ApplyHeal();
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
}