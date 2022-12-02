using TMPro;
using UnityEngine;

namespace Dialogue.LineLogicScripts
{
    public class Interrupt : MonoBehaviour
    {
        private float _timer = 0;
        private const float TimerEnd = 5;

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            gameObject.GetComponentInChildren<TextMeshProUGUI>().text =
                "!! " + gameObject.GetComponentInChildren<TextMeshProUGUI>().text + " !!";
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if(_timer >= TimerEnd) Destroy(gameObject);
        }
    }
}
