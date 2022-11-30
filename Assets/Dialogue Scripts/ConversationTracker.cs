using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;


public class ConversationTracker : MonoBehaviour
{
  
    // object in top screen holding text
    [SerializeField] private GameObject TopPanel;
    [SerializeField] private TextMeshProUGUI CurrentTxt;
    
    // button prefab
    [SerializeField] private GameObject button;
    
    // player panels
    [SerializeField] private GameObject player1panel;
    [SerializeField] private GameObject player2panel;

    // next story dialogue
    [SerializeField] private GameObject NextButton;
    
    // next action button
    [SerializeField] private GameObject InteractButton;

    // the first info sequence;
    [SerializeField] private string[] InfoSequence1;
    
    // how far are we in the info sequence
    int sequencetracker = 0;
    
    private int nextState; // 0 = info is next, 1 = player action is next, 2 = NPC is next
    
    
    // player 1 - choices 1
    [SerializeField] private string[] P1Choices1;
    
    // player 2 - choices 1
    [SerializeField] private string[] P2Choices1;

    // NPC dialogue
    [SerializeField] private string[] NPCDialogue;
    private int NPCSequenceTracker = 0;
    



    public void StartInfoSequence()
    {
        LoadNPCContent(InfoSequence1[sequencetracker], false);
    }

    public void NextInfo()
    {
        if (sequencetracker < InfoSequence1.Length-1) // if the info dialogue is done, change the state to player action
        {
            LoadNPCContent(InfoSequence1[sequencetracker], false);
        }
        else
        {
            InfoOver();
        }
    }

    public void NextNPCLine()
    {
        print("I got here");
        player1panel.SetActive(false);
        player2panel.SetActive(false);
        LoadNPCContent(NPCDialogue[NPCSequenceTracker], true);
    }

    public void InfoOver()
    {
        NextButton.SetActive(false);
        if (InfoSequence1[InfoSequence1.Length-1] == "Players")
        {
            print("Player Actions now");
                    
            LoadPlayerContent(player1panel, P1Choices1.Length, P1Choices1);
            LoadPlayerContent(player2panel, P2Choices1.Length, P2Choices1);
        }
        
        if (InfoSequence1[InfoSequence1.Length-1] == "NPC")
        {
            print("NPC Dialogue now");
            LoadNPCContent(NPCDialogue[NPCSequenceTracker], true);
        }
    }
    
    void LoadNPCContent(string dialogue, bool NPC)
    {
        TopPanel.SetActive(true);
        CurrentTxt.text = dialogue;

        if (!NPC) // if story we need next button, if npc we do not
        {
            sequencetracker += 1;
            NextButton.SetActive(true);
        }

        if (NPC) // if NPC said something, we need to enable the player options
        {
            NPCSequenceTracker += 1;
            
            // load first player options
            LoadPlayerContent(player1panel, P1Choices1.Length, P1Choices1);
            LoadPlayerContent(player2panel, P2Choices1.Length, P2Choices1);
        }
        
    }

    void LoadPlayerContent(GameObject player, int responses, string[] responseText)
    {
        player.SetActive(true);
        for (int i = 0; i < responses; i++)
        {
            GameObject dialogueButton = Instantiate(button, player.transform);
            dialogueButton.GetComponentInChildren<TextMeshProUGUI>().text = responseText[i];
            
            var btn = player.GetComponentInChildren<UnityEngine.UI.Button>();
            btn.onClick.AddListener((() => OnPlayerChose()));
        }
    }

    void OnPlayerChose()
    {
        
    }
    
    
}
