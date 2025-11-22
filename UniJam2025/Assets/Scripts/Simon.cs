using UnityEngine;

public class Simon : MonoBehaviour
{
    private enum SimonState { Idle, Walking, Posing }

    [Header("Idle settings")]
    [SerializeField] private float minIdleDuration = 1f;
    [SerializeField] private float maxIdleDuration = 3f;
    [SerializeField] private float idleChance = 0.3f;

    [Header("Walk settings")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private Transform leftBoundary;
    [SerializeField] private Transform rightBoundary;

    [Header("Pose settings")]
    [SerializeField] private bool canPoseWhileWalking = true;  // active les poses pendant la marche
    [SerializeField] private float poseChance = 0.1f;          // chance de poser
    [SerializeField] private float poseDuration = 3f;          // durée de pose

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private SimonState state = SimonState.Walking;
    private float idleTimer;
    private float poseTimer;
    private int direction = 1;
    private Color normalColor;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        normalColor = sr.color;

        PickRandomDirection();
    }

    private void Update()
    {
        switch (state)
        {
            case SimonState.Walking:
                HandleWalking();
                break;

            case SimonState.Idle:
                HandleIdle();
                break;

            case SimonState.Posing:
                HandlePosing();
                break;
        }
    }

    private void HandleWalking()
    {
        rb.linearVelocity = new Vector2(direction * speed, 0);

        // Boundaries
        if (transform.position.x > rightBoundary.position.x && direction == 1)
            direction = -1;
        else if (transform.position.x < leftBoundary.position.x && direction == -1)
            direction = 1;

        // Pose possible pendant la marche ?
        if (canPoseWhileWalking && Random.value < Time.deltaTime * poseChance)
        {
            StartPosing();
            return;
        }

        // Possibilité de s'arrêter (idle)
        if (Random.value < Time.deltaTime * idleChance)
        {
            state = SimonState.Idle;
            idleTimer = Random.Range(minIdleDuration, maxIdleDuration);

            // 🔥 Si pose interdite pendant la marche, on peut poser seulement ici !
            if (!canPoseWhileWalking && Random.value < poseChance)
            {
                StartPosing();
            }

            rb.linearVelocity = Vector2.zero;
        }
    }

    private void HandleIdle()
    {
        idleTimer -= Time.deltaTime;
        if (idleTimer <= 0)
        {
            if (Random.value > 0.5f)
                PickRandomDirection();

            state = SimonState.Walking;
        }
    }

    private void HandlePosing()
    {
        poseTimer -= Time.deltaTime;
        if (poseTimer <= 0)
        {
            // Retour en couleur normale
            sr.color = normalColor;

            // Retour à la marche
            state = SimonState.Walking;
        }
    }

    private void StartPosing()
    {
        state = SimonState.Posing;

        poseTimer = poseDuration;

        rb.linearVelocity = Vector2.zero;

        // Simon devient rouge
        sr.color = Color.red;
    }

    private void PickRandomDirection()
    {
        direction = Random.value > 0.5f ? 1 : -1;
    }
}
