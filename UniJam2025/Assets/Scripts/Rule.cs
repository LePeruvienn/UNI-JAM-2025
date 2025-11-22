using UnityEngine;

public class Rule
{
    public enum ConditionType { TextRed, TitleUnderlined, ImgDiagram, SimonTwoArms }
    public enum ActionType { Clap, HighFive, RaiseHands }

    public ConditionType conditionType;
    public ActionType actionType;
    public int actionAmount;

    public Rule(ConditionType condition, ActionType action, int amount)
    {
        conditionType = condition;
        actionType = action;
        actionAmount = amount;
    }
}
