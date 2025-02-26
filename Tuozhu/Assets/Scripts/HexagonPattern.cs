using UnityEngine;

public class HexagonPattern : MonoBehaviour
{
    [Range(3, 10)]
    public int n = 4;

    public int[,] height=new int[4,4]{{3,3,3,2},{3,3,2,0},{2,2,2,0},{2,1,1,0}};

    private Vector3[] vertices = new Vector3[6];
    private Vector3 center;

    public Color colorBlue = new Color(31f / 255f, 150f / 255f, 192f / 255f);
    public Color colorLightBlue = new Color(142f / 255f, 205f / 255f, 223f / 255f);
    public Color colorDarkBlue = new Color(19f / 255f, 102f / 255f, 132f / 255f);

    public float Radius;

    private Material lineMaterial;

    void Start()
    {
        lineMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
        lineMaterial.hideFlags = HideFlags.HideAndDontSave;
        lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        lineMaterial.SetInt("_ZWrite", 0);
    }

    void OnRenderObject()
    {
        CalculateVertices();
        DrawHexagon();
        DrawTriangles();
        DrawLines();
        for(int i=0;i<4;i++)
        {
            for(int j=0;j<4;j++)
            {
                if(height[i,j]>0)
                {
                    for(int k=1;k<=height[i,j];k++)
                    {
                        CalculateLittleHexagons(i,j,k);
                        DrawLittleHexagons();
                        DrawLittleTriangles();
                        DrawLittleLines();
                    }
                }
            }
        }
    }

    void CalculateVertices()
    {
        center = transform.position;
        float radius = 0.5f * transform.localScale.x;
        float angleOffset = Mathf.Deg2Rad * (transform.eulerAngles.z);

        for (int i = 0; i < 6; i++)
        {
            float angle = Mathf.Deg2Rad * 60f * i;
            vertices[i] = new Vector3(
                center.x + radius * Mathf.Cos(angle + angleOffset),
                center.y + radius * Mathf.Sin(angle + angleOffset),
                center.z
            );
        }
    }

    void DrawHexagon()
    {
        lineMaterial.SetPass(0);
        GL.Begin(GL.TRIANGLES);
        GL.Color(Color.white);

        for (int i = 0; i < 6; i++)
        {
            GL.Vertex(center);
            GL.Vertex(vertices[i]);
            GL.Vertex(vertices[(i + 1) % 6]);
        }

        GL.End();
    }

    void DrawTriangles()
    {
        lineMaterial.SetPass(0);


        GL.Begin(GL.TRIANGLES);
        GL.Color(colorBlue);
        GL.Vertex(center);
        GL.Vertex(vertices[5]);
        GL.Vertex(vertices[0]);
        GL.End();

        GL.Begin(GL.TRIANGLES);
        GL.Color(colorBlue);
        GL.Vertex(center);
        GL.Vertex(vertices[0]);
        GL.Vertex(vertices[1]);
        GL.End();

        GL.Begin(GL.TRIANGLES);
        GL.Color(colorLightBlue);
        GL.Vertex(center);
        GL.Vertex(vertices[3]);
        GL.Vertex(vertices[4]);
        GL.End();

        GL.Begin(GL.TRIANGLES);
        GL.Color(colorLightBlue);
        GL.Vertex(center);
        GL.Vertex(vertices[4]);
        GL.Vertex(vertices[5]);
        GL.End();

        GL.Begin(GL.TRIANGLES);
        GL.Color(colorDarkBlue);
        GL.Vertex(center);
        GL.Vertex(vertices[1]);
        GL.Vertex(vertices[2]);
        GL.End();

        GL.Begin(GL.TRIANGLES);
        GL.Color(colorDarkBlue);
        GL.Vertex(center);
        GL.Vertex(vertices[2]);
        GL.Vertex(vertices[3]);
        GL.End();
    }

    void DrawLines()
    {
        lineMaterial.SetPass(0);
        GL.Begin(GL.LINES);
        GL.Color(Color.white);

        DrawLinesInTriangle(vertices[0], vertices[1], vertices[5], center, n - 1);

        DrawLinesInTriangle(vertices[1], vertices[2], center, vertices[3], n - 1);

        DrawLinesInTriangle(vertices[2], vertices[3], vertices[1], center, n - 1);

        DrawLinesInTriangle(vertices[3], vertices[4], center, vertices[5], n - 1);

        DrawLinesInTriangle(vertices[4], vertices[5], vertices[3], center, n - 1);

        DrawLinesInTriangle(vertices[5], vertices[0], center, vertices[1], n - 1);

        GL.End();
    }

