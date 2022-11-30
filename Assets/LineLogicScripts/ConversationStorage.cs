using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
public class ConversationStorage : MonoBehaviour
{
    private List<string> p1Context, p2Context, p1Information, p2Information;
    

    public void AddContext(string context, int player)
    {
        switch (player)
        {
            case 1:
                p1Context.Add(context);
                break;
            case 2:
                p2Context.Add(context);
                break;
        }
    }

    public bool CheckContext(string context, int player)
    {
        return player switch //Fancy return switch case :)
        {
            1 => p1Context.Contains(context),
            2 => p2Context.Contains(context),
            _ => false
        };
    }
    
    public void AddInformation(string information, int player)
    {
        switch (player)
        {
            case 1:
                p1Information.Add(information);
                break;
            case 2:
                p2Information.Add(information);
                break;
        }
    }

    public bool CheckInformation(string information, int player)
    {
        return player switch //Fancy return switch case :)
        {
            1 => p1Information.Contains(information),
            2 => p2Information.Contains(information),
            _ => false
        };
    }
    




}
