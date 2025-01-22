using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class StatusPanel : MonoBehaviour
{
    [SerializeField]
    private Image _lifeImage;
    [SerializeField]
    private Text _soularyText;
    [SerializeField]
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
                float life = player.maxLife > 0 ? player.remainLife / player.maxLife : 0;
                _lifeImage.DOKill();
                _lifeImage.DOFillAmount(life, 2f);
            }
        }
    }
}