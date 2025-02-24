using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;

public class GravityCalculation : MonoBehaviour
{
    private Vector3 center= new Vector3(0, 0, 0);
    public HexagonPattern hexagonPattern;
    public HexagonControl hexagonControl;
    private int[,] height;
    private int n;
    private float radius;
    private float angleOffset;
    private int x, y, h;
    private float x2, y2, h2;
    private int status;

    void CenterCalculation()
    {
        
    }
    void Start()
    {
        
    }
    void Update()
    {
        Vector3 bEulerAngles = hexagonPattern.transform.eulerAngles;
        status = hexagonControl.status;
        x = 0;
        y = 0;
        height = hexagonPattern.height;
        n = hexagonPattern.n;
        h = height[x, y];
        x2 = x;
        y2 = y;
        h2 = h;
        radius = 0.5f * 8 / n; // �뾶���������ŵ���
        angleOffset = Mathf.Deg2Rad*(bEulerAngles.z); // ˳ʱ����ת30��
        switch (status)
        {
            case 0:
                h = height[x, y];
                h2 = h + 0.5f;
                x2 = x;
                y2 = y;
                break;
            case 1:
                x2 = x - 0.5f;
                y2 = y;
                h2 = h;
                break;
            case 2:
                y2 = y + 0.5f;
                x2 = x;
                h2 = h;
                break;
            case 3:

                h2 = h - 0.5f;
                x2 = x;
                y2 = y;
                break;
            case 4:
                x2 = x + 0.5f;
                y2 = y;
                h2 = h;
                break;
            case 5:
                y2 = y - 0.5f;
                x2 = x;
                h2 = h;
                break;
        }
        center.x = radius * y2 * Mathf.Cos(1.0472f - angleOffset) - radius * x2 * Mathf.Cos(angleOffset) - radius * (h2 - 1) * Mathf.Sin(angleOffset - 0.5236f);
        center.y = -radius * y2 * Mathf.Sin(1.0472f - angleOffset) - radius * x2 * Mathf.Sin(angleOffset) + radius * (h2 - 1) * Mathf.Cos(angleOffset - 0.5236f);
        transform.position = center;
    }
}
