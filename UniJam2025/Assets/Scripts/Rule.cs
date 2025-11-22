using UnityEngine;

public class Rule
{
    public enum ConditionType { TextRed, TitleUnderlined, ImgDiagram, SimonTwoArms }
    public enum ActionType { Clap, HighFive, RaiseHands }

    public ConditionType conditionType;
    public ActionType actionType;
    public int actionAmount;


    public string description; 
    public Rule(ConditionType condition, ActionType action, int amount, string description)
    {
        this.conditionType = condition;
        this.actionType = action;
        this.actionAmount = amount;
        this.description = description;
    }
}
