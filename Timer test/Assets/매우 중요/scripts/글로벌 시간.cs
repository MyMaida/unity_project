using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro 네임스페이스 추가
using System; // DateTime 사용을 위해 필요

public class 글로벌시간 : MonoBehaviour
{
    public TextMeshProUGUI timeText; // 시간을 표시할 TextMeshPro 텍스트

    void Update()
    {
        // 현재 시간 가져오기
        DateTime currentTime = DateTime.Now;

        // "tt hh : mm" 형식으로 시간 포맷팅
        string formattedTime = currentTime.ToString("tt hh : mm");

        // TextMeshProUGUI 컴포넌트에 시간 설정
        timeText.text = formattedTime;
    }
}
