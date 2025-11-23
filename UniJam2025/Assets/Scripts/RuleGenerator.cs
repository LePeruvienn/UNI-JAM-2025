using System.Collections.Generic;
using UnityEngine;

public static class RuleGenerator
{
    // Randomly generates a rule, avoiding duplicates
    public static Rule GenerateRandomRule(List<Rule> existingRules)
    {
        Rule.ConditionType[] conditions =
            (Rule.ConditionType[])System.Enum.GetValues(typeof(Rule.ConditionType));

        Rule.ActionType[] actions =
            (Rule.ActionType[])System.Enum.GetValues(typeof(Rule.ActionType));

        Rule.ConditionType randomCondition;
        Rule.ActionType randomAction;

        int attempts = 0;
        const int maxAttempts = 100;

        do
        {
            randomCondition = conditions[Random.Range(0, conditions.Length)];
            randomAction = actions[Random.Range(0, actions.Length)];
            attempts++;
        }
        while (existingRules.Exists(r =>
            r.conditionType == randomCondition &&
            r.actionType == randomAction) && attempts < maxAttempts);

        int randomAmount = Random.Range(1, 4);

        string description = GenerateDescription(randomCondition, randomAction, randomAmount);

        return new Rule(randomCondition, randomAction, randomAmount, description);
    }

    public static string GenerateDescription(
    Rule.ConditionType condition,
    Rule.ActionType action,
    int amount)
    {
        string conditionText = condition switch
        {
            Rule.ConditionType.TextRed => "si le texte est rouge",
            Rule.ConditionType.TextBlue => "si le texte est bleu",
            Rule.ConditionType.TextYellow => "si le texte est jaune",
            Rule.ConditionType.TitleUnderlined => "si le titre est souligne",
            Rule.ConditionType.ImgDiagram => "si l'image est un diagramme",
            Rule.ConditionType.ImgAnimal => "Si l'image est un animal REEL",
            _ => ""
        };

        string actionText = action switch
        {
            Rule.ActionType.Clap => "ajoute XXX claps",
            Rule.ActionType.HighFive => "ajoute XXX high-fives",
            Rule.ActionType.RaiseHands => "ajoute XXX levees des mains",
            _ => ""
        };

        return $" {conditionText}, {actionText.Replace("XXX", amount.ToString())}";
    }
}
