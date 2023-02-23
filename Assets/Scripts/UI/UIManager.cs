using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    [SerializeField] PanelJoypad _joypad;
    [SerializeField] PanelReward _reward;
    [SerializeField] PanelSizeUp _sizeUp;
    [SerializeField] PanelSkill _skill;
    Action<int> _doGetPower;
    public event Action<int> DoGetPower
    {
        add => _doGetPower += value;
        remove => _doGetPower -= value;
    }

    public void Init(IPlayable player)
    {
        _joypad.SetConnect(player);
        _reward.Init(DoDone);
        _sizeUp.Init(SizeUpClick);
    }

    public void ShowRewardPanel(string absorb)
    {
        _reward.SetAbsorbText(absorb);
        _reward.gameObject.SetActive(true);
    }

    public void ShowSizeUpPanel(string txt)
    {
        _sizeUp.SetLevelText(txt);
        _sizeUp.gameObject.SetActive(true);
    }

    public void ShowSkillPanel(float time, float maxTime)
    {
        _skill.SetMagnetTime(time, maxTime);
        _skill.gameObject.SetActive(true);
    }

    public void HideSkillPanel()
    {
        _skill.gameObject.SetActive(false);
    }

    void DoDone()
    {
        Time.timeScale = 1;
        _reward.gameObject.SetActive(false);
        SceneManager.LoadScene("SampleScene");
    }

    void SizeUpClick()
    {
        var name = EventSystem.current.currentSelectedGameObject.name;
        Time.timeScale = 1;
        GameManager.Instance.IsPause = false;
        _sizeUp.gameObject.SetActive(false);
        _doGetPower?.Invoke(int.Parse(name));
    }
}
