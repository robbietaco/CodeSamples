using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class DialogPoints
{
    [Header("Dialog Attributes")]
    public Sprite name;
    public Sprite characterImage;
    [Tooltip("Side of the screen the character will slide into, 1 = left, 2 = right, 0 = none")]
    public int sideInt;

    [Tooltip("Speaking text for dialog")]
    [TextArea(3, 10)]
    public string sentence;

}
