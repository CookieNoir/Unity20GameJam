using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ZoneTriggerDialog : MonoBehaviour
{
    [Tooltip("Unique zone ID (matches the key in the database).")]
    public string zoneId = "mine_entrance";

    public ZoneDialogueDB db;
    public DialogueSystem dialogueSystem;

    [Tooltip("Display the dialog only if the player has pressed the button inside the trigger (E). If false, launch immediately upon entry.")]
    public bool requireButtonPress = false;

    [Header("Behaviour")]
    public bool closeOnExit = true;                 // close the dialog when leaving the zone
    private bool dialogueOpenedByThisZone = false;  // so as not to close someone else's dialog

    private static HashSet<string> shownZones = new();
    private bool playerInside = false;
    private const string PlayerPrefsPrefix = "zone_shown_";

    private void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void Start()
    {
        // if (PlayerPrefs.GetInt(PlayerPrefsPrefix + zoneId, 0) == 1)
        //     shownZones.Add(zoneId);
    }

    private void Update()
    {
        if (!requireButtonPress) return;
        if (!playerInside) return;
        if (shownZones.Contains(zoneId)) return;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) 
            return;
        playerInside = true;


        if (requireButtonPress) 
            return;

        TryStartDialogue();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = false;

        if (closeOnExit && dialogueOpenedByThisZone && dialogueSystem != null)
        {
            dialogueSystem.CloseDialogue();
            dialogueOpenedByThisZone = false;
        }
    }

    private void TryStartDialogue()
    {
        if (shownZones.Contains(zoneId)) 
            return;

        var lines = db != null ? db.GetLines(zoneId) : null;
        if (lines == null || lines.Count == 0) 
            return;

        dialogueSystem.StartDialogue(lines);
        dialogueOpenedByThisZone = true;

        shownZones.Add(zoneId);
        PlayerPrefs.SetInt(PlayerPrefsPrefix + zoneId, 1);
        PlayerPrefs.Save();
    }
}
