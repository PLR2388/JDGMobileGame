using UnityEngine;

public class TutorialMessage : MonoBehaviour
{
    public Scenario scenario;

    private ActionScenario[] actionScenario;
    
    // State
    private int currentIndex = 0;
    private bool messageShowed = false;
  
    void Start()
    {
        actionScenario = scenario.actionScenarios;
    }
    
    void Update()
    {
        var action = actionScenario[currentIndex];

        if (action.Message != null && !messageShowed)
        {
            
        }

        if (action.Highlight != Highlight.unknown)
        {
            
        }

        if (action.PutCard != null)
        {
            
        }

        if (action.Trigger != Trigger.unknown)
        {
            
        }

        if (action.Image != null)
        {
            
        }

        if (action.Video != null)
        {
            
        }

        if (action.Attack != null)
        {
            
        }
    }

    private void DisplayMessage(string message)
    {
        
    }

    void Next()
    {
        currentIndex++;
    }
}
