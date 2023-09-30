using UnityEngine;

public class BehaviorTree : MonoBehaviour
{
    public enum ENodeStatus
    {
        Unknown,
        InProgress,
        Failed,
        Succeeded
    }

    public BTNodeBase RootNode { get; private set; } = new BTNodeBase("ROOT");

    private void Start()
    {
        RootNode.Reset();
    }

    private void Update()
    {
        RootNode.Tick(Time.deltaTime);
    }

    public string GetDebugText()
    {
        return RootNode.GetDebugText();
    }
}
