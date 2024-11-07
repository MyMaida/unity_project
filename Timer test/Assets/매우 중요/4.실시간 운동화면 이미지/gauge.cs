using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SliderController : MonoBehaviour
{
    public Image fillImage;               // 이미지 (filledAmount로 슬라이더처럼 사용)
    public TextMeshProUGUI statusText;    // TextMeshPro 텍스트 컴포넌트
    public float updateInterval = 1f;     // 새로운 목표값으로 업데이트되는 간격 (초 단위)
    public float transitionSpeed = 0.5f;  // 게이지가 목표값으로 이동하는 속도

    private float targetFillAmount = 0f;  // 목표 fillAmount 값

    private void Start()
    {
        // 지정된 시간 간격으로 새로운 목표값을 설정하는 코루틴 시작
        StartCoroutine(UpdateTargetAmount());
    }

    private IEnumerator UpdateTargetAmount()
    {
        while (true)
        {
            // 새로운 목표 filledAmount 값을 랜덤으로 설정 (0 ~ 1)
            targetFillAmount = Random.Range(0f, 1f);

            // 외관 업데이트 (색상과 텍스트 변경)
            UpdateAppearance();

            // 지정된 시간 간격 대기 후 목표값 다시 설정
            yield return new WaitForSeconds(updateInterval);
        }
    }

    private void Update()
    {
        // 현재 fillAmount를 목표값으로 점진적으로 이동
        fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, targetFillAmount, Time.deltaTime * transitionSpeed);
    }

    private void UpdateAppearance()
    {
        // targetFillAmount 값을 5등분하여 구간에 따라 색상과 텍스트 변경
        if (targetFillAmount < 0.2f)
        {
            SetFillColor("#F1483F"); // 빨간색
            statusText.text = "C";
        }
        else if (targetFillAmount < 0.4f)
        {
            SetFillColor("#E88829"); // 주황색
            statusText.text = "B";
        }
        else if (targetFillAmount < 0.6f)
        {
            SetFillColor("#FFDC60"); // 노란색
            statusText.text = "B+";
        }
        else if (targetFillAmount < 0.8f)
        {
            SetFillColor("#60FF68"); // 초록색
            statusText.text = "A";
        }
        else
        {
            SetFillColor("#49C4F0"); // 파란색
            statusText.text = "A+";
        }
    }

    private void SetFillColor(string hexColor)
    {
        // HEX 코드를 Color로 변환하여 이미지 색상 설정
        if (ColorUtility.TryParseHtmlString(hexColor, out Color newColor))
        {
            fillImage.color = newColor;
        }
        else
        {
            Debug.LogWarning("Invalid HEX color code: " + hexColor);
        }
    }
}
