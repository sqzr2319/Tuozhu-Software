using UnityEngine;

public class HexagonControl : MonoBehaviour
{
    public GravityCalculation gravityCalculation;
    public HexagonPattern hexagonPattern;
    public int n;
    public int status;
    public int tempstatus;
    // Ŀ����ת�Ƕȣ�ֻ��¼Z�ᣩ
    private float targetZ;
    // ������תƽ����
    public float rotationSpeed = 5f;

    void Start()
    {
        // ��������ʱ��ʼ��n
        n = 0;
        // ��¼��ǰZ��ĳ�ʼֵ
        targetZ = transform.eulerAngles.z;
        status = (int)(transform.eulerAngles.z + 0.01 - 30) / 60;
        tempstatus = status;
    }

    void Update()
    {
        if(!hexagonPattern.isFaded)
        {
            // ��A����ʱ����ת60��
            if (Input.GetKeyDown(KeyCode.A))
            {
                targetZ -= 60f;
                tempstatus= (tempstatus + 5) % 6;
            }
            // ��D��˳ʱ����ת60��
            if (Input.GetKeyDown(KeyCode.D))
            {
                targetZ += 60f;
                tempstatus = (tempstatus + 1) % 6;
            }
            // ��S��n��1
            if (Input.GetKeyDown(KeyCode.S))
            {
                n++;
                status = tempstatus;
                gravityCalculation.gravityCalculation();
            }

            // ʹ��LerpAngleʵ��ƽ����ת
            Vector3 currentAngles = transform.eulerAngles;
            currentAngles.z = Mathf.LerpAngle(currentAngles.z, targetZ, Time.deltaTime * rotationSpeed);
            transform.eulerAngles = currentAngles;
        }
        
    }

    void OnGUI()
    {
        // ����������ʾn��ֵ
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 36;

        GUI.Label(new Rect(10, 10, 300, 60), "���������" + n, style);
    }
}
