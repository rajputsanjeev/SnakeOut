using UnityEngine;
using UnityEngine.UI;

namespace ArrowOut.UI
{
    public class ArrowThemePopup : MonoBehaviour
    {
        [Header("Theme Buttons")]
        public Button whiteArrowButton;
        public Button blackArrowButton;
        public Button colorsArrowButton;

        void Start()
        {
            // Subscribe the buttons to the theme change function
            if (whiteArrowButton != null)
                whiteArrowButton.onClick.AddListener(() => SetTheme(ArrowColorMode.White));
                
            if (blackArrowButton != null)
                blackArrowButton.onClick.AddListener(() => SetTheme(ArrowColorMode.Black));
                
            if (colorsArrowButton != null)
                colorsArrowButton.onClick.AddListener(() => SetTheme(ArrowColorMode.Colors));
        }

        private void SetTheme(ArrowColorMode mode)
        {
            if (GridManager.Instance != null)
            {
                GridManager.Instance.SetAllArrowsTheme(mode);
            }
            else
            {
                Debug.LogWarning("GridManager Instance is null. Cannot change arrow theme right now.");
            }
        }
    }
}
