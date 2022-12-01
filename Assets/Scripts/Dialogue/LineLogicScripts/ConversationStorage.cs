using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue.LineLogicScripts
{
    public class ConversationStorage : MonoBehaviour
    {
        public static ConversationStorage Instance { get; private set; }
        
        private List<string> _p1Context, _p2Context, _p1Information, _p2Information;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            
            _p1Context = new List<string>();
            _p2Context = new List<string>();
            _p1Information = new List<string>();
            _p2Information = new List<string>();
        }

        public void AddContext(string context, int player)
        {
            switch (player)
            {
                case 1:
                    _p1Context.Add(context);
                    break;
                case 2:
                    _p2Context.Add(context);
                    break;
            }
        }

        public bool CheckContext(string context, int player)
        {
            return player switch //Fancy return switch case :)
            {
                1 => _p1Context.Contains(context),
                2 => _p2Context.Contains(context),
                _ => false
            };
        }
    
        public void AddInformation(string information, int player)
        {
            switch (player)
            {
                case 1:
                    _p1Information.Add(information);
                    break;
                case 2:
                    _p2Information.Add(information);
                    break;
            }
        }

        public bool CheckInformation(string information, int player)
        {
            return player switch //Fancy return switch case :)
            {
                1 => _p1Information.Contains(information),
                2 => _p2Information.Contains(information),
                _ => false
            };
        }
    }
}
