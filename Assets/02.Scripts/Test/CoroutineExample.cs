using System.Collections;
using UnityEngine;

public class CoroutineExample : MonoBehaviour
{
    // 매 초마다 : "시작" --> "1초!" --> "2초!" --> "3초!" 센 이후 "나로호 발사!" 로그 출력

    private void Start()
    {
        Debug.Log("시작!");

        StartCoroutine(CountDown_Coroutine());

        Debug.Log("끝!");
    }

    private IEnumerator CountDown_Coroutine()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("3초!");

        yield return new WaitForSeconds(1f);
        Debug.Log("2초!");

        yield return new WaitForSeconds(1f);
        Debug.Log("1초!");

        yield return new WaitForSeconds(1f);
        Debug.Log("나로호 발사!");
    }
}