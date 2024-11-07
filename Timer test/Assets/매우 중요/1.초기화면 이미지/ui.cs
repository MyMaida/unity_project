using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SequentialFadeGameObject : MonoBehaviour
{
    public GameObject[] objects; // 순차적으로 나타날 오브젝트들
    public float fadeDuration = 1.0f; // 페이드 인 시간
    public float delayBetweenObjects = 0.5f; // 오브젝트 간의 딜레이 시간
    public float delayBeforeRestart = 2.0f; // 모든 오브젝트가 사라진 후 재시작 전 딜레이

    void Start()
    {
        // 처음에 모든 오브젝트를 비활성화 상태로 설정
        foreach (GameObject obj in objects)
        {
            obj.SetActive(false);
        }

        // 페이드 인/아웃 루틴 시작
        StartCoroutine(SequentialFadeInAndOut());
    }

    IEnumerator SequentialFadeInAndOut()
    {
        while (true) // 반복해서 실행
        {
            // 모든 오브젝트 순차적으로 나타나기 (페이드 인)
            for (int i = 0; i < objects.Length; i++)
            {
                yield return StartCoroutine(FadeIn(objects[i])); // 오브젝트 서서히 나타나기
                yield return new WaitForSeconds(delayBetweenObjects); // 다음 오브젝트 전 대기
            }

            // 모든 오브젝트가 나타난 후 잠시 대기
            yield return new WaitForSeconds(delayBeforeRestart);

            // 모든 오브젝트를 한 번에 사라지게 하기 (즉시 비활성화)
            for (int i = 0; i < objects.Length; i++)
            {
                objects[i].SetActive(false); // 오브젝트 즉시 비활성화
            }

            // 사라진 후 다시 시작하기 전 잠시 대기
            yield return new WaitForSeconds(delayBeforeRestart);
        }
    }

    IEnumerator FadeIn(GameObject obj)
    {
        Image img = obj.GetComponent<Image>(); // Image 컴포넌트 가져오기
        if (img != null)
        {
            float elapsedTime = 0.0f;
            Color color = img.color;
            color.a = 0f; // 처음에 투명
            img.color = color;
            obj.SetActive(true); // 오브젝트 활성화

            // fadeDuration 동안 서서히 알파값을 0에서 1로 변경
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                color.a = Mathf.Clamp01(elapsedTime / fadeDuration); // 알파값 증가
                img.color = color;
                yield return null;
            }
        }
    }
}
