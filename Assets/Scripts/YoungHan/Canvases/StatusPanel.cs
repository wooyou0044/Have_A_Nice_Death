using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class StatusPanel : MonoBehaviour
{
    [SerializeField, Header("������ �پ��� �ӵ�"), Range(0.1f, 10f)]
    private float _wainingValue = 1f;
    [SerializeField, Header("ü�¹� �̹���")]
    private Image _lifeImage;
    [SerializeField, Header("�ҿ�� �̹���")]
    private Text _soularyText;
    [SerializeField, Header("������̿� �̹���")]
    private Text _prismiumText;

    public byte soulary
    {
        set
        {
            if(_soularyText != null)
            {
                _soularyText.text = value.ToString();
            }
        }
    }

    public byte prismium
    {
        set
        {
            if (_prismiumText != null)
            {
                _prismiumText.text = value.ToString();
            }
        }
    }

    public void Set(Player player)
    {
        if (player != null)
        {
            if (_lifeImage != null)
            {
                float life = player.maxLife > 0 ? (float)player.remainLife / player.maxLife : 0;
                _lifeImage.DOKill();
                _lifeImage.DOFillAmount(life, _wainingValue);
            }
        }
    }
}