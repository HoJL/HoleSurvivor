using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PanelJoypad : MonoBehaviour
{
    [SerializeField] RectTransform _viewport = null;
    [SerializeField] RectTransform _anchor = null;
    [SerializeField] RectTransform _frame = null;
    [SerializeField] RectTransform _pad = null;
    [SerializeField] RectTransform _mainPanel = null;

    [Space]
    [SerializeField] bool _interactable = true;
    [SerializeField] float _moveThreshold = 3.0f;
    [SerializeField] float _padRadius = 100.0f;
    IPlayable _connectedCharacter = null;

    bool _beginInput = false;
    Vector3 _beginPos = Vector3.zero;
    Vector2 _startFramePos = Vector2.zero;
    float _xFactor = 0.0f;
    float _yFactor = 0.0f;

    public bool Interactable
    {
        get => _interactable;
        set => _interactable = value;
    }

    void Start()
    {
        Init();
    }

    public void Init()
    {
        _frame.position         = _anchor.position;
        _pad.anchoredPosition   = Vector2.zero;

        if (null == _viewport)
            return;

        _xFactor = _viewport.rect.width     / (float)Screen.width;
        _yFactor = _viewport.rect.height    / (float)Screen.height;
    }

    public void Release()
    {
        _beginInput = false;
        _beginPos   = Vector3.zero;
        _xFactor    = 0.0f;
        _yFactor    = 0.0f;
    }

    public void SetConnect(IPlayable character) => _connectedCharacter = character;

    void Update()
    {
        if (GameManager.Instance.IsPause) return;
        if (GameManager.Instance.IsGameEnd) return;
        if (!_interactable)
        {
            if (_beginInput)
                FinishInput();

            return;
        }

        if (!_beginInput)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject()) return;
                GameManager.Instance.IsGameStart = true;
                _mainPanel.gameObject.SetActive(false);
                BeginInput();
            }
            return;
        }

        if (!Input.GetMouseButton(0))
        {
            FinishInput();
            return;
        }
        DragInput();
    }

    void BeginInput()
    {
        _beginInput = true;
        _frame.gameObject.SetActive(true);
        _beginPos   = Input.mousePosition;
        Debug.Log(Input.mousePosition);
        _startFramePos = new Vector2(_beginPos.x * _xFactor, _beginPos.y * _yFactor);
        _frame.anchoredPosition = _startFramePos;
        _pad.anchoredPosition = Vector2.zero;
    }

    void DragInput()
    {
        var dragPos = Input.mousePosition;
        var padPos = new Vector2(dragPos.x * _xFactor, dragPos.y * _yFactor) - _frame.anchoredPosition;
        var sqrDistance = padPos.sqrMagnitude;
        
        if (sqrDistance < _moveThreshold * _moveThreshold)
        {
            if (null != _connectedCharacter)
                _connectedCharacter.FinishMove();

            _pad.anchoredPosition = padPos;
            return;
        }
        var radian = Mathf.Atan2(padPos.x, padPos.y);
        if (null != _connectedCharacter)
            _connectedCharacter.UpdateMove(radian, new Vector3(padPos.x, 0, padPos.y));
        
        if (_padRadius * _padRadius < sqrDistance)
        {
            var x   = Mathf.Sin(radian) * _padRadius;
            var y   = Mathf.Cos(radian) * _padRadius;
            padPos  = new Vector2(x, y);
        }
        
        _pad.anchoredPosition = padPos;
    }

    void FinishInput()
    {
        if (null != _connectedCharacter)
        {
            _connectedCharacter.FinishMove();
        }
        _frame.gameObject.SetActive(false);
        _beginInput = false;
        _frame.position = _anchor.position;
        _pad.anchoredPosition = Vector2.zero;
    }
}

