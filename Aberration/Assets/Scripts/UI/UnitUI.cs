using UnityEngine;
using UnityEngine.UI;

namespace Aberration
{
	public class UnitUI : MonoBehaviour
    {
        [SerializeField]
        private Image selectedIndicator;

        [SerializeField]
        private Slider hpBar;

        [SerializeField]
        private Color allySelectionColor;

        [SerializeField]
        private Color enemySelectionColor;

        public void SetHp(float value, float maxValue)
		{
            hpBar.minValue = 0;
            hpBar.maxValue = maxValue;
            hpBar.value = value;
        }

        public void SetHpVisible(bool isVisible)
		{
            hpBar.gameObject.SetActive(isVisible);
		}

        public void SetSelected(bool isSelected, bool isAlly)
		{
            selectedIndicator.gameObject.SetActive(isSelected);
            selectedIndicator.color = (isAlly) ? allySelectionColor : enemySelectionColor;
        }
    }
}
