using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Windows;
using static Unity.Collections.Unicode;

public class GameManager : MonoBehaviour
{
    public UnityEvent<Rule.ActionType> OnSlideSuccess;
    public UnityEvent<bool> OnSlideFail;

    public UnityEvent OnSlideFailEnd;


    private List<Rule.ActionType> requiredActions = new List<Rule.ActionType>();
    private List<Rule.ActionType> pendingActions = new List<Rule.ActionType>();

    public List<Rule> presentedRules = new List<Rule>();
    public List<Rule> activeRules = new List<Rule>();


    public SlideManager slideManager;
    public Simon simon;

    private int slideCount = 0;

    private float slideTimer = 0f;
    private bool timerRunning = false;

    public int showRuleSlideEveryXSlide = 5;
    public float simonPresentProbability = 0.5f;
    public float timeLimit = 5f;
    public float failTime = 4f;
    public float successfulSlideDelay = 4f;
    public float ruleSlideDelay = 7f;


    private Coroutine failRoutine;

    public static GameManager Instance { get; private set; }

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip failClip;
    [SerializeField] private AudioClip clapAudienceClip;
    [SerializeField] private AudioClip hiFiveAudienceClip;
    [SerializeField] private AudioClip riseHandsAudienceClip;
    [SerializeField] private AudioClip changeSlideClip;

    private void Awake()
    {
        // If an instance already exists and it�s not this, destroy this object
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        //DontDestroyOnLoad(gameObject);

        LoadNextSlide();
    } 

    public void SetRules(List<Rule> rules)
    {
        requiredActions = BuildRequiredActions(rules);
        pendingActions = new List<Rule.ActionType>(requiredActions);

        Debug.Log("[GameManager] Active rules set.");

        /*foreach (var action in requiredActions)
        {
            Debug.Log("Required: " + action);
        }*/
    }


    //create list of rules
    private List<Rule.ActionType> BuildRequiredActions(List<Rule> rules)
    {
        List<Rule.ActionType> actions = new List<Rule.ActionType>();

        foreach (var rule in rules)
        {
           // Debug.Log($"Rule: {rule.actionType}, amount: {rule.actionAmount}");
            for (int i = 0; i < rule.actionAmount; i++)
            {
                actions.Add(rule.actionType);
            }
        }

        Debug.Log("Built actions list: " + string.Join(", ", actions));
        return actions;
    }



    // Called by InputMgr whenever the player presses one of the action buttons
    public void OnPlayerInput(Rule.ActionType input)
    {
       //Debug.Log("[GameManager] Player input: " + input);

        if (pendingActions.Contains(input) && simon.GetSimonState() == SimonState.Posing) //add simon active check
        {
            pendingActions.Remove(input);
          //  Debug.Log("Correct input: " + input);

            if (pendingActions.Count == 0)
            {
                SlideCompletedSuccessfully(input);
            }
        }
        else
        {
        //    Debug.Log("Wrong input: " + input);

            TriggerFail();
        }
    }

    private void SlideCompletedSuccessfully(Rule.ActionType input)
    {
        timerRunning = false;

        Debug.Log("Slide successfully completed.");

        StopAllCoroutines();
        StartCoroutine(SuccessfulSlideDelay(input));

        // jouer le son d'audience correspondant au succ s
        PlayAudienceAudio(input);

        LoadNextSlide();
    }

    private void LoadNextSlide()
    {
        slideCount++;
        //Debug.Log("[GameManager] Loading next slide...");


        requiredActions.Clear();
        pendingActions.Clear();

        if (slideCount % showRuleSlideEveryXSlide == 1)
        {
            Rule newRule = RuleGenerator.GenerateRandomRule(presentedRules);
            presentedRules.Add(newRule);

            Debug.Log("[GameManager] New rule generated: " + newRule.description);

            StopAllCoroutines();
            StartCoroutine(ShowRuleSlideThenContinue("<u>Nouvelle Regle!</u>\n" + newRule.description));


            // RULE SLIDE = no gameplay rules yet
            timerRunning = false;
        }
        else
        {
            slideManager.GenerateSlide();

            // Choose Simon state
            if (UnityEngine.Random.value < simonPresentProbability)
                simon.ChangeState(SimonState.Posing);
            else
                simon.ChangeState(SimonState.Walking);

            // Build active rules BEFORE calling SetRules()
            activeRules.Clear();
            
            if(simon.GetSimonState() == SimonState.Posing)
            {
                foreach (Rule rule in presentedRules)
                {
                    if (slideManager.IsRuleActive(rule))
                    {
                        Debug.Log($"{rule.conditionType} is tested as true");
                        activeRules.Add(rule);
                    }
                    else
                        Debug.Log($"{rule.conditionType} is tested as false");
                }

                // Now correctly build player tasks
                SetRules(activeRules);
            }

            StartSlideTimer();
        }
    }
    
