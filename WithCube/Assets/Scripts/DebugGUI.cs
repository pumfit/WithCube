using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGUI : MonoBehaviour
{
    public static DebugGUI Inst { get; private set; }
    void Awake() => Inst = this;

    [SerializeField] KeyCode enableKey = KeyCode.Tab;
    [SerializeField] bool enable = true;
    [SerializeField] float matrixScale = 1.8f;
    [SerializeField] int maxWidth = 200;

    string innerText;
    Vector2 scrollPos;


    void OnGUI()
    {
        if (!enable)
            return;

        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * matrixScale);

        GUILayout.BeginVertical("Box", GUILayout.Width(maxWidth));
        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.MaxHeight(200));
        GUILayout.Label(innerText);
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    public static void Info(object message) => Inst.ShowLog(message, "white");
    public static void Warn(object message) => Inst.ShowLog(message, "yellow");
    public static void Error(object message) => Inst.ShowLog(message, "red");

    void ShowLog(object message, string colorName)
    {
        string curMessage = string.IsNullOrEmpty(Inst.innerText) ? "" : "\n";
        curMessage += $"<color={colorName}>{message}</color>";

        Inst.innerText += curMessage;
        Inst.scrollPos.y = Mathf.Infinity;
    }

    void Update()
    {
        if (Input.GetKeyDown(enableKey))
            enable = !enable;
    }
}