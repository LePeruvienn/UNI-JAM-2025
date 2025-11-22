using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour
{
    [Header("Vie")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [Header("Animation / Sprite")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashDuration = 0.08f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clapClip;
    [SerializeField] private AudioClip hiFiveClip;
    [SerializeField] private AudioClip riseHandsClip;

    [Header("Réponse aux slides")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private int slideFailDamage = 10;
    [SerializeField] private int slideSuccessHeal = 10;

    [Header("Evénements")]
    public UnityEvent onDamaged;
    public UnityEvent onHealed;
    public UnityEvent onDead;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        currentHealth = Mathf.Clamp(currentHealth > 0 ? currentHealth : maxHealth, 1, maxHealth);
    }

    private void OnEnable()
    {
        // résout la référence au GameManager si nécessaire
        if (gameManager == null)
            gameManager = FindAnyObjectByType<GameManager>();

        if (gameManager != null)
        {
            // GameManager expose des UnityEvent : utiliser AddListener / RemoveListener
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
        else if (spriteRenderer != null)
        {
            StartCoroutine(FlashSprite());
        }

        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    private IEnumerator FlashSprite()
    {
        if (spriteRenderer == null) yield break;
        var original = spriteRenderer.color;
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = original;
    }

    public void ApplyDamage(int amount)
    {
        if (amount <= 0) return;
        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);
        Debug.Log($"[PlayerManager] Dégâts: {amount} -> HP = {currentHealth}/{maxHealth}");
        onDamaged?.Invoke();

        if (currentHealth == 0)
            Die();
    }

    public void ApplyHeal(int amount)
    {
        if (amount <= 0) return;
        int prev = currentHealth;
        currentHealth += amount;
        currentHealth = Mathf.Min(maxHealth, currentHealth);
        Debug.Log($"[PlayerManager] Soin: {amount} -> HP = {currentHealth}/{maxHealth} (was {prev})");
        onHealed?.Invoke();
    }

    private void Die()
    {
        Debug.Log("[PlayerManager] Mort détectée.");
        onDead?.Invoke();

        gameObject.SetActive(false);
    }

    private void HandleSlideFail()
    {
        ApplyDamage(slideFailDamage);
    }

    private void HandleSlideSuccess(Rule.ActionType type)
    {
        ApplyHeal(slideSuccessHeal);
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
}