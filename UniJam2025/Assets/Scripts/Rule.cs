using UnityEngine;

public class Rule
{
    public enum ConditionType { TextRed, TitleUnderlined, ImgDiagram, SimonTwoArms }
    public enum ActionType { Clap, HighFive }

    public ConditionType conditionType;
    public ActionType actionType;
    public int actionAmount;
}
