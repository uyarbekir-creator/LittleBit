using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using MaskTransitions;
public class MenuControllerUI : MonoBehaviour
{
    [SerializeField] private Button _playbutton;
    [SerializeField] private Button _quitButton;
    [SerializeField] private Button _htpButton;
    [SerializeField] private Button _creditsButton;

    private void Awake()
    {
        _playbutton.onClick.AddListener(() =>
        {
            TransitionManager.Instance.LoadLevel(Consts.SceneNames.GAME_SCENE);
        });

        _quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
}