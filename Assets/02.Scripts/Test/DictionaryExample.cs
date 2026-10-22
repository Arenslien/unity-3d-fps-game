using System.Collections.Generic;
using UnityEngine;

public class DictionaryExample : MonoBehaviour
{
    // 딕셔너리
    private void Start()
    {
        Dictionary<string, int> students = new Dictionary<string, int>()
        {
            { "정성훈", 27 },
            { "김민석", 27 },
            { "남경원", 33 }
        };
        Debug.Log(students["정성훈"]);

        students["남경원"] = 23;
        students.Add("김호준", 26);
        students.Remove("정성훈");

        bool has = students.ContainsKey("정성훈");

        foreach (KeyValuePair<string, int> item in students)
        {
            Debug.Log($"Key: {item.Key}, Value: {item.Value}");
        }
    }
}