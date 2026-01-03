using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    [TextArea(3, 5)]
    public string text;
    public string characterName = "DefaultName";
    public Sprite characterPortrait;
}

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    public DialogueLine[] dialogueLines;
    public string ghostName = "DefaultName";
    public Sprite defaultPersonPortrait;
}

