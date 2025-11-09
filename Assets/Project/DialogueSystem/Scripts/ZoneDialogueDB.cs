using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Zone Dialogue DB", fileName = "ZoneDialogueDB")]
public class ZoneDialogueDB : ScriptableObject
{
    [Serializable]
    public class DialogueLine
    {
        [TextArea(2, 5)]
        public string text;
        public float charDelay = 0.02f;
    }

    [Serializable]
    public class ZoneDialogue
    {
        public string zoneId;
        public List<DialogueLine> lines;
    }

    public List<ZoneDialogue> dialogues = new();

    public List<DialogueLine> GetLines(string zoneId)
    {
        var found = dialogues.Find(d => d.zoneId == zoneId);
        return found != null ? found.lines : null;
    }
}
