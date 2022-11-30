using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Button = UnityEngine.UI.Button;

public class Context : MonoBehaviour
{

    public bool contextType; //true = getContext, false = setContext
    public string context;
    public int player;
    private ConversationStorage conversationStorage;
    public Button button;

    private void Awake()
    {
        Initialize();
        if(!contextType)button.onClick.AddListener(TaskOnClick);
        else if(conversationStorage.CheckContext(context, player)) Destroy(this);
    }



    private void Initialize()
    {
        //If button is null, assign button to button
        button ??= GetComponent<Button>();
        conversationStorage = GameObject.Find("ConversationStorage").GetComponent<ConversationStorage>();
    }

    private void TaskOnClick()
    {
        conversationStorage.AddContext(context,player);
    }

    public void SetContext(string c)
    {
        context = c;
    }

    public void SetPlayer(int p)
    {
        player = p;
    }

    public void SetContextType(bool b)
    {
        contextType = b;
    }
    
    
}
