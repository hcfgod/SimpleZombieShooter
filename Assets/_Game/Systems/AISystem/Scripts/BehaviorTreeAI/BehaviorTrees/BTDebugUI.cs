using UnityEngine;
using TMPro;

public class BTDebugUI : MonoBehaviour
{
    [SerializeField] BehaviorTree LinkedBT;
    [SerializeField] TextMeshProUGUI LinkDebugText;

    private void Start()
    {
        LinkDebugText.text = "";
    }

    private void Update()
    {
        LinkDebugText.text = LinkedBT.GetDebugText();
    }
}
