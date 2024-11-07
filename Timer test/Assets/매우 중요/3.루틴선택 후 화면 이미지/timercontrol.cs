using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro를 사용하기 위한 네임스페이스 추가

public class TimerControl : MonoBehaviour
{
    public TextMeshProUGUI timerText; // 시간을 표시할 TextMeshProUGUI
    public Button addButton; // 10초를 증가시킬 버튼
    public Button subtractButton; // 10초를 감소시킬 버튼

    private int totalSeconds = 0; // 현재 시간을 초로 저장

    void Start()
    {
        // 초기 시간 설정
        UpdateTimerDisplay();

        // 버튼 클릭 시 이벤트 연결
        addButton.onClick.AddListener(IncreaseTime);
        subtractButton.onClick.AddListener(DecreaseTime);
    }

    // 10초 증가
    void IncreaseTime()
    {
        totalSeconds += 10;
        UpdateTimerDisplay();
    }

    // 10초 감소 (최소 0초까지)
    void DecreaseTime()
    {
        totalSeconds = Mathf.Max(0, totalSeconds - 10); // 시간이 0보다 작아지지 않도록 설정
        UpdateTimerDisplay();
    }

    // 타이머를 00:00 형식으로 업데이트
    void UpdateTimerDisplay()
    {
        int minutes = totalSeconds / 60; // 분 계산
        int seconds = totalSeconds % 60; // 초 계산

        // 시간을 00:00 형식으로 표시
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
