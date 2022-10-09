using System;
using System.Collections;
using UnityEngine;

public class Scenario
{
    public ActionScenario[] actionScenarios;
    public DialogueObject dialogueObject;
}

public class ActionScenario
{
    public string Message;
    public Highlight Highlight;
    public string PutCard;
    public Trigger Trigger;
    public string Image;
    public string Video;
    public string[] Attack;
}

[System.Serializable]
public class JsonScenario
{
    public ActionJsonScenario[] jsonScenarios;

    public Scenario ToScenario()
    {
        var scenario = new Scenario();
        var actionScenarios = new ArrayList();
        foreach (var actionJson in jsonScenarios)
        {
            var actionScenario = new ActionScenario();
            actionScenario.Message = actionJson.message;
            if (actionJson.highlight == null)
            {
                actionScenario.Highlight = Highlight.unknown;
            }
            else
            {
                Enum.TryParse(actionJson.highlight, out actionScenario.Highlight);
            }

            if (actionJson.trigger == null)
            {
                actionScenario.Trigger = Trigger.unknown;
            }
            else
            {
                Enum.TryParse(actionJson.trigger, out actionScenario.Trigger);
            }
        
            actionScenario.PutCard = actionJson.putCard;
            actionScenario.Image = actionJson.image;
            actionScenario.Video = actionJson.video;
            actionScenario.Attack = actionJson.attack?.Split('>');
            actionScenarios.Add(actionScenario);
        }

        scenario.actionScenarios = (ActionScenario[])actionScenarios.ToArray(typeof(ActionScenario));
        return scenario;
    }
}

[System.Serializable]
public class ActionJsonScenario
{
    public string message;
    public string highlight;
    public string putCard;
    public string trigger;
    public string image;
    public string video;
    public string attack;
}

public enum Highlight
{
    deck,
    yellow_trash,
    field,
    invocation_cards,
    effect_cards,
    hand_cards,
    next_phase,
    unknown
}

public enum Trigger
{
    put_card,
    next_phase,
    put_card_effect,
    unknown
}