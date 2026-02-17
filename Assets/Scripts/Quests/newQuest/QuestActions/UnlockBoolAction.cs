using UnityEngine;

[CreateAssetMenu(fileName = "UnlockBoolAction", menuName = "Scriptable Objects/UnlockBoolAction")]
public class UnlockBoolAction : QuestAction
{
    public string boolName;
    public override void Execute()
    {
        GameManager.Instance.SetFlag(boolName, true);
    }
}
