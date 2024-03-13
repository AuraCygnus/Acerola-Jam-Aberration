using Aberration.Assets.Scripts;
using Aberration.Assets.Scripts.UI.HUD.Win_Lose;
using Aberration.Assets.Scripts.UI.Menus;
using Aberration.Assets.Scripts.Utils;
using UnityEngine;

namespace Aberration
{
	public class HUD : MonoBehaviour
    {
        [SerializeField]
        private GameState gameState;

        [SerializeField]
        private Team team;

		[Header("UI")]
		[SerializeField]
		private Canvas canvas;
		public Canvas Canvas
		{
			get { return canvas; }
		}

		[SerializeField]
		private RectTransform selectionBoxTransform;
		public RectTransform SelectionBox
		{
			get { return selectionBoxTransform; }
		}

		[SerializeField]
		private RectTransform yeetImage;
		public RectTransform YeetImage
		{
			get { return yeetImage; }
		}

		[SerializeField]
		private LevelWinLosePanel winLosePanel;

		[SerializeField]
		private ActionsPanel actionsPanel;

		[SerializeField]
		private PauseMenu pausePanel;

		private void OnEnable()
		{
			if (team != null)
				Setup(team);
		}

		public void Setup(Team team)
		{
			actionsPanel.Setup(team);
		}

		public void DisplayWin()
		{
			winLosePanel.DisplayWin();
			winLosePanel.gameObject.SetActive(true);
		}

		public void DisplayLose()
		{
			winLosePanel.DisplayLose();
			winLosePanel.gameObject.SetActive(true);
		}

		public void Pause()
		{
			GameSystemUtils.SetPaused(true);
			pausePanel.gameObject.SetActive(true);
		}
    }
}
