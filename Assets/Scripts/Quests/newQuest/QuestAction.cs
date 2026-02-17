using UnityEngine;

[CreateAssetMenu(fileName = "QuestAction", menuName = "Scriptable Objects/QuestAction")]
public abstract class QuestAction : ScriptableObject
{
    public abstract void Execute();

}
