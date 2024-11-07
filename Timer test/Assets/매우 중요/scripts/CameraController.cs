using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public Transform target; // 카메라가 바라볼 모델 (대상)
    public float moveSpeed = 5f; // 카메라의 좌우 이동 속도
    public float distanceFromTarget = 10f; // 대상과의 거리 설정
    public float heightAboveTarget = 5f; // 카메라가 대상보다 y축으로 얼마나 위에 위치할지
    public float lookHeightAboveTarget = 3f; // 카메라가 바라볼 위치가 대상보다 y축으로 얼마나 위에 있을지

    private float currentHorizontalAngle = 0f;
    private Vector2 lastTouchPosition; // 이전 터치 위치 저장 변수
    private bool isDragging = false; // 마우스 드래그 상태를 확인하는 변수

    // UI 버튼과 이미지 오브젝트
    public Button frontButton;
    public Button rightButton;
    public Button backButton;
    public Button leftButton;
    
    public GameObject image1;
    public GameObject image2;
    public GameObject image3;
    public GameObject image4;

    void Start()
    {
        // 버튼 클릭 이벤트 설정
        frontButton.onClick.AddListener(SetFrontView);
        rightButton.onClick.AddListener(SetRightView);
        backButton.onClick.AddListener(SetBackView);
        leftButton.onClick.AddListener(SetLeftView);

        // 초기 설정
        SetFrontView();
    }

    void Update()
    {
        float horizontalInput = 0f;

        // 키보드 입력 처리
        if (Input.GetAxis("Horizontal") != 0)
        {
            horizontalInput = Input.GetAxis("Horizontal"); // A, D 또는 왼쪽, 오른쪽 화살표
        }

        // 터치 입력 처리
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                lastTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                float deltaX = touch.position.x - lastTouchPosition.x;
                horizontalInput = deltaX * 0.01f; // 터치 민감도 조절

                lastTouchPosition = touch.position;
            }
        }

        // 마우스 입력 처리
        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 버튼 클릭 시
        {
            isDragging = true;
            lastTouchPosition = Input.mousePosition; // 마우스 위치를 저장
        }
        else if (Input.GetMouseButtonUp(0)) // 마우스 왼쪽 버튼을 놓을 때
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector2 mousePosition = Input.mousePosition;
            float deltaX = mousePosition.x - lastTouchPosition.x;
            horizontalInput = deltaX * 0.01f; // 마우스 민감도 조절

            lastTouchPosition = mousePosition;
        }

        // 현재 각도 업데이트 (터치, 키보드, 마우스 입력에 따라)
        currentHorizontalAngle += horizontalInput * moveSpeed * Time.deltaTime;

        // 카메라 위치 계산
        Vector3 direction = new Vector3(Mathf.Sin(currentHorizontalAngle), 0, Mathf.Cos(currentHorizontalAngle));
        Vector3 cameraPosition = target.position - direction * distanceFromTarget;
        cameraPosition.y = target.position.y + heightAboveTarget;

        // 카메라가 바라볼 위치 설정 (대상의 특정 높이 위)
        Vector3 lookAtPosition = new Vector3(target.position.x, target.position.y + lookHeightAboveTarget, target.position.z);

        // 카메라 위치와 회전 설정
        transform.position = cameraPosition;
        transform.LookAt(lookAtPosition);
    }

    // 정면, 우측면, 후면, 좌측면 버튼 클릭 시 호출할 메서드
    public void SetFrontView()
    {
        SetCameraAngle(0f);
        ActivateImage(1);
    }

    public void SetRightView()
    {
        SetCameraAngle(90f);
        ActivateImage(2);
    }

    public void SetBackView()
    {
        SetCameraAngle(180f);
        ActivateImage(3);
    }

    public void SetLeftView()
    {
        SetCameraAngle(270f);
        ActivateImage(4);
    }

    private void SetCameraAngle(float angle)
    {
        currentHorizontalAngle = angle * Mathf.Deg2Rad;

        Vector3 direction = new Vector3(Mathf.Sin(currentHorizontalAngle), 0, Mathf.Cos(currentHorizontalAngle));
        Vector3 cameraPosition = target.position - direction * distanceFromTarget;
        cameraPosition.y = target.position.y + heightAboveTarget;

        transform.position = cameraPosition;
        transform.LookAt(new Vector3(target.position.x, target.position.y + lookHeightAboveTarget, target.position.z));
    }

    private void ActivateImage(int imageNumber)
    {
        // 모든 이미지를 비활성화
        image1.SetActive(false);
        image2.SetActive(false);
        image3.SetActive(false);
        image4.SetActive(false);

        // 선택된 이미지만 활성화
        switch (imageNumber)
        {
            case 1:
                image1.SetActive(true);
                image2.SetActive(false);
                image3.SetActive(false);
                image4.SetActive(false);
                break;
            case 2:
                image1.SetActive(false);
                image2.SetActive(true);
                image3.SetActive(false);
                image4.SetActive(false);
                break;
            case 3:
                image1.SetActive(false);
                image2.SetActive(false);
                image3.SetActive(true);
                image4.SetActive(false);
                break;
            case 4:
                image1.SetActive(false);
                image2.SetActive(false);
                image3.SetActive(false);
                image4.SetActive(true);
                break;
        }
    }
}
