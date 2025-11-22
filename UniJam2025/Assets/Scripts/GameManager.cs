using Assets;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private List<Rule.ActionType> requiredActions = new List<Rule.ActionType>();
    private List<Rule.ActionType> pendingActions = new List<Rule.ActionType>();

    public SlideManager slideManager;   

    public int health = 50;

    private int slideCount = 0;

    private float slideTimer = 0f;
    private bool timerRunning = false;

    public float timeLimit = 5f;   


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

        if (pendingActions.Contains(input))
        {
            pendingActions.Remove(input);
            Debug.Log("Correct input: " + input);

            if (pendingActions.Count == 0)
            {
                SlideCompletedSuccessfully();
            }
        }
        else
        {
            Debug.Log("Wrong input: " + input);
            ApplyPenalty();
        }
    }

    private void SlideCompletedSuccessfully()
    {

        timerRunning = false;

        Debug.Log("Slide successfully completed.");
        IncreaseHealth();

        LoadNextSlide();
    }

    private void LoadNextSlide()
    {
        slideCount++;
        Debug.Log("[GameManager] Loading next slide...");

        slideManager.GenerateSlide();
    }

    private void IncreaseHealth()
    {
        health = Mathf.Clamp(health + 10, 0, 100);
        Debug.Log("Health increased. Current health: " + health);
    }

    private void ApplyPenalty()
    {
        health = Mathf.Clamp(health - 10, 0, 100);
        Debug.Log("Penalty applied. Current health: " + health);
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
        ApplyPenalty();

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

}
