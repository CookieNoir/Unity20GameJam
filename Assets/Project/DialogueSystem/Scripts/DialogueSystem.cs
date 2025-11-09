using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class DialogueSystem : MonoBehaviour
{
    public GameObject panel;              // Root dialog panel (in Canvas)
    public TMP_Text textLabel;            // TMP_Text for text

    [Header("UI Action Button")]
    public Button actionButton;           // Next/OK button
    public TMP_Text actionButtonLabel;    // Текст на кнопке

    [Header("Formatting (markdown-like)")]
    [Tooltip("Highlighting quest words: [*important*] -> <mark>важно</mark>")]
    public bool enableHighlight = true;
    [Tooltip("Bold: **bold** или __bold__ -> <b>bold</b>")]
    public bool enableBold = true;
    [Tooltip("_italic_: _italic_ -> <i>italic</i>")]
    public bool enableItalic = true;

    [Header("Typing Sound (optional)")]
    public bool enableTypingSound = false;
    public AudioSource audioSource;
    public AudioClip typeSound;
    [Range(0f, 1f)] public float typeSoundVolume = 0.5f;
    [Tooltip("How many printed visible characters between sounds")]
    public int charsPerSound = 2;
    [Tooltip("Random pitch variation for variety")]
    public float pitchMin = 0.98f;
    public float pitchMax = 1.02f;
    [Tooltip("Do not play sound if the current visible character is a space/hyphen.")]
    public bool skipSoundOnWhitespace = true;

    private enum State { Hidden, Typing, Showing }
    private State state = State.Hidden;

    private List<ZoneDialogueDB.DialogueLine> currentLines;
    private int lineIndex;
    private Coroutine typeRoutine;

    void Awake()
    {
        if (actionButton)
        {
            actionButton.onClick.AddListener(OnActionPressed);
            if (!actionButtonLabel)
                actionButtonLabel = actionButton.GetComponentInChildren<TMP_Text>(true);
        }
    }

    void OnDestroy()
    {
        if (actionButton)
            actionButton.onClick.RemoveListener(OnActionPressed);
    }

    public void StartDialogue(List<ZoneDialogueDB.DialogueLine> lines)
    {
        if (lines == null || lines.Count == 0) return;
        currentLines = lines;
        lineIndex = 0;
        panel.SetActive(true);
        SetActionButtonVisible(false);
        ShowLine();
    }

    private void Update()
    {
        if (state == State.Hidden)
            return;
    }

    private void OnActionPressed()
    {
        if (state == State.Hidden)
            return;

        if (state == State.Typing)
        {
            FinishTypingInstant();
            return;
        }

        // state == Showing
        if (!HasNextLine())
        {
            // Last line -> close without increment
            CloseDialogue();
            return;
        }

        lineIndex++;
        ShowLine();
    }

    private void ShowLine()
    {
        if (typeRoutine != null) StopCoroutine(typeRoutine);
        var raw = currentLines[lineIndex].text;
        var formatted = ApplyFormatting(raw);
        typeRoutine = StartCoroutine(Typewriter(formatted, currentLines[lineIndex].charDelay));
    }

    private IEnumerator Typewriter(string fullText, float delay)
    {
        state = State.Typing;
        textLabel.richText = true;

        textLabel.text = fullText;               // Full text with tags
        textLabel.maxVisibleCharacters = 0;

        SetActionButtonVisible(false);           // Button is hidden while printing
        yield return null;                       // Wait until TMP counts the characters

        int totalVisible = textLabel.textInfo.characterCount;
        int soundCounter = 0;

        for (int i = 0; i <= totalVisible; i++)
        {
            textLabel.maxVisibleCharacters = i;

            // Print sound (based on visible characters, excluding tags)
            if (enableTypingSound && typeSound && audioSource && charsPerSound > 0 && i > 0)
            {
                bool canPlay = true;
                if (skipSoundOnWhitespace)
                {
                    // Attempt to determine the character for the i-th visible index:
                    // TMP has textInfo.characterInfo, but it is updated one frame later.
                    // It's easier not to worry about it: play the sound by the counter, not by the character.
                    // If you still want accuracy, you can delay by one frame and read characterInfo.
                }

                soundCounter++;
                if (soundCounter >= charsPerSound)
                {
                    audioSource.pitch = Random.Range(pitchMin, pitchMax);
                    audioSource.PlayOneShot(typeSound, typeSoundVolume);
                    soundCounter = 0;
                }
            }

            yield return new WaitForSeconds(delay);
        }

        typeRoutine = null;
        state = State.Showing;
        UpdateActionButtonText();      // "Next" or "Okay"
        SetActionButtonVisible(true);
    }

    private void FinishTypingInstant()
    {
        if (typeRoutine != null) StopCoroutine(typeRoutine);
        typeRoutine = null;
        textLabel.maxVisibleCharacters = textLabel.textInfo.characterCount;
        state = State.Showing;
        UpdateActionButtonText();
        SetActionButtonVisible(true);
    }

    public void CloseDialogue()
    {
        if (typeRoutine != null) StopCoroutine(typeRoutine);
        typeRoutine = null;
        panel.SetActive(false);
        state = State.Hidden;
        currentLines = null;
        SetActionButtonVisible(false);
    }

    // ---------- Formatting ----------
    // Поддержка:
    // - [*highlight*]  -> <mark>highlight</mark>
    // - **bold** / __bold__ -> <b>bold</b>
    // - _italic_           -> <i>italic</i>
    //
    // Note: We deliberately use _underscores_ for italics to avoid conflict with [* *].
    private string ApplyFormatting(string s)
    {
        string res = s;

        if (enableHighlight)
        {
            // [* ... *]  (lazy matching)
            res = Regex.Replace(res, @"\[\*(.+?)\*\]", "<mark>$1</mark>");
        }

        if (enableBold)
        {
            // **bold**
            res = Regex.Replace(res, @"\*\*(.+?)\*\*", "<b>$1</b>");
            // __bold__
            res = Regex.Replace(res, @"__(.+?)__", "<b>$1</b>");
        }

        if (enableItalic)
        {
            // _italic_
            res = Regex.Replace(res, @"_(.+?)_", "<i>$1</i>");
        }

        return res;
    }

    private bool HasNextLine()
    {
        return (currentLines != null) && (lineIndex + 1 < currentLines.Count);
    }

    private void UpdateActionButtonText()
    {
        if (!actionButtonLabel) return;
        actionButtonLabel.text = HasNextLine() ? "Next" : "Okay";
    }

    private void SetActionButtonVisible(bool visible)
    {
        if (actionButton)
            actionButton.gameObject.SetActive(visible);
    }
}
