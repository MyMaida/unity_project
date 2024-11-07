using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TimerDisplay : MonoBehaviour
{
    public TextMeshProUGUI timerText;  // TextMeshPro 텍스트 객체 연결
    public Button startButton;         // 시작 버튼 연결
    public Button stopButton;          // 정지 버튼 연결
    
    private float elapsedTime;
    private bool isTimerRunning = false;

    private void Start()
    {
        // 버튼에 클릭 이벤트 리스너 추가
        startButton.onClick.AddListener(StartTimer);
        stopButton.onClick.AddListener(StopTimer);
        
        // 초기에는 타이머가 멈춰 있고, 경과 시간은 0입니다.
        elapsedTime = 0f;
        UpdateTimerText();
    }

    public void StartTimer()
    {
        elapsedTime = 0f;
        isTimerRunning = true;
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerText();
        }
    }

    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void StopTimer()
    {
        isTimerRunning = false;
    }
}
