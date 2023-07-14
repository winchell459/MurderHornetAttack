using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialoguePanel : MonoBehaviour
{
    [SerializeField] private Text dialogueText;

    public string text { set { dialogueText.text = value; } }
}
