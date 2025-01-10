using System;
using UnityEngine;

/// <summary>
/// 키보드 입력을 이용하여 플레이어를 조종할 수 있는 클래스
/// </summary>
public class Controller : MonoBehaviour
{
    //오른쪽 이동 방향
    private const bool RightDirection = true;
    //왼쪽 이동 방향
    private const bool LeftDirection = false;

    //조종할 대상 플레이어
    public Player _player;

    /// <summary>
    /// 특정 키보드 입력이 이루어졌는지 확인하고 그 결과를 반환하는 구조체 
    /// </summary>
    [Serializable]
    private struct Input
    {
        [SerializeField]
        public KeyCode[] keyCodes;

        public bool isPressed
        {
            set;
            get;
        }
    }

    //방향키 ↑
    [SerializeField, Header("방향키 ↑")]
    private Input _upInput;
    //방향키 ↓
    [SerializeField, Header("방향키 ↓")]
    private Input _downInput;
    //방향키 ←
    [SerializeField, Header("방향키 ←")]
    private Input _leftInput;
    //방향키 →
    [SerializeField, Header("방향키 →")]
    private Input _rightInput;
    //공격 1
    [SerializeField, Header("공격 1")]
    private Input _attack1Input;
    //공격 2
    [SerializeField, Header("공격 2")]
    private Input _attack2Input;
    //공격 3
    [SerializeField, Header("공격 3")]
    private Input _attack3Input;
    //광역기
    [SerializeField, Header("광역기")]
    private Input _attackWideInput;
    //점프
    [SerializeField, Header("점프")]
    private KeyCode[] _jumpKeyCodes;
    //상호 작용
    [SerializeField, Header("상호 작용")]
    private KeyCode[] _interactionKeyCodes;
    //체력 회복
    [SerializeField, Header("체력 회복")]
    private KeyCode[] _healKeyCodes;

    //캔버스가 활성화 될 때는 캔버스를 선택하는 내용으로 바꿈

    private void Update()
    {
        SetKey(ref _upInput);
        SetKey(ref _downInput);
        SetKey(ref _leftInput);
        SetKey(ref _rightInput);
        SetKey(ref _attack1Input);
        SetKey(ref _attack2Input);
        SetKey(ref _attack3Input);
        SetKey(ref _attackWideInput);
        if (_player != null)
        {
            if(_rightInput.isPressed != _leftInput.isPressed)
            {
                switch (_rightInput.isPressed)
                {
                    case RightDirection:
                        _player.MoveRight();
                        break;
                    case LeftDirection:
                        _player.MoveLeft();
                        break;
                }
            }
            else
            {
                _player.MoveStop();
            }
            if (_upInput.isPressed == true)
            {
                _player.MoveUp();
            }
            if (_attack1Input.isPressed == true)
            {
                _player.Attack1();
            }
            if (_attack2Input.isPressed == true)
            {
                _player.Attack2();
            }
            if (_attack3Input.isPressed == true)
            {
                _player.Attack3();
            }
            if(_attackWideInput.isPressed == true)
            {
                _player.AttackWide();
            }
            //점프
            if (GetKey(_jumpKeyCodes) == true)
            {
                if (_downInput.isPressed == true)
                {
                    _player.MoveDown();
                }
                else
                {
                    _player.Jump();
                }
            }
            //상호작용
            if (GetKey(_interactionKeyCodes) == true)
            {
                _player.Interact();
            }
            //체력 회복
            if (GetKey(_healKeyCodes) == true)
            {
                _player.Heal();
            }
        }
        _upInput.isPressed = false;
        _downInput.isPressed = false;
        _leftInput.isPressed = false;
        _rightInput.isPressed = false;
        _attack1Input.isPressed = false;
        _attack2Input.isPressed = false;
        _attack3Input.isPressed = false;
        _attackWideInput.isPressed = false;
    }

    /// <summary>
    /// 특정 키보드 입력을 받는 객체에 해당 키가 눌러졌는지 확인하고 결과를 넣어주는 메서드
    /// </summary>
    /// <param name="input"></param>
    private void SetKey(ref Input input)
    {
        int length = input.keyCodes != null ? input.keyCodes.Length : 0;
        for (int i = 0; i < length; i++)
        {
            if (UnityEngine.Input.GetKey(input.keyCodes[i]) == true)
            {
                input.isPressed = true;
                break;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="keyCodes"></param>
    /// <returns></returns>
    private bool GetKey(KeyCode[] keyCodes)
    {
        int length = keyCodes != null ? keyCodes.Length : 0;
        for(int i = 0; i < length; i++)
        {
            if (UnityEngine.Input.GetKeyDown(keyCodes[i]) == true)
            {
                return true;
            }
        }
        return false;
    }
}