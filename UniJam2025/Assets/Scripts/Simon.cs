using UnityEngine;

public enum SimonState {

    Walking,
    Idle,
    Baiting,
    Posing
}

public class Simon : MonoBehaviour
{

    [Header("LOCK SYSTEM")]
    [SerializeField] private bool isLocked = false;

    [Header("=== TEMPO ===")]
    [SerializeField] private float tempoMin = 1f;
    [SerializeField] private float tempoMax = 2f;

    [Header("=== CHANCES ===")]
    [SerializeField, Range(0f, 1f)] private float idleChance = 0.4f;
    [SerializeField, Range(0f, 1f)] private float baitingChance = 0.2f;
    [SerializeField, Range(0f, 1f)] private float poseChance = 0.1f;
    [SerializeField, Range(0f, 1f)] private float postIdlePoseChance = 0.3f;

    [Header("=== POSE SETTINGS ===")]
    [SerializeField] private float poseDurationMin = 1.5f;
    [SerializeField] private float poseDurationMax = 3f;
    [SerializeField] private float poseMinCooldown = 2f;  // Minimum wait before next pose
    [SerializeField] private float poseMaxCooldown = 6f;  // Maximum wait before forcing next pose
    [SerializeField] private float poseMaxDuration = 4f;  // Maximum time a single pose can last

    [Header("=== BAITING SETTINGS ===")]
    [SerializeField] private float baitingDurationMin = 0.5f;
    [SerializeField] private float baitingDurationMax = 1.0f;

    private float baitingDuration;

    [Header("=== MOVEMENT ===")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private Transform leftBoundary;
    [SerializeField] private Transform rightBoundary;

    private Rigidbody2D rb;
    private SpriteRenderer sr; 
    private Color baseColor;

    private SimonState state;
    private float tempoTimer;
    private float currentTempo;
    private float stateTimer;

    private float poseDuration;
    private float lastPoseTime = -999f;
    private int direction = 1;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        baseColor = sr.color;

        PickRandomDirection();
        SetNewTempo();
        ChangeState(SimonState.Walking);
    }

    private void Update()
    {
        if (isLocked) {

            if (state == SimonState.Walking) {
                HandleBoundaries();
                rb.linearVelocity = new Vector2(direction * speed, 0);
            }

            return;
        }

        tempoTimer += Time.deltaTime;
        stateTimer += Time.deltaTime;

        switch (state)
        {
            case SimonState.Walking: UpdateWalking(); break;
            case SimonState.Idle: UpdateIdle(); break;
            case SimonState.Baiting: UpdateBaiting(); break;
            case SimonState.Posing: UpdatePose(); break;
        }

        if (tempoTimer >= currentTempo)
        {
            tempoTimer = 0f;
            SetNewTempo();
            EvaluateNextState();
        }
    }

    // ------------------------------
    // TEMPO
    // ------------------------------
    private void SetNewTempo()
    {
        currentTempo = Random.Range(tempoMin, tempoMax);
    }

    private void EvaluateNextState()
    {
        // On n'évalue pas de nouvelle transition si on est déjà dans un état "bloquant"
        if (state == SimonState.Posing || state == SimonState.Idle || state == SimonState.Baiting) 
            return;

        float timeSinceLastPose = Time.time - lastPoseTime;

        // 1. Check Posing (Force ou Chance)
        if (timeSinceLastPose >= poseMaxCooldown || 
            (timeSinceLastPose >= poseMinCooldown && Random.value < poseChance))
        {
            StartPose();
            return;
        }

        // 2. Check Baiting (Nouvelle vérification)
        if (Random.value < baitingChance)
        {
            ChangeState(SimonState.Baiting);
            return;
        }

        // 3. Check Idle
        if (Random.value < idleChance)
        {
            ChangeState(SimonState.Idle);
            return;
        }

        // 4. Continue walking
        ChangeState(SimonState.Walking);
    }

    // ------------------------------
    // STATES
    // ------------------------------
    private void ChangeState(SimonState newState)
    {
        state = newState;
        stateTimer = 0f;

        switch (newState)
        {
            case SimonState.Walking:
                sr.color = baseColor;
                break;

            case SimonState.Idle:
                rb.linearVelocity = Vector2.zero;
                sr.color = baseColor;
                break;

            case SimonState.Baiting:
                rb.linearVelocity = Vector2.zero;
                sr.color = Color.yellow; // Visualisation de l'état Baiting (peut être ajusté)
                baitingDuration = Random.Range(baitingDurationMin, baitingDurationMax);
                break;

            case SimonState.Posing:
                rb.linearVelocity = Vector2.zero;
                sr.color = Color.red;
                poseDuration = Random.Range(poseDurationMin, poseDurationMax);
                lastPoseTime = Time.time;
                break;
        }
    }

    private void UpdateWalking()
    {
        HandleBoundaries();
        rb.linearVelocity = new Vector2(direction * speed, 0);
    }

    private void UpdateIdle()
    {
        if (stateTimer >= currentTempo)
        {
            float timeSinceLastPose = Time.time - lastPoseTime;

            // Post-idle pose check
            if (timeSinceLastPose >= poseMaxCooldown || 
                (timeSinceLastPose >= poseMinCooldown && Random.value < postIdlePoseChance))
            {
                StartPose();
                return;
            }

            PickRandomDirection();
            ChangeState(SimonState.Walking);
        }
    }

    private void UpdateBaiting()
    {
        if (stateTimer >= baitingDuration)
        {
            PickRandomDirection();
            ChangeState(SimonState.Walking);
        }
    }

    private void StartPose()
    {
        ChangeState(SimonState.Posing);
    }

    private void UpdatePose()
    {
        // Enforce max pose duration
        if (stateTimer >= Mathf.Min(poseDuration, poseMaxDuration))
        {
            PickRandomDirection();
            ChangeState(SimonState.Walking);
        }
    }

    // ------------------------------
    // BOUNDARIES
    // ------------------------------
    private void HandleBoundaries()
    {
        if (transform.position.x > rightBoundary.position.x) direction = -1;
        if (transform.position.x < leftBoundary.position.x) direction = 1;
    }

    private void PickRandomDirection()
    {
        direction = Random.value > 0.5f ? 1 : -1;
    }

    public SimonState GetSimonState()
    {
        return state;
    }

    public void SetLocked(bool val)
    {
        isLocked = val;
    }
}
