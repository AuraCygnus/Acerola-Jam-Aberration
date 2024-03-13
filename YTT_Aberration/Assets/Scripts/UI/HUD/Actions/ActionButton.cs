using Aberration.Assets.Scripts;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Aberration
{
	public class ActionButton : MonoBehaviour
    {
        [SerializeField]
        private Button button;

        [SerializeField]
        private TextMeshProUGUI cooldownText;

        [SerializeField]
        private AudioSource audioSource;

        [SerializeField]
        private AudioClip confirmSound;

        [SerializeField]
        private AudioClip cancelSound;

        [SerializeField]
        private float updateRateSecs = 0.25f;

        [Header("Icon")]
        [SerializeField]
        private Image iconImage;

        [SerializeField]
        private Color normalIconColor;

        [SerializeField]
        private Color cooldownIconColor;

        [SerializeField]
        private Color selectedIconColor;

        [Header("Frame")]
        [SerializeField]
        private Image frameImage;

        [SerializeField]
        private Color normalFrameColor;

        [SerializeField]
        private Color cooldownFrameColor;

        [SerializeField]
        private Color selectedFrameColor;

        private Team team;
        private TeamActionState actionState;

        private bool isUpdating;

		protected void Awake()
		{
            button.onClick.AddListener(OnActionClick);
		}

		public void Setup(Team team, TeamActionState actionState)
		{
            this.team = team;
            this.actionState = actionState;
        }

		private void OnEnable()
		{
            isUpdating = true;
            StartCoroutine(UpdateTick());
		}

		protected IEnumerator UpdateTick()
        {
            while (isUpdating)
			{
                yield return new WaitForSeconds(updateRateSecs);

                UpdateUI();
            }
        }

        private void UpdateUI()
		{
            if (!actionState.CanSelect())
			{
                button.interactable = false;

                int remainingTime = Mathf.CeilToInt(actionState.CooldownTime - Time.time);
                cooldownText.SetText(remainingTime.ToString());
                cooldownText.enabled = true;

                iconImage.color = cooldownIconColor;
                frameImage.color = cooldownFrameColor;
            }
            else
			{
                button.interactable = true;

                cooldownText.enabled = false;

                if (actionState.IsSelected)
                {
                    iconImage.color = selectedIconColor;
                    frameImage.color = selectedFrameColor;
                }
                else
                {
                    iconImage.color = normalIconColor;
                    frameImage.color = normalFrameColor;
                }
            }
		}

        private void OnActionClick()
		{
            audioSource.PlayOneShot(confirmSound, 0.5f);
            team.SetSelectedAction(actionState);
        }
    }
}
