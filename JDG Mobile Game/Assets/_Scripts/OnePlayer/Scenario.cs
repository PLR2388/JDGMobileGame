using System;
using System.Collections.Generic;

/// <summary>
/// Represents the structured data of a game scenario.
/// </summary>
public class Scenario
{
    /// <summary>
    /// Collection of actions defined in the scenario.
    /// </summary>
    public ActionScenario[] ActionScenarios { get; set; }
}

/// <summary>
/// Represents a single action that can be taken during a scenario.
/// </summary>
public class ActionScenario
{
    /// <summary>
    /// Index of the action in the scenario.
    /// </summary>
    public int Index;
    
    /// <summary>
    /// Type of highlight associated with the action.
    /// </summary>
    public Highlight Highlight;
    
    /// <summary>
    /// Card associated with the action.
    /// </summary>
    public string PutCard;
    
    /// <summary>
    /// Image associated with the action.
    /// </summary>
    public string Image;
    
    /// <summary>
    /// Video associated with the action.
    /// </summary>
    public string Video;
    
    /// <summary>
    /// Attack sequences associated with the action.
    /// </summary>
    public string[] Attack;
    
    /// <summary>
    /// Type of action to be taken.
    /// </summary>
    public Action Action;
}

/// <summary>
/// Represents the raw JSON data for a scenario.
/// </summary>
[Serializable]
public class JsonScenario
{
    public ActionJsonScenario[] jsonScenarios;

    /// <summary>
    /// Converts the JSON data into a structured Scenario object.
    /// </summary>
    /// <returns>A Scenario object derived from the JSON data.</returns>
    public Scenario ToScenario()
    {
        var scenario = new Scenario();
        var actionScenarios = new List<ActionScenario>();
        foreach (var actionJson in jsonScenarios)
        {
            var actionScenario = ToActionScenario(actionJson);
            actionScenarios.Add(actionScenario);
        }

        scenario.ActionScenarios = actionScenarios.ToArray();
        return scenario;
    }
    
    /// <summary>
    /// Converts a JSON action representation to an ActionScenario object.
    /// </summary>
    /// <param name="actionJson">The JSON representation of the action.</param>
    /// <returns>An ActionScenario derived from the JSON data.</returns>
    private static ActionScenario ToActionScenario(ActionJsonScenario actionJson)
    {
        var actionScenario = new ActionScenario
        {
            Index = actionJson.index
        };
        
        SetHighlightFromActionJson(actionJson, actionScenario);
        SetActionFromActionJson(actionJson, actionScenario);

        actionScenario.PutCard = actionJson.putCard;
        actionScenario.Image = actionJson.image;
        actionScenario.Video = actionJson.video;
        actionScenario.Attack = actionJson.attack?.Split('>');
        return actionScenario;
    }
    
    /// <summary>
    /// Sets the action type for an ActionScenario based on the JSON representation.
    /// </summary>
    /// <param name="actionJson">The JSON representation of the action.</param>
    /// <param name="actionScenario">The ActionScenario object to be modified.</param>
    private static void SetActionFromActionJson(ActionJsonScenario actionJson, ActionScenario actionScenario)
    {
        if (actionJson.action == null)
        {
            actionScenario.Action = Action.unknown;
        }
        else
        {
            Enum.TryParse(actionJson.action, out actionScenario.Action);
        }
    }
    
    /// <summary>
    /// Sets the highlight type for an ActionScenario based on the JSON representation.
    /// </summary>
    /// <param name="actionJson">The JSON representation of the action.</param>
    /// <param name="actionScenario">The ActionScenario object to be modified.</param>
    private static void SetHighlightFromActionJson(ActionJsonScenario actionJson, ActionScenario actionScenario)
    {

        if (actionJson.highlight == null)
        {
            actionScenario.Highlight = Highlight.unknown;
        }
        else
        {
            Enum.TryParse(actionJson.highlight, out actionScenario.Highlight);
        }
    }
}

/// <summary>
/// Represents the raw JSON data for an action within a scenario.
/// </summary>
[Serializable]
public class ActionJsonScenario
{
    /// <summary>
    /// Index of the action in the JSON data.
    /// </summary>
    public int index;
    
    /// <summary>
    /// Type of highlight in the JSON data.
    /// </summary>
    public string highlight;
    
    /// <summary>
    /// Card information in the JSON data.
    /// </summary>
    public string putCard;
    
    /// <summary>
    /// Image information in the JSON data.
    /// </summary>
    public string image;
    
    /// <summary>
    /// Video information in the JSON data.
    /// </summary>
    public string video;
    
    /// <summary>
    /// Attack sequences in the JSON data.
    /// </summary>
    public string attack;
    
    /// <summary>
    /// Type of action in the JSON data.
    /// </summary>
    public string action;
}

/// <summary>
/// Enumerates possible highlights in a scenario.
/// </summary>
public enum Highlight
{
    space,
    deck,
    yellow_trash,
    field,
    invocation_cards,
    effect_cards,
    hand_cards,
    next_phase,
    unknown,
    tentacules,
    life_point
}

/// <summary>
/// Enumerates possible actions that can be taken in a scenario.
/// </summary>
public enum Action
{
    next_phase,
    unknown
}