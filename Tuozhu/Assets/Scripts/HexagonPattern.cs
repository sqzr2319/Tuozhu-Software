using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class HexagonPattern : MonoBehaviour
{
	public HexagonData hexagonData;

	public int n;
	public int[,] height;

	public int status;
	public float targetZ;
	public float rotationSpeed = 5f;

	public Vector3[] vertices = new Vector3[6];
	public Vector3 center;

	public Color colorBlue = new Color(31f / 255f, 150f / 255f, 192f / 255f);
	public Color colorLightBlue = new Color(142f / 255f, 205f / 255f, 223f / 255f);
	public Color colorDarkBlue = new Color(19f / 255f, 102f / 255f, 132f / 255f);

	// 保存原始颜色
	public Color originalBlue;
	public Color originalLightBlue;
	public Color originalDarkBlue;

	public float Radius;
	public float radius;
	public float angleOffset;

	public Material lineMaterial;

	// 添加平滑褪色参数及状态变量
	public float fadeDuration = 1f;
	public bool isFaded = false;
	public bool isTransitioning = false;

	public string sceneName;

	void Start()
	{
		// 获取当前场景名
		sceneName = SceneManager.GetActiveScene().name;
		switch (sceneName)
		{
			case "TutorialScene":
				n = hexagonData.tutorialN;
				height = hexagonData.tutorialHeight;
				break;
			case "Level1Scene":
				n = hexagonData.level1N;
				height = hexagonData.level1Height;
				break;
			case "Level2Scene":
				n = hexagonData.level2N;
				height = hexagonData.level2Height;
				break;
			case "Level3Scene":
				n = hexagonData.level3N;
				height = hexagonData.level3Height;
				break;
			case "Level4Scene":
				n = hexagonData.level4N;
				height = hexagonData.level4Height;
				break;
			case "Level5Scene":
				n = hexagonData.level5N;
				height = hexagonData.level5Height;
				break;
			case "Level6Scene":
				n = hexagonData.level6N;
				height = hexagonData.level6Height;
				break;
			case "Level7Scene":
				n = hexagonData.level7N;
				height = hexagonData.level7Height;
				break;
			case "Level8Scene":
				n = hexagonData.level8N;
				height = hexagonData.level8Height;
				break;
			case "Level9Scene":
				n = hexagonData.level9N;
				height = hexagonData.level9Height;
				break;
			case "Level10Scene":
				n = hexagonData.level10N;
				height = hexagonData.level10Height;
				break;
			case "Level11Scene":
				n = hexagonData.level11N;
				height = hexagonData.level11Height;
				break;
			case "Level12Scene":
				n = hexagonData.level12N;
				height = hexagonData.level12Height;
				break;
		}

		// 初始化材质
		lineMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
		lineMaterial.hideFlags = HideFlags.HideAndDontSave;
		lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
		lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
		lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
		lineMaterial.SetInt("_ZWrite", 0);

		// 存储原始颜色
		originalBlue = colorBlue;
		originalLightBlue = colorLightBlue;
		originalDarkBlue = colorDarkBlue;

		targetZ = transform.eulerAngles.z;
		status = (int)(transform.eulerAngles.z + 0.01 - 30) / 60;
		Radius = 0.5f * transform.localScale.x;
		radius = Radius / n;
	}

	void Update()
	{
		angleOffset = Mathf.Deg2Rad * (transform.eulerAngles.z);

		if (Input.GetKeyDown(KeyCode.X) && !isTransitioning)
		{
			StartCoroutine(ToggleFade());
		}

		if (!isFaded)
		{
			if (Input.GetKeyDown(KeyCode.A))
			{
				targetZ -= 60f;
				status = (status + 5) % 6;
			}
			if (Input.GetKeyDown(KeyCode.D))
			{
				targetZ += 60f;
				status = (status + 1) % 6;
			}
		}

		Vector3 currentAngles = transform.eulerAngles;
		currentAngles.z = Mathf.LerpAngle(currentAngles.z, targetZ, Time.deltaTime * rotationSpeed);
		transform.eulerAngles = currentAngles;
	}

	IEnumerator ToggleFade()
	{
		isTransitioning = true;
		float t = 0f;

		Color startBlue = colorBlue;
		Color startLightBlue = colorLightBlue;
		Color startDarkBlue = colorDarkBlue;
		// 根据当前状态设置目标颜色
		Color targetColor = isFaded ? Color.gray : Color.gray; // 目标均为灰色
		Color targetBlue = isFaded ? originalBlue : Color.gray;
		Color targetLightBlue = isFaded ? originalLightBlue : Color.gray;
		Color targetDarkBlue = isFaded ? originalDarkBlue : Color.gray;

		while (t < fadeDuration)
		{
			t += Time.deltaTime;
			float ratio = t / fadeDuration;
			colorBlue = Color.Lerp(startBlue, targetBlue, ratio);
			colorLightBlue = Color.Lerp(startLightBlue, targetLightBlue, ratio);
			colorDarkBlue = Color.Lerp(startDarkBlue, targetDarkBlue, ratio);
			yield return null;
		}

		colorBlue = targetBlue;
		colorLightBlue = targetLightBlue;
		colorDarkBlue = targetDarkBlue;
		isFaded = !isFaded;
		isTransitioning = false;
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
						CalculateLittleHexagons(i, j, k);
						DrawLittleHexagons();
						DrawLittleTriangles();
						DrawLittleLines();
					}
				}
			}
		}
	}

	public void CalculateVertices()
	{
		center = transform.position;
		for (int i = 0; i < 6; i++)
		{
			float angle = Mathf.Deg2Rad * 60f * i;
			vertices[i] = new Vector3(
				center.x + Radius * Mathf.Cos(angle + angleOffset),
				center.y + Radius * Mathf.Sin(angle + angleOffset),
				center.z
			);
		}
	}

	public void DrawHexagon()
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

	public void DrawTriangles()
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

	public void DrawLittleTriangles()
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
		GL.Vertex(vertices[5]);
		GL.Vertex(vertices[0]);
		GL.End();

		GL.Begin(GL.TRIANGLES);
		GL.Color(colorDarkBlue);
		GL.Vertex(center);
		GL.Vertex(vertices[4]);
		GL.Vertex(vertices[5]);
		GL.End();
	}

	public void DrawLines()
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

	public void DrawLinesInTriangle(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, int lineCount)
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
		center.x = center.x+radius * y * Mathf.Cos(1.0472f - angleOffset) - radius * x * Mathf.Cos(angleOffset) - radius * (h - 1) * Mathf.Sin(angleOffset - 0.5236f);
		center.y = center.y-radius * y * Mathf.Sin(1.0472f - angleOffset) - radius * x * Mathf.Sin(angleOffset) + radius * (h - 1) * Mathf.Cos(angleOffset - 0.5236f);

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

	public void DrawLittleHexagons()
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

	public void DrawLittleLines()
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

		GL.Vertex(center);
		GL.Vertex(vertices[0]);

		GL.Vertex(center);
		GL.Vertex(vertices[2]);

		GL.Vertex(center);
		GL.Vertex(vertices[4]);

		GL.End();
	}

}
