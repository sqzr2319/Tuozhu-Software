using UnityEngine;

public class HexagonControl : MonoBehaviour
{
    public int n;
    public int status;
    // 目标旋转角度（只记录Z轴）
    private float targetZ;
    // 控制旋转平滑度
    public float rotationSpeed = 5f;

    void Start()
    {
        // 场景加载时初始化n
        n = 0;
        // 记录当前Z轴的初始值
        targetZ = transform.eulerAngles.z;
        status = (int)(transform.eulerAngles.z - 30) / 60;
    }

    void Update()
    {
        // 按A键逆时针旋转60度
        if (Input.GetKeyDown(KeyCode.A))
        {
            targetZ -= 60f;
        }
        // 按D键顺时针旋转60度
        if (Input.GetKeyDown(KeyCode.D))
        {
            targetZ += 60f;
        }
        // 按S键n加1
        if (Input.GetKeyDown(KeyCode.S))
        {
            n++;
            status = (((int)(targetZ + 30) / 60) + 65) % 6;
        }

        // 使用LerpAngle实现平滑旋转
        Vector3 currentAngles = transform.eulerAngles;
        currentAngles.z = Mathf.LerpAngle(currentAngles.z, targetZ, Time.deltaTime * rotationSpeed);
        transform.eulerAngles = currentAngles;
    }

    void OnGUI()
    {
        // 增大字体显示n的值
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 36;

        GUI.Label(new Rect(10, 10, 300, 40), "n = " + n, style);
    }
}
