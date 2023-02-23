using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PanelSizeUp : MonoBehaviour
{
    [SerializeField] Button _magnetBtn;
    [SerializeField] Button _heartBtn;
    [SerializeField] TextMeshProUGUI _levelTxt;

    public void Init(UnityEngine.Events.UnityAction onClick)
    {
        _magnetBtn.onClick.AddListener(onClick);
        _heartBtn.onClick.AddListener(onClick);
    }

    public void SetLevelText(string txt)
    {
        _levelTxt.text = txt;
    }
}
