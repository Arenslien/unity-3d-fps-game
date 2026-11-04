using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 로그인 화면. 로그인 모드와 회원가입 모드를 바꾸고, 입력을 확인해 AccountStorage에 부탁한다.
public class UI_Login : MonoBehaviour
{
    private enum Mode
    {
        Login,     // 로그인
        Register,  // 회원가입
    }

    [Header("입력 칸")]
    [SerializeField] private TMP_InputField _idInput;
    [SerializeField] private TMP_InputField _passwordInput;
    [SerializeField] private TMP_InputField _passwordConfirmInput;  // 회원가입 모드에서만 보인다.

    [Header("버튼")]
    [SerializeField] private Button _loginButton;         // 로그인 모드: 로그인
    [SerializeField] private Button _gotoRegisterButton;  // 로그인 모드: 회원가입 모드로
    [SerializeField] private Button _registerButton;      // 회원가입 모드: 가입하기
    [SerializeField] private Button _gotoLoginButton;     // 회원가입 모드: 로그인 모드로

    [Header("기타")]
    [SerializeField] private TextMeshProUGUI _messageText;        // 안내 글자
    [SerializeField] private string _gameSceneName = "GameScene";  // 로그인하면 불러올 씬

    [Header("하단 안내문")]
    [SerializeField] private GameObject _loginNotice;     // 로그인 모드: 여유 공간에 들어가는 긴 안내문
    [SerializeField] private GameObject _registerNotice;  // 회원가입 모드: 좁아진 공간에 들어가는 짧은 안내문

    [Header("도장 연출")]
    [SerializeField] private UI_Stamp _stamp;                      // 로그인 결과 도장
    [SerializeField] private float _loadDelay = 1.4f;              // 승인 도장을 보여 준 뒤 씬을 불러오기까지의 시간

    [Header("메시지 타이핑")]
    [SerializeField] private float _typeInterval = 0.035f;  // 글자 하나가 나오는 간격
    [SerializeField] private float _typeStartDelay = 0.12f; // 도장이 찍힌 뒤 타이핑을 시작하기까지의 시간

    private Mode _mode = Mode.Login;
    private bool _isLoading;  // 로그인에 성공해 씬을 불러오는 중
    private Coroutine _typeRoutine;

