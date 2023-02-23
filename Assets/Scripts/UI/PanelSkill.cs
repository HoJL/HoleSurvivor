using System;
using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PanelSkill : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _magnetTimeText;
    [SerializeField] Image _magnetImg;

    public void SetMagnetTime(float time, float maxTime)
    {
        _magnetImg.fillAmount = 1 - time / maxTime;
        var t = TimeSpan.FromSeconds(maxTime - time);
        _magnetTimeText.text = string.Format("{0:D1}:{1:D2}", t.Minutes, t.Seconds);
    }
}
