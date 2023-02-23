using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ExpText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _text;

    public void SetText(string txt)
    {
        _text.text = txt;
    }
}