    void DrawLinesInTriangle(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, int lineCount)
    {
        for (int i = 0; i <= lineCount+1; i++)
        {
            float t = (float)i / (lineCount + 1);

            Vector3 pointA = Vector3.Lerp(v1, v2, t);
            Vector3 pointB = Vector3.Lerp(v3, v4, t);
            GL.Vertex(pointA);
            GL.Vertex(pointB);
        }
    }

    public void CalculateLittleHexagons(int x,int y,int h)
    {
        center = transform.position;
        float radius = 0.5f * transform.localScale.x/n;
        Radius = radius;
        float angleOffset = Mathf.Deg2Rad * (transform.eulerAngles.z);
        center.x=center.x+radius*y*Mathf.Cos(1.0472f-angleOffset)-radius*x*Mathf.Cos(angleOffset)-radius*(h-1)*Mathf.Sin(angleOffset-0.5236f);
        center.y=center.y-radius*y*Mathf.Sin(1.0472f-angleOffset)-radius*x*Mathf.Sin(angleOffset)+radius*(h-1)*Mathf.Cos(angleOffset-0.5236f);

        for (int i = 0; i < 6; i++)
        {
            float angle = Mathf.Deg2Rad * 60f * i;
            vertices[i] = new Vector3(
                center.x + radius * Mathf.Cos(angle + angleOffset),
                center.y + radius * Mathf.Sin(angle + angleOffset),
                center.z
            );
        }
    }

    void DrawLittleHexagons()
    {
        lineMaterial.SetPass(0);
        GL.Begin(GL.TRIANGLES);
        GL.Color(Color.white);

        for (int i = 0; i < 6; i++)
        {
            GL.Vertex(center);
            GL.Vertex(vertices[i]);
            GL.Vertex(vertices[(i + 1) % 6]);
        }

        GL.End();
    }

    void DrawLittleTriangles()
    {
        lineMaterial.SetPass(0);

        GL.Begin(GL.TRIANGLES);
        GL.Color(colorBlue);
        GL.Vertex(center);
        GL.Vertex(vertices[2]);
        GL.Vertex(vertices[3]);
        GL.End();

        GL.Begin(GL.TRIANGLES);
        GL.Color(colorBlue);
        GL.Vertex(center);
        GL.Vertex(vertices[3]);
        GL.Vertex(vertices[4]);
        GL.End();

        GL.Begin(GL.TRIANGLES);
        GL.Color(colorLightBlue);
        GL.Vertex(center);
        GL.Vertex(vertices[0]);
        GL.Vertex(vertices[1]);
        GL.End();

        GL.Begin(GL.TRIANGLES);
        GL.Color(colorLightBlue);
        GL.Vertex(center);
        GL.Vertex(vertices[1]);
        GL.Vertex(vertices[2]);
        GL.End();

        GL.Begin(GL.TRIANGLES);
        GL.Color(colorDarkBlue);
        GL.Vertex(center);
        GL.Vertex(vertices[4]);
        GL.Vertex(vertices[5]);
        GL.End();

        GL.Begin(GL.TRIANGLES);
        GL.Color(colorDarkBlue);
        GL.Vertex(center);
        GL.Vertex(vertices[5]);
        GL.Vertex(vertices[0]);
        GL.End();
    }
    
    void DrawLittleLines()
    {
        lineMaterial.SetPass(0);
        GL.Begin(GL.LINES);
        GL.Color(Color.white);

        GL.Vertex(vertices[0]);
        GL.Vertex(vertices[1]);

        GL.Vertex(vertices[1]);
        GL.Vertex(vertices[2]);

        GL.Vertex(vertices[2]);
        GL.Vertex(vertices[3]);

        GL.Vertex(vertices[3]);
        GL.Vertex(vertices[4]);

        GL.Vertex(vertices[4]);
        GL.Vertex(vertices[5]);

        GL.Vertex(vertices[5]);
        GL.Vertex(vertices[0]);

        GL.End();
    }
}
