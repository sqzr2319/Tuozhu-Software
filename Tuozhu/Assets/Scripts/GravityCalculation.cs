using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class GravityCalculation : MonoBehaviour
{
    public HexagonPattern hexagonPattern;
    public HexagonControl hexagonControl;
    private int n;
    private int[,] height;
    private int[,,] polygon;
    private int[] position = new int[]{ 1,1,0 };
    private float radius;
    private float angleOffset;
    private Vector3 center;
    private int status;
    private Vector3 velocity;
    public float smoothTime;
    public float lastAngle;
    void Start()
    {
        n = hexagonPattern.n;
        polygon = new int[3, n, n];
    }
    void Update()
    {
        polygonCalculation();
        positionCalculation();
    }

    private void polygonCalculation()
    {
        height = hexagonPattern.height;
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                polygon[0, i, j] = height[i, j];
            }
        }
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                int temp = -1;
                for (int k = 0; k < n; k++)
                {
                    if (polygon[0, j, k] >= i + 1)
                    {
                        temp++;
                    }
                }
                polygon[2, i, j] = temp + 1;
            }
        }
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                int temp = -1;
                for (int k = 0; k < n; k++)
                {
                    if (polygon[0, k, i] >= j + 1)
                    {
                        temp++;
                    }
                }
                polygon[1, i, j] = temp + 1;
            }
        }
    }

    public void positionCalculation()
    {
        radius = hexagonPattern.Radius;
        angleOffset = Mathf.Deg2Rad * (hexagonPattern.transform.eulerAngles.z);
        int h = polygon[position[0], position[1], position[2]];
        switch (position[0])
        {
            case 0:
                center.x = - radius * position[1] * Mathf.Cos(angleOffset) + radius * position[2] * Mathf.Cos(1.0472f - angleOffset) - radius * (h - 0.5f) * Mathf.Sin(angleOffset - 0.5236f);
                center.y = - radius * position[1] * Mathf.Sin(angleOffset) - radius * position[2] * Mathf.Sin(1.0472f - angleOffset) + radius * (h - 0.5f) * Mathf.Cos(angleOffset - 0.5236f);
                break;
            case 2:
                center.x = - radius * position[2] * Mathf.Cos(angleOffset) + radius * (h - 0.5f) * Mathf.Cos(1.0472f - angleOffset) - radius * position[1] * Mathf.Sin(angleOffset - 0.5236f);
                center.y = - radius * position[2] * Mathf.Sin(angleOffset) - radius * (h - 0.5f) * Mathf.Sin(1.0472f - angleOffset) + radius * position[1] * Mathf.Cos(angleOffset - 0.5236f);
                break;
            case 1:
                center.x = - radius * (h - 0.5f) * Mathf.Cos(angleOffset) + radius * position[1] * Mathf.Cos(1.0472f - angleOffset) - radius * position[2] * Mathf.Sin(angleOffset - 0.5236f);
                center.y = - radius * (h - 0.5f) * Mathf.Sin(angleOffset) - radius * position[1] * Mathf.Sin(1.0472f - angleOffset) + radius * position[2] * Mathf.Cos(angleOffset - 0.5236f);
                break;
        }
        center.z = -2;
        if(lastAngle-angleOffset<0.001f)
            transform.position = Vector3.SmoothDamp(transform.position, center, ref velocity, smoothTime);
        else
            transform.position = center;
        lastAngle = angleOffset;
    }

    public void gravityCalculation()
    {
        status = hexagonControl.status;
        int h=polygon[position[0], position[1], position[2]];
        switch (status)
        {
            case 0:
                switch (position[0])
                {
                    case 0:
                        break;
                    case 1:
                        if (h < n)
                        {
                            position[2] = position[1];
                            position[1] = h;
                            position[0] = status % 3;
                        }
                        break;
                    case 2:
                        if (h < n)
                        {
                            position[1] = position[2];
                            position[2] = h;
                            position[0] = status % 3;
                        }
                        break;
                }
                break;
            case 1:
                switch (position[0])
                {
                    case 0:
                        if (h > 0)
                        {
                            position[1] = position[2];
                            position[2] = h - 1;
                            position[0] = status % 3;
                        }
                        break;
                    case 1:
                        break;
                    case 2:
                        if (h > 0)
                        {
                            position[2] = position[1];
                            position[1] = h - 1;
                            position[0] = status % 3;
                        }
                        break;
                }
                break;
            case 2:
                switch (position[0])
                {
                    case 0:
                        if(h < n)
                        {
                            position[2] = position[1];
                            position[1] = h;
                            position[0] = status % 3;
                        }
                        break;
                    case 1:
                        if(h < n)
                        {
                            position[1] = position[2];
                            position[2] = h;
                            position[0] = status % 3;
                        }
                        break;
                    case 2:
                        break;
                }
                break;
            case 3:
                switch (position[0])
                {
                    case 0:
                        break;
                    case 1:
                        if (h > 0)
                        {
                            position[2] = position[1];
                            position[1] = h - 1;
                            position[0] = status % 3;
                        }
                        break;
                    case 2:
                        if (h > 0)
                        {
                            position[1] = position[2];
                            position[2] = h - 1;
                            position[0] = status % 3;
                        }
                        break;
                }
                break;
            case 4:
                switch (position[0])
                {
                    case 0:
                        if(h < n)
                        {
                            position[1] = position[2];
                            position[2] = h;
                            position[0] = status % 3;
                        }
                        break;
                    case 1:
                        break;
                    case 2:
                        if (h < n)
                        {
                            position[2] = position[1];
                            position[1] = h;
                            position[0] = status % 3;
                        }
                        break;
                }
                break;
            case 5:
                switch (position[0])
                {
                    case 0:
                        if (h > 0)
                        {
                            position[2] = position[1];
                            position[1] = h - 1;
                            position[0] = status % 3;
                        }
                        break;
                    case 1:
                        if (h > 0)
                        {
                            position[1] = position[2];
                            position[2] = h - 1;
                            position[0] = status % 3;
                        }
                        break;
                    case 2:
                        break;
                }
                break;
        }
        positionCalculation();
    }
}
