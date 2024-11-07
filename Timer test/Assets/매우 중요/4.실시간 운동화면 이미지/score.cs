using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class AverageFillAmountController : MonoBehaviour
{
    public Image fillImage1;              // 첫 번째 이미지
    public Image fillImage2;              // 두 번째 이미지
    public Image fillImage3;              // 세 번째 이미지
    public TextMeshProUGUI averageText;   // 평균 fillAmount를 표시할 TextMeshPro 텍스트

    public float updateInterval = 1f;     // 업데이트 간격 (초 단위)

    private void Start()
    {
        // 지정된 간격으로 평균값을 업데이트하는 코루틴 시작
        StartCoroutine(UpdateAverageFillAmount());
    }

    private IEnumerator UpdateAverageFillAmount()
    {
        while (true)
        {
            // fillAmount 값의 평균 계산
            float averageFill = (fillImage1.fillAmount + fillImage2.fillAmount + fillImage3.fillAmount) / 3f;

            // 퍼센트 값으로 변환 후 소수점 1자리까지 표시
            averageText.text = (averageFill * 100f).ToString("F1");

            // 지정된 시간 간격 대기
            yield return new WaitForSeconds(updateInterval);
        }
    }
}
