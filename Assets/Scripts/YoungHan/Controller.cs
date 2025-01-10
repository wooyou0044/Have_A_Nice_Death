using System;
using UnityEngine;

/// <summary>
/// Ű���� �Է��� �̿��Ͽ� �÷��̾ ������ �� �ִ� Ŭ����
/// </summary>
public class Controller : MonoBehaviour
{
    //������ �̵� ����
    private const bool RightDirection = true;
    //���� �̵� ����
    private const bool LeftDirection = false;

    //������ ��� �÷��̾�
    public Player _player;

    /// <summary>
    /// Ư�� Ű���� �Է��� �̷�������� Ȯ���ϰ� �� ����� ��ȯ�ϴ� ����ü 
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

    //����Ű ��
    [SerializeField, Header("����Ű ��")]
    private Input _upInput;
    //����Ű ��
    [SerializeField, Header("����Ű ��")]
    private Input _downInput;
    //����Ű ��
    [SerializeField, Header("����Ű ��")]
    private Input _leftInput;
    //����Ű ��
    [SerializeField, Header("����Ű ��")]
    private Input _rightInput;
    //���� 1
    [SerializeField, Header("���� 1")]
    private Input _attack1Input;
    //���� 2
    [SerializeField, Header("���� 2")]
    private Input _attack2Input;
    //���� 3
    [SerializeField, Header("���� 3")]
    private Input _attack3Input;
    //������
    [SerializeField, Header("������")]
    private Input _attackWideInput;
    //����
    [SerializeField, Header("����")]
    private KeyCode[] _jumpKeyCodes;
    //��ȣ �ۿ�
    [SerializeField, Header("��ȣ �ۿ�")]
    private KeyCode[] _interactionKeyCodes;
    //ü�� ȸ��
    [SerializeField, Header("ü�� ȸ��")]
    private KeyCode[] _healKeyCodes;

    //ĵ������ Ȱ��ȭ �� ���� ĵ������ �����ϴ� �������� �ٲ�

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
            //����
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
            //��ȣ�ۿ�
            if (GetKey(_interactionKeyCodes) == true)
            {
                _player.Interact();
            }
            //ü�� ȸ��
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
    /// Ư�� Ű���� �Է��� �޴� ��ü�� �ش� Ű�� ���������� Ȯ���ϰ� ����� �־��ִ� �޼���
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