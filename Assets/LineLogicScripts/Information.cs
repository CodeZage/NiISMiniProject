using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Button = UnityEngine.UI.Button;

public class Information : MonoBehaviour
{
    public bool informationType; //true = getInformation, false = setInformation
    public string information;
    public int player;
    private ConversationStorage conversationStorage;
    public Button button;

    private void Awake()
    {
        Initialize();
        if(!informationType)button.onClick.AddListener(TaskOnClick);
        else if(conversationStorage.CheckInformation(information, player)) Destroy(this);
    }



    private void Initialize()
    {
        //If button is null, assign button to button
        button ??= GetComponent<Button>();
        conversationStorage = GameObject.Find("ConversationStorage").GetComponent<ConversationStorage>();
    }

    private void TaskOnClick()
    {
        conversationStorage.AddInformation(information,player);
    }

    public void SetContext(string c)
    {
        information = c;
    }

    public void SetPlayer(int p)
    {
        player = p;
    }

    public void SetContextType(bool b)
    {
        informationType = b;
    }

}
