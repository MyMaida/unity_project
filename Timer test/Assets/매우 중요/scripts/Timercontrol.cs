using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AdjustableCountdownTimer : MonoBehaviour
{
    public TextMeshProUGUI initialTimeText;  // 초기 시간을 설정할 텍스트 객체
    public TextMeshProUGUI timerDisplayText; // 타이머가 줄어드는 모습을 표시할 텍스트 객체
    public Image fillImage;                  // 시간이 줄어드는 동안 fill이 줄어들 이미지
    public Button startButton;               // 타이머 시작 버튼
    public Button addTimeButton;             // 30초 추가 버튼
    public Button pauseResumeButton;         // 일시 정지/재개 버튼
    
    private float remainingTime;             // 남은 시간
    private float totalTime;                 // 총 타이머 시간 (initialTime + 추가된 시간)
    private bool isTimerRunning = false;

    private void Start()
    {
        // 버튼 이벤트 추가
        startButton.onClick.AddListener(StartTimer);
        addTimeButton.onClick.AddListener(AddTime);
        pauseResumeButton.onClick.AddListener(TogglePauseResume);
    }

    // 초기 텍스트에 설정된 시간을 기준으로 타이머 세팅
    private void SetTimerFromInitialText()
    {
        if (TryParseTime(initialTimeText.text, out remainingTime))
        {
            totalTime = remainingTime;  // 초기 총 시간을 남은 시간과 동일하게 설정
            UpdateTimerDisplay();
        }
        else
        {
            Debug.LogError("초기 시간 형식이 올바르지 않습니다. 'mm:ss' 형식으로 설정해주세요.");
        }
    }

    // "mm:ss" 형식의 텍스트를 초 단위 시간으로 변환하는 메서드
    private bool TryParseTime(string timeText, out float timeInSeconds)
    {
        timeInSeconds = 0;
        string[] timeParts = timeText.Split(':');
        if (timeParts.Length == 2 && int.TryParse(timeParts[0], out int minutes) && int.TryParse(timeParts[1], out int seconds))
        {
            timeInSeconds = minutes * 60 + seconds;
            return true;
        }
        return false;
    }

    // 타이머 시작 메서드
    public void StartTimer()
    {
        SetTimerFromInitialText();  // 매번 최신의 initialTimeText 값을 사용하여 타이머 초기화
        isTimerRunning = true;
    }

    // 타이머 일시 정지/재개 메서드
    public void TogglePauseResume()
    {
        isTimerRunning = !isTimerRunning;
    }

    // 30초 추가 메서드
    public void AddTime()
    {
        remainingTime += 30f;
        totalTime = remainingTime;  // 추가된 시간에 맞춰 totalTime을 업데이트
        fillImage.fillAmount = 1f;  // 게이지를 다시 가득 채움
        if (!isTimerRunning) StartTimer();  // 타이머가 멈춰있다면 다시 시작
    }

    private void Update()
    {
        if (isTimerRunning && remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;

            if (remainingTime <= 0)
            {
                remainingTime = 0;
                isTimerRunning = false;
                UpdateTimerDisplay();  // 타이머 종료 시 00:00으로 갱신
            }
            else
            {
                UpdateTimerDisplay();
                fillImage.fillAmount = remainingTime / totalTime;  // 이미지 fill을 남은 시간에 비례하여 감소
            }
        }
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerDisplayText.text = $"{minutes:00}:{seconds:00}";
    }
}
