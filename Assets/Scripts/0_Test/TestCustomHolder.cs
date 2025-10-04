using UnityEngine;
using System.Collections.Generic;

public class TestCustomHolder : MonoBehaviour
{
    public List<List<TestCustom>> data = new List<List<TestCustom>>();

    public TestCustomList testCustomList;

    [Button("import")]
    public bool button1;
    [Button("export")]
    public bool button2;

    public void import()
    {
        data = testCustomList.data;
    }

    public void export()
    {
        testCustomList.data = data;
    }
}