    private void Start()
    {
        _loginButton.onClick.AddListener(Login);
        _gotoRegisterButton.onClick.AddListener(GotoRegister);
        _registerButton.onClick.AddListener(Register);
        _gotoLoginButton.onClick.AddListener(GotoLogin);

        // 입력을 고치기 시작하면 이전 결과 도장은 지운다.
        _idInput.onValueChanged.AddListener(_ => HideStamp());
        _passwordInput.onValueChanged.AddListener(_ => HideStamp());
        Refresh();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) MoveToNextInput(!(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)));
    }

    // Tab이면 다음 입력칸으로, Shift+Tab이면 이전 입력칸으로 옮긴다. 끝에서는 반대쪽 끝으로 돌아간다.
    private void MoveToNextInput(bool forward)
    {
        if (_isLoading) return;

        TMP_InputField[] inputs = { _idInput, _passwordInput, _passwordConfirmInput };
        GameObject current = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;

        int currentIndex = -1;
        for (int i = 0; i < inputs.Length; i++)
        {
            if (inputs[i].gameObject == current) currentIndex = i;
        }

        // 지금 쓸 수 있는(보이고 조작 가능한) 칸 중에서 다음 칸을 찾는다.
        int step = forward ? 1 : -1;
        int index = currentIndex;
        for (int count = 0; count < inputs.Length; count++)
        {
            index = (index + step + inputs.Length) % inputs.Length;
            TMP_InputField candidate = inputs[index];
            if (!candidate.gameObject.activeInHierarchy || !candidate.IsInteractable()) continue;

            candidate.Select();
            candidate.ActivateInputField();
            return;
        }
    }

    private void HideStamp()
    {
        if (_stamp != null && !_isLoading) _stamp.Hide();
    }

    // 안내 글자를 왼쪽부터 한 글자씩 타이핑하듯 보여 준다.
    private void ShowMessage(string message)
    {
        ClearMessage();
        _typeRoutine = StartCoroutine(Type_Coroutine(message));
    }

    private void ClearMessage()
    {
        if (_typeRoutine != null) StopCoroutine(_typeRoutine);
        _typeRoutine = null;
        _messageText.text = "";
    }

    private IEnumerator Type_Coroutine(string message)
    {
        _messageText.text = message;
        _messageText.maxVisibleCharacters = 0;
        yield return new WaitForSecondsRealtime(_typeStartDelay);

        for (int i = 1; i <= message.Length; i++)
        {
            _messageText.maxVisibleCharacters = i;
            yield return new WaitForSecondsRealtime(_typeInterval);
        }

        _messageText.maxVisibleCharacters = int.MaxValue;
        _typeRoutine = null;
    }

    // 지금 모드에 맞는 칸과 버튼만 보여 준다.
    private void Refresh()
    {
        bool isLogin = _mode == Mode.Login;
        _passwordConfirmInput.gameObject.SetActive(!isLogin);
        _loginButton.gameObject.SetActive(isLogin);
        _gotoRegisterButton.gameObject.SetActive(isLogin);
        _registerButton.gameObject.SetActive(!isLogin);
        _gotoLoginButton.gameObject.SetActive(!isLogin);

        if (_loginNotice != null) _loginNotice.SetActive(isLogin);
        if (_registerNotice != null) _registerNotice.SetActive(!isLogin);
    }

    private void GotoRegister()
    {
        if (_isLoading) return;

        HideStamp();
        ClearInputErrors();
        _mode = Mode.Register;
        ClearMessage();
        Refresh();
    }

    private void GotoLogin()
    {
        if (_isLoading) return;

        HideStamp();
        ClearInputErrors();
        _mode = Mode.Login;
        ClearMessage();
        Refresh();
    }

    // 로그인: 입력을 확인하고, 맞으면 게임 씬을 불러온다.
    private void Login()
    {
        if (_isLoading) return;

        string id = _idInput.text;
        string password = _passwordInput.text;

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password))
        {
            ShowMessage("Please enter your ID and password.");
            PlayEmptyFieldErrors(false);
            return;
        }

        if (!AccountStorage.CheckPassword(id, password))
        {
            // 아이디가 없는지 비밀번호가 틀렸는지는 알려 주지 않는다.
            ShowMessage("Incorrect ID or password.");
            if (_stamp != null) _stamp.Show(false);  // 빨간 거부 도장
            return;
        }

        ClearMessage();
        _isLoading = true;
        if (_stamp != null) _stamp.Show(true);  // 초록 승인 도장
        StartCoroutine(LoadGame_Coroutine());
    }

    // 비어 있는 입력칸만 떨리고 빨간 밑줄이 켜지게 한다.
    // includeConfirm이 true면 비밀번호 확인 칸도 검사한다(회원가입 모드).
    private void PlayEmptyFieldErrors(bool includeConfirm)
    {
        if (string.IsNullOrEmpty(_idInput.text)) PlayInputError(_idInput);
        if (string.IsNullOrEmpty(_passwordInput.text)) PlayInputError(_passwordInput);
        if (includeConfirm && string.IsNullOrEmpty(_passwordConfirmInput.text)) PlayInputError(_passwordConfirmInput);
    }

    // 모드를 바꿀 때 이전에 표시된 빨간 오류 표시(밑줄, 안내 글자 등)를 모든 입력칸에서 지운다.
    private void ClearInputErrors()
    {
        TMP_InputField[] inputs = { _idInput, _passwordInput, _passwordConfirmInput };
        foreach (TMP_InputField input in inputs)
        {
            UI_InputFieldFeedback feedback = input.GetComponent<UI_InputFieldFeedback>();
            if (feedback != null) feedback.ClearError();
        }
    }

    private void PlayInputError(TMP_InputField input)
    {
        UI_InputFieldFeedback feedback = input.GetComponent<UI_InputFieldFeedback>();
        if (feedback != null) feedback.PlayError();
    }

    // 승인 도장이 찍히는 걸 잠깐 보여 준 뒤 게임 씬을 불러온다.
    private IEnumerator LoadGame_Coroutine()
    {
        yield return new WaitForSecondsRealtime(_loadDelay);
        SceneManager.LoadScene(_gameSceneName);
    }

    // 회원가입: 입력을 확인하고, 문제가 없으면 계정을 저장한 뒤 로그인 모드로 돌아간다.
    private void Register()
    {
        string id = _idInput.text;
        string password = _passwordInput.text;
        string passwordConfirm = _passwordConfirmInput.text;

        // 비어 있는 칸은 모두 먼저 표시한다(비밀번호 확인 칸 포함).
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordConfirm))
        {
            bool onlyConfirmEmpty = !string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(password);
            ShowMessage(onlyConfirmEmpty ? "Please confirm your password." : "Please enter your ID and password.");
            PlayEmptyFieldErrors(true);
            return;
        }

        if (password != passwordConfirm)
        {
            ShowMessage("Passwords do not match.");
            PlayInputError(_passwordConfirmInput);
            return;
        }

        if (AccountStorage.Exists(id))
        {
            ShowMessage("That ID is already taken.");
            PlayInputError(_idInput);
            return;
        }

        AccountStorage.Register(id, password);
        GotoLogin();
        ShowMessage("Account created. Please log in.");
    }
}