    private void StartSlideTimer()
    {
        if (!timerRunning)
        {
            slideTimer = timeLimit;
            timerRunning = true;
        }
       // Debug.Log("[GameManager] Timer started: " + slideTimer + " seconds");
    }

    private void SlideFailedDueToTimeout()
    {

        if (pendingActions.Count != 0)
        {
            Debug.Log("Slide failed: Time expired.");
            TriggerFail();
        }
        else
        {
            Debug.Log("Slide success: Time expired.");
            LoadNextSlide();
        }
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
      //  Debug.Log("[TestCallback] Re�u : Clap (UI ou clavier). Time=" + Time.time);
        OnPlayerInput(Rule.ActionType.Clap);
    }

    public void OnHiFive()
    {
       // Debug.Log("[TestCallback] Re�u : HiFive (UI ou clavier). Time=" + Time.time);
        OnPlayerInput(Rule.ActionType.HighFive);
    }

    public void OnRiseHands()
    {
       // Debug.Log("[TestCallback] Re�u : RiseHands (UI ou clavier). Time=" + Time.time);
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

    public void TriggerFail()
    {

        requiredActions.Clear();
        pendingActions.Clear();

        if (failRoutine != null)
        {
            StopCoroutine(failRoutine);
        }

        failRoutine = StartCoroutine(FailCoroutine());
    }

    private IEnumerator FailCoroutine()
    {
        // Announce that a fail happened
        OnSlideFail?.Invoke(true);

        // play fail audio once at the start of the fail
        PlayClip(failClip);

        string failedRulesDescription = "Fail !\nVoici toutes les regles:\n";

        /*foreach (Rule.ActionType actionType in Enum.GetValues(typeof(Rule.ActionType)))
        {
            if (pendingActions.Contains(actionType))
            {
                foreach (Rule r in activeRules)
                {
                    if (r.actionType == actionType)
                        failedRulesDescription += r.description + '\n';
                }
            }
        }*/

        foreach(Rule r in presentedRules)
            failedRulesDescription += r.description + '\n';

        simon.ChangeState(SimonState.Walking);
        slideManager.GenerateRuleSlide(failedRulesDescription);

        // Wait for failTime seconds
        yield return new WaitForSeconds(failTime);

        // Announce that the fail effect is over
        OnSlideFail?.Invoke(false);

        failRoutine = null;

        LoadNextSlide();
    }

    private IEnumerator ShowRuleSlideThenContinue(string description)
    {
        slideManager.GenerateRuleSlide(description);

        timerRunning = false; // stop gameplay timer

        yield return new WaitForSeconds(ruleSlideDelay);

        // Now load the next *gameplay* slide
        LoadNextSlide();
    }

    private IEnumerator SuccessfulSlideDelay(Rule.ActionType lastInput)
    {
        // invoke success event so audience reacts
        OnSlideSuccess?.Invoke(lastInput);

        // Simon stops posing -> walks again during applause
        simon.ChangeState(SimonState.Walking);

        yield return new WaitForSeconds(successfulSlideDelay);

        LoadNextSlide();
    }

    // Helper functions to play audio
    private void PlayClip(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;
        audioSource.PlayOneShot(clip);
    }

    private void PlayAudienceAudio(Rule.ActionType action)
    {
        switch (action)
        {
            case Rule.ActionType.Clap:
                PlayClip(clapAudienceClip);
                break;
            case Rule.ActionType.HighFive:
                PlayClip(hiFiveAudienceClip);
                break;
            case Rule.ActionType.RaiseHands:
                PlayClip(riseHandsAudienceClip);
                break;
        }
    }
}
