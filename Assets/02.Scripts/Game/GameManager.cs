using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Ready,
    Playing,
    Pause,
    GameOver
}

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    private GameState _state = GameState.Ready;
    public GameState State => _state;

    [SerializeField] private TextMeshProUGUI _stateText;

    [Header("컴포넌트")]
    [SerializeField] private Animator _playerAnimator;
    [SerializeField] private CursorManager _cursorManager;
    [SerializeField] private GameObject _optionPopup;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }

    private void Start()
    {
        StartCoroutine(StartSequence_Coroutine());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_state == GameState.Playing)
            {
                Pause();
            }
            else if (_state == GameState.Pause)
            {
                Resume();
            }
        }
    }

    private IEnumerator StartSequence_Coroutine()
    {
        _stateText.gameObject.SetActive(true);
        _stateText.text = "준비!";
        yield return new WaitForSeconds(2f);
        _stateText.text = "시작!";
        yield return new WaitForSeconds(0.5f);
        _stateText.gameObject.SetActive(false);

        _state = GameState.Playing;
    }

    public void GameOver()
    {
        _state = GameState.GameOver;

        _stateText.gameObject.SetActive(true);
        _stateText.text = "게임오버";
        _stateText.color = Color.red;

        _playerAnimator.SetFloat("MoveMotion", 0f);
    }

    public void Pause()
    {
        if (_state != GameState.Playing) return;

        _state = GameState.Pause;
        Time.timeScale = 0f;
        _optionPopup.SetActive(true);
        _cursorManager.UnlockCursor();
    }

    public void Resume()
    {
        if (_state != GameState.Pause) return;

        _state = GameState.Playing;
        Time.timeScale = 1f;
        _optionPopup.SetActive(false);
        _cursorManager.LockCursor();
    }

    public void Restart()
    {
        Debug.Log("[GameManager.cs] - Restart");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        Debug.Log("[GameManager.cs] - Quit");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}