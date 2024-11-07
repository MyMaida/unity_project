using UnityEngine;
using TMPro;

public class TimePercentageDisplay : MonoBehaviour
{
    public TextMeshProUGUI initialTimeText;       // 기준 시간을 표시하는 텍스트 (mm:ss 형식)
    public TextMeshProUGUI percentageDisplayText; // %로 계산된 결과를 표시할 텍스트

    private float totalSecondsInAnHour = 3600f;   // 1시간을 초로 변환 (3600초)

    private void Update()
    {
        UpdatePercentage();
    }

    private void UpdatePercentage()
    {
        // initialTimeText의 시간을 초 단위로 변환
        if (TryParseTime(initialTimeText.text, out float timeInSeconds))
        {
            // 퍼센트 계산
            int percentage = Mathf.Clamp(Mathf.FloorToInt((timeInSeconds / totalSecondsInAnHour) * 100), 0, 100);

            // 퍼센트 표시
            percentageDisplayText.text = $"{percentage}%";
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
}
