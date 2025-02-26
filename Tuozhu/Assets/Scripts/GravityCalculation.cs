using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.SceneManagement;

public class GravityCalculation : MonoBehaviour
{
    public HexagonPattern hexagonPattern;
    public HexagonControl hexagonControl;
    public DestinationControl destinationControl;
    private int n;
    private int[,] height;
    private int[,,] polygon;
    public int[] position;
    public int[] destination;
    private float radius;
    private float angleOffset;
    private Vector3 center;
    private Vector3 destcenter;
    private int status;
    private Vector3 velocity;
    public float smoothTime;
    public float lastAngle;
    void Start()
    {
        n = hexagonPattern.n;
        polygon = new int[3, n, n];
        position= new int[]{ 0,0,0 };
        destination = new int[] { 0,1,2 };
    }
    void Update()
    {
        polygonCalculation();
        positionCalculation();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("LevelSelectScene");
        }
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
        if(lastAngle-angleOffset<0.001f&&lastAngle-angleOffset>-0.001f)
            transform.position = Vector3.SmoothDamp(transform.position, center, ref velocity, smoothTime);
        else
            transform.position = center;
        lastAngle = angleOffset;
        if(position[0]==destination[0]&&position[1]==destination[1]&&position[2]==destination[2])
        {
            StartCoroutine(LoadWinningSceneDelay());
        }
    }
    private System.Collections.IEnumerator LoadWinningSceneDelay()
    {
        yield return new WaitForSeconds(0.3f);
        SceneManager.LoadScene("WinningScene");
    }

    public void destinationCalculation()
    {
        int h=polygon[destination[0], destination[1], destination[2]];
        switch(destination[0])
        {
            case 0:
                destcenter.x = - radius * destination[1] * Mathf.Cos(angleOffset) + radius * destination[2] * Mathf.Cos(1.0472f - angleOffset) - radius * (h - 0.5f) * Mathf.Sin(angleOffset - 0.5236f);
                destcenter.y = - radius * destination[1] * Mathf.Sin(angleOffset) - radius * destination[2] * Mathf.Sin(1.0472f - angleOffset) + radius * (h - 0.5f) * Mathf.Cos(angleOffset - 0.5236f);
                break;
            case 2:
                destcenter.x = - radius * destination[2] * Mathf.Cos(angleOffset) + radius * (h - 0.5f) * Mathf.Cos(1.0472f - angleOffset) - radius * destination[1] * Mathf.Sin(angleOffset - 0.5236f);
                destcenter.y = - radius * destination[2] * Mathf.Sin(angleOffset) - radius * (h - 0.5f) * Mathf.Sin(1.0472f - angleOffset) + radius * destination[1] * Mathf.Cos(angleOffset - 0.5236f);
                break;
            case 1:
                destcenter.x = - radius * (h - 0.5f) * Mathf.Cos(angleOffset) + radius * destination[1] * Mathf.Cos(1.0472f - angleOffset) - radius * destination[2] * Mathf.Sin(angleOffset - 0.5236f);
                destcenter.y = - radius * (h - 0.5f) * Mathf.Sin(angleOffset) - radius * destination[1] * Mathf.Sin(1.0472f - angleOffset) + radius * destination[2] * Mathf.Cos(angleOffset - 0.5236f);
                break;
        }
        destcenter.z = -2;
        destinationControl.transform.position = destcenter;
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
                        hexagonControl.n--;
                        break;
                    case 1:
                        if (h < n)
                        {
                            position[2] = position[1];
                            position[1] = h;
                            position[0] = status % 3;
                        }
                        else{
                            SceneManager.LoadScene("LoseScene");
                        }
                        break;
                    case 2:
                        if (h < n)
                        {
                            position[1] = position[2];
                            position[2] = h;
                            position[0] = status % 3;
                        }
                        else{
                            SceneManager.LoadScene("LoseScene");
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
                        else{
                            SceneManager.LoadScene("LoseScene");
                        }
                        break;
                    case 1:
                        hexagonControl.n--;
                        break;
                    case 2:
                        if (h > 0)
                        {
                            position[2] = position[1];
                            position[1] = h - 1;
                            position[0] = status % 3;
                        }
                        else{
                            SceneManager.LoadScene("LoseScene");
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
                        else{
                            SceneManager.LoadScene("LoseScene");
                        }
                        break;
                    case 1:
                        if(h < n)
                        {
                            position[1] = position[2];
                            position[2] = h;
                            position[0] = status % 3;
                        }
                        else{
                            SceneManager.LoadScene("LoseScene");
                        }
                        break;
                    case 2:
                        hexagonControl.n--;
                        break;
                }
                break;
            case 3:
                switch (position[0])
                {
                    case 0:
                        hexagonControl.n--;
                        break;
                    case 1:
                        if (h > 0)
                        {
                            position[2] = position[1];
                            position[1] = h - 1;
                            position[0] = status % 3;
                        }
                        else{
                            SceneManager.LoadScene("LoseScene");
                        }
                        break;
                    case 2:
                        if (h > 0)
                        {
                            position[1] = position[2];
                            position[2] = h - 1;
                            position[0] = status % 3;
                        }
                        else{
                            SceneManager.LoadScene("LoseScene");
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
                        else{
                            SceneManager.LoadScene("LoseScene");
                        }
                        break;
                    case 1:
                        hexagonControl.n--;
                        break;
                    case 2:
                        if (h < n)
                        {
                            position[2] = position[1];
                            position[1] = h;
                            position[0] = status % 3;
                        }
                        else{
                            SceneManager.LoadScene("LoseScene");
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
                        else{
                            SceneManager.LoadScene("LoseScene");
                        }
                        break;
                    case 1:
                        if (h > 0)
                        {
                            position[1] = position[2];
                            position[2] = h - 1;
                            position[0] = status % 3;
                        }
                        else{
                            SceneManager.LoadScene("LoseScene");
                        }
                        break;
                    case 2:
                        hexagonControl.n--;
                        break;
                }
                break;
        }
        positionCalculation();
    }
}
