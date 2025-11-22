using Assets;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Windows;

public class GameManager : MonoBehaviour
{
    public UnityEvent<Rule.ActionType> OnSlideSuccess;
    public UnityEvent OnSlideFail;


    private List<Rule.ActionType> requiredActions = new List<Rule.ActionType>();
    private List<Rule.ActionType> pendingActions = new List<Rule.ActionType>();

    public List<Rule> activeRules = new List<Rule>();


    public SlideManager slideManager;
    public Simon simon;

    private int slideCount = 0;

    private float slideTimer = 0f;
    private bool timerRunning = false;

    public float timeLimit = 5f;


    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        // If an instance already exists and it’s not this, destroy this object
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    } 

    public void SetRules(List<Rule> rules)
    {
        requiredActions = BuildRequiredActions(rules);
        pendingActions = new List<Rule.ActionType>(requiredActions);

        Debug.Log("[GameManager] New slide rules received.");

        foreach (var action in requiredActions)
        {
            Debug.Log("Required: " + action);
        }

        StartSlideTimer();
    }


    //create list of rules
    private List<Rule.ActionType> BuildRequiredActions(List<Rule> rules)
    {
        List<Rule.ActionType> actions = new List<Rule.ActionType>();

        foreach (var rule in rules)
        {
            for (int i = 0; i < rule.actionAmount; i++)
            {
                actions.Add(rule.actionType);
            }
        }

        return actions;
    }


    // Called by InputMgr whenever the player presses one of the action buttons
    public void OnPlayerInput(Rule.ActionType input)
    {
        Debug.Log("[GameManager] Player input: " + input);

        if (pendingActions.Contains(input) && simon.GetSimonState().Equals("Posing")) //add simon active check
        {
            pendingActions.Remove(input);
            Debug.Log("Correct input: " + input);

            if (pendingActions.Count == 0)
            {
                SlideCompletedSuccessfully(input);
            }
        }
        else
        {
            Debug.Log("Wrong input: " + input);

            OnSlideFail?.Invoke();

            //add coroutine for failure event
        }
    }

    private void SlideCompletedSuccessfully(Rule.ActionType input)
    {

        timerRunning = false;

        Debug.Log("Slide successfully completed.");

        OnSlideSuccess?.Invoke(input);

        LoadNextSlide();
    }

    private void LoadNextSlide()
    {
        slideCount++;
        Debug.Log("[GameManager] Loading next slide...");

        // Every 5 slides -> introduce a new rule
        if (slideCount % 5 == 1)
        {
            // Generate a new rule (assuming you keep a list called activeRules)
            Rule newRule = RuleGenerator.GenerateRandomRule(activeRules);
            activeRules.Add(newRule);

            Debug.Log("[GameManager] New rule generated: " + newRule.description);

            // Tell SlideManager to generate a special "rule slide"
            slideManager.GenerateRuleSlide(newRule.description);
        }
        else
        {
            // Regular gameplay slide
            slideManager.GenerateSlide();
        }
    }


    private void StartSlideTimer()
    {
        if (!timerRunning)
        {
            slideTimer = timeLimit;
            timerRunning = true;
        }
        Debug.Log("[GameManager] Timer started: " + slideTimer + " seconds");
    }

    private void SlideFailedDueToTimeout()
    {
        Debug.Log("Slide failed: Time expired.");

        if (simon.GetSimonState().Equals("Posing")) {

            OnSlideFail?.Invoke();

            //add coroutine for failure event
        }


        LoadNextSlide();
    }

    private void Update()
    {
        if (!timerRunning)
            return;

        slideTimer -= Time.deltaTime;

        if (slideTimer <= 0f)
        {
            timerRunning = false;
            SlideFailedDueToTimeout();
        }
    }

    public void OnClap()
    {
        Debug.Log("[TestCallback] Reçu : Clap (UI ou clavier). Time=" + Time.time);
        OnPlayerInput(Rule.ActionType.Clap);
    }

    public void OnHiFive()
    {
        Debug.Log("[TestCallback] Reçu : HiFive (UI ou clavier). Time=" + Time.time);
        OnPlayerInput(Rule.ActionType.HighFive);
    }

    public void OnRiseHands()
    {
        Debug.Log("[TestCallback] Reçu : RiseHands (UI ou clavier). Time=" + Time.time);
        OnPlayerInput(Rule.ActionType.RaiseHands);
    }



    public void LoadTestActions()
    {
        // Clear any previous actions
        requiredActions.Clear();
        pendingActions.Clear();

        // Example test list
        List<Rule.ActionType> testActions = new List<Rule.ActionType>()
    {
        Rule.ActionType.Clap,
        Rule.ActionType.HighFive,
        Rule.ActionType.Clap
    };

        // Fill the required and pending lists
        requiredActions = new List<Rule.ActionType>(testActions);
        pendingActions = new List<Rule.ActionType>(testActions);

        // Start the timer
        StartSlideTimer();

        // Debug log
        Debug.Log("[Test] Loaded test actions:");
        foreach (var action in testActions)
        {
            Debug.Log("- " + action);
        }
    }


}
