using System.Collections.Generic;
using UnityEngine;

public static class RuleGenerator
{
    // Randomly generates a rule, avoiding duplicates
    public static Rule GenerateRandomRule(List<Rule> existingRules)
    {
        Rule.ConditionType[] conditions = (Rule.ConditionType[])System.Enum.GetValues(typeof(Rule.ConditionType));
        Rule.ActionType[] actions = (Rule.ActionType[])System.Enum.GetValues(typeof(Rule.ActionType));

        Rule.ConditionType randomCondition;
        Rule.ActionType randomAction;

        int attempts = 0;
        const int maxAttempts = 100; // avoid infinite loop

        do
        {
            randomCondition = conditions[Random.Range(0, conditions.Length)];
            randomAction = actions[Random.Range(0, actions.Length)];
            attempts++;
        }
        while (existingRules.Exists(r => r.conditionType == randomCondition && r.actionType == randomAction)
               && attempts < maxAttempts);

        int randomAmount = Random.Range(2, 6); // 2 to 5 inclusive

        return new Rule(randomCondition, randomAction, randomAmount);
    }
}
