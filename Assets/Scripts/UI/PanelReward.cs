using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
public class PanelReward : MonoBehaviour
{
    [SerializeField] Button _doneBtn;
    [SerializeField] TextMeshProUGUI _absorbTxt;
    [SerializeField] TextMeshProUGUI _extraGoldTxt;

    public void Init(UnityAction doDone)
    {
        _doneBtn.onClick.AddListener(doDone);
    }

    public void SetAbsorbText(string txt)
    {
        _absorbTxt.text = txt;
    }

    public void SetExtraGoldText(string txt)
    {
        _extraGoldTxt.text = txt;
    }

}
