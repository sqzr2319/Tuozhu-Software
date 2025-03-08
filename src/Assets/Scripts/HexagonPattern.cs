using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;
using Unity.VisualScripting;

public class HexagonPattern : MonoBehaviour
{
	public HexagonData hexagonData;
	public GravityCalculation gravityCalculation;

	public int n;
	public int[,] height;
	public int[,,] polygon;
	public int changecnt = 0;

    public int status;
	public float targetZ;
	public float rotationSpeed = 5f;

	public Vector3[] bigvertices = new Vector3[6];
	public Vector3[] smallvertices = new Vector3[6];
	public Vector3[] tempvertices = new Vector3[6];
	public Vector3 bigcenter;
	public Vector3 smallcenter;
    public Vector3 tempcenter;

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

	Vector2 mousePos;
	public int[] nearest;
	public bool convex;
	public bool concave;

	private float blinkTimer = 0f;
	private bool showLines = true;
	private float blinkInterval = 1f;

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
		polygon = new int[3, n, n];

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
		polygonCalculation();
		angleOffset = Mathf.Deg2Rad * (transform.eulerAngles.z);

		if (Input.GetKeyDown(KeyCode.X) && !isTransitioning)
		{
			if(sceneName!="TutorialScene"&&sceneName!="Level1Scene")
			{
				StartCoroutine(ToggleFade());
			}
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
		else
		{
			heightChanging();
		}

		Vector3 currentAngles = transform.eulerAngles;
		currentAngles.z = Mathf.LerpAngle(currentAngles.z, targetZ, Time.deltaTime * rotationSpeed);
		transform.eulerAngles = currentAngles;
	}
	IEnumerator ToggleFade()
	{
		// 记录当前（旧）状态
		bool oldFaded = isFaded;
		isTransitioning = true;

		float t = 0f;
		Color startBlue = colorBlue;
		Color startLightBlue = colorLightBlue;
		Color startDarkBlue = colorDarkBlue;

		// 目标均为灰色，只是最终会替换原色
		Color targetBlue = oldFaded ? originalBlue : Color.gray;
		Color targetLightBlue = oldFaded ? originalLightBlue : Color.gray;
		Color targetDarkBlue = oldFaded ? originalDarkBlue : Color.gray;

		// 这里是淡入/淡出的动画过渡过程
		while (t < fadeDuration)
		{
			t += Time.deltaTime;
			float ratio = t / fadeDuration;
			colorBlue = Color.Lerp(startBlue, targetBlue, ratio);
			colorLightBlue = Color.Lerp(startLightBlue, targetLightBlue, ratio);
			colorDarkBlue = Color.Lerp(startDarkBlue, targetDarkBlue, ratio);
			yield return null;
		}

		// 动画结束后切换 isFaded
		isFaded = !oldFaded;
		isTransitioning = false;
	}

	void OnRenderObject()
	{
		CalculateVertices();
		DrawHexagon();
		DrawTriangles();
		DrawLines();
		for(int i=0;i<n;i++)
		{
			for(int j=0;j<n;j++)
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

		if (isFaded)
		{
            if (concave)
            {
                drawConcaveLines();
            }
            if (convex)
            {
                drawConvexLines();
            }
        }
    }

	public void CalculateVertices()
	{
		bigcenter = transform.position;
		for (int i = 0; i < 6; i++)
		{
			float angle = Mathf.Deg2Rad * 60f * i;
			bigvertices[i] = new Vector3(
				bigcenter.x + Radius * Mathf.Cos(angle + angleOffset),
				bigcenter.y + Radius * Mathf.Sin(angle + angleOffset),
				bigcenter.z
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
			GL.Vertex(bigcenter);
			GL.Vertex(bigvertices[i]);
			GL.Vertex(bigvertices[(i + 1) % 6]);
		}

		GL.End();
	}

	public void DrawTriangles()
	{
		lineMaterial.SetPass(0);

		GL.Begin(GL.TRIANGLES);
		GL.Color(colorBlue);
		GL.Vertex(bigcenter);
		GL.Vertex(bigvertices[5]);
		GL.Vertex(bigvertices[0]);
		GL.End();

		GL.Begin(GL.TRIANGLES);
		GL.Color(colorBlue);
		GL.Vertex(bigcenter);
		GL.Vertex(bigvertices[0]);
		GL.Vertex(bigvertices[1]);
		GL.End();

		GL.Begin(GL.TRIANGLES);
		GL.Color(colorLightBlue);
		GL.Vertex(bigcenter);
		GL.Vertex(bigvertices[3]);
		GL.Vertex(bigvertices[4]);
		GL.End();

		GL.Begin(GL.TRIANGLES);
		GL.Color(colorLightBlue);
		GL.Vertex(bigcenter);
		GL.Vertex(bigvertices[4]);
		GL.Vertex(bigvertices[5]);
		GL.End();

		GL.Begin(GL.TRIANGLES);
		GL.Color(colorDarkBlue);
		GL.Vertex(bigcenter);
		GL.Vertex(bigvertices[1]);
		GL.Vertex(bigvertices[2]);
		GL.End();

		GL.Begin(GL.TRIANGLES);
		GL.Color(colorDarkBlue);
		GL.Vertex(bigcenter);
		GL.Vertex(bigvertices[2]);
		GL.Vertex(bigvertices[3]);
		GL.End();
	}

	public void DrawLittleTriangles()
	{
		lineMaterial.SetPass(0);

		GL.Begin(GL.TRIANGLES);
		GL.Color(colorBlue);
		GL.Vertex(smallcenter);
		GL.Vertex(smallvertices[2]);
		GL.Vertex(smallvertices[3]);
		GL.End();

		GL.Begin(GL.TRIANGLES);
		GL.Color(colorBlue);
		GL.Vertex(smallcenter);
		GL.Vertex(smallvertices[3]);
		GL.Vertex(smallvertices[4]);
		GL.End();

		GL.Begin(GL.TRIANGLES);
		GL.Color(colorLightBlue);
		GL.Vertex(smallcenter);
		GL.Vertex(smallvertices[0]);
		GL.Vertex(smallvertices[1]);
		GL.End();

		GL.Begin(GL.TRIANGLES);
		GL.Color(colorLightBlue);
		GL.Vertex(smallcenter);
		GL.Vertex(smallvertices[1]);
		GL.Vertex(smallvertices[2]);
		GL.End();

		GL.Begin(GL.TRIANGLES);
		GL.Color(colorDarkBlue);
		GL.Vertex(smallcenter);
		GL.Vertex(smallvertices[5]);
		GL.Vertex(smallvertices[0]);
		GL.End();

		GL.Begin(GL.TRIANGLES);
		GL.Color(colorDarkBlue);
		GL.Vertex(smallcenter);
		GL.Vertex(smallvertices[4]);
		GL.Vertex(smallvertices[5]);
		GL.End();
	}

	public void DrawLines()
	{
		lineMaterial.SetPass(0);
		GL.Begin(GL.LINES);
		GL.Color(Color.white);

		DrawLinesInTriangle(bigvertices[0], bigvertices[1], bigvertices[5], bigcenter, n - 1);
		DrawLinesInTriangle(bigvertices[1], bigvertices[2], bigcenter, bigvertices[3], n - 1);
		DrawLinesInTriangle(bigvertices[2], bigvertices[3], bigvertices[1], bigcenter, n - 1);
		DrawLinesInTriangle(bigvertices[3], bigvertices[4], bigcenter, bigvertices[5], n - 1);
		DrawLinesInTriangle(bigvertices[4], bigvertices[5], bigvertices[3], bigcenter, n - 1);
		DrawLinesInTriangle(bigvertices[5], bigvertices[0], bigcenter, bigvertices[1], n - 1);

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
		smallcenter = transform.position;
		smallcenter.x = smallcenter.x+radius * y * Mathf.Cos(1.0472f - angleOffset) - radius * x * Mathf.Cos(angleOffset) - radius * (h - 1) * Mathf.Sin(angleOffset - 0.5236f);
		smallcenter.y = smallcenter.y-radius * y * Mathf.Sin(1.0472f - angleOffset) - radius * x * Mathf.Sin(angleOffset) + radius * (h - 1) * Mathf.Cos(angleOffset - 0.5236f);

		for (int i = 0; i < 6; i++)
		{
			float angle = Mathf.Deg2Rad * 60f * i;
			smallvertices[i] = new Vector3(
				smallcenter.x + radius * Mathf.Cos(angle + angleOffset),
				smallcenter.y + radius * Mathf.Sin(angle + angleOffset),
				smallcenter.z
			);
		}
	}

    public void CalculateTempHexagons(int x, int y, int h)
    {
        tempcenter = transform.position;
        tempcenter.x = tempcenter.x + radius * y * Mathf.Cos(1.0472f - angleOffset) - radius * x * Mathf.Cos(angleOffset) - radius * (h - 1) * Mathf.Sin(angleOffset - 0.5236f);
        tempcenter.y = tempcenter.y - radius * y * Mathf.Sin(1.0472f - angleOffset) - radius * x * Mathf.Sin(angleOffset) + radius * (h - 1) * Mathf.Cos(angleOffset - 0.5236f);

        for (int i = 0; i < 6; i++)
        {
            float angle = Mathf.Deg2Rad * 60f * i;
            tempvertices[i] = new Vector3(
                tempcenter.x + radius * Mathf.Cos(angle + angleOffset),
                tempcenter.y + radius * Mathf.Sin(angle + angleOffset),
                tempcenter.z
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
			GL.Vertex(smallcenter);
			GL.Vertex(smallvertices[i]);
			GL.Vertex(smallvertices[(i + 1) % 6]);
		}

		GL.End();
	}

	public void DrawLittleLines()
	{
		lineMaterial.SetPass(0);
		GL.Begin(GL.LINES);
		GL.Color(Color.white);

		GL.Vertex(smallvertices[0]);
		GL.Vertex(smallvertices[1]);

		GL.Vertex(smallvertices[1]);
		GL.Vertex(smallvertices[2]);

		GL.Vertex(smallvertices[2]);
		GL.Vertex(smallvertices[3]);

		GL.Vertex(smallvertices[3]);
		GL.Vertex(smallvertices[4]);

		GL.Vertex(smallvertices[4]);
		GL.Vertex(smallvertices[5]);

		GL.Vertex(smallvertices[5]);
		GL.Vertex(smallvertices[0]);

		GL.Vertex(smallcenter);
		GL.Vertex(smallvertices[0]);

		GL.Vertex(smallcenter);
		GL.Vertex(smallvertices[2]);

		GL.Vertex(smallcenter);
		GL.Vertex(smallvertices[4]);

		GL.End();
	}

	public void polygonCalculation()
	{
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
	public void OnGUI()
	{
        GUIStyle style = new GUIStyle(GUI.skin.label);
		style.fontSize = 36;
		GUI.Label(new Rect(10, 10, 300, 60), "下落次数：" + gravityCalculation.downfallcnt, style);
		if(sceneName!="TutorialScene"&&sceneName!="Level1Scene")
		{
			GUI.Label(new Rect(10, 70, 300, 60), "更改次数：" + changecnt, style);
		}
	}

	public void heightChanging()
	{
		nearest = new int[3];
		mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		if (IsPointInPolygon(bigvertices, mousePos))
		{
			CalculateNearest();
			concave = false;
			convex = false;

			int h1 = 0, h2 = 0, h3 = 0;
			int[] concavetemp1 = new int[3];
			int[] concavetemp2 = new int[3];
			int[] concavetemp3 = new int[3];
			int[] convextemp1 = new int[3];
			int[] convextemp2 = new int[3];
			int[] convextemp3 = new int[3];

			while (true)
			{
				h1 = polygon[nearest[0], nearest[1], nearest[2]];
				if (h1 >= n) break;
				h2 = polygon[(nearest[0] + 1) % 3, nearest[2], h1];
				if (h2 >= n) break;
				if (h2 != nearest[1]) break;
				h3 = polygon[(nearest[0] + 2) % 3, h1, nearest[1]];
				if (h3 >= n) break;
				if (h3 != nearest[2]) break;
				concave = true;
				break;
			}

			while (true)
			{
				h1 = polygon[nearest[0], nearest[1], nearest[2]];
				if (h1 <= 0) break;
				h2 = polygon[(nearest[0] + 1) % 3, nearest[2], h1 - 1];
				if (h2 <= 0) break;
				if (h2 != nearest[1]+1) break;
				h3 = polygon[(nearest[0] + 2) % 3, h1 - 1, nearest[1]];
				if (h3 <= 0) break;
				if (h3 != nearest[2]+1) break;
				convex = true;
				break;
			}

			if (concave && convex)
			{
				concavetemp1 = new int[3] { nearest[0], nearest[1], nearest[2] };
				concavetemp2 = new int[3] { (nearest[0] + 1) % 3, nearest[2], h1 };
				concavetemp3 = new int[3] { (nearest[0] + 2) % 3, h1, nearest[1] };
				convextemp1 = new int[3] { nearest[0], nearest[1], nearest[2] };
				convextemp2 = new int[3] { (nearest[0] + 1) % 3, nearest[2], h1 - 1 };
				convextemp3 = new int[3] { (nearest[0] + 2) % 3, h1 - 1, nearest[1] };

				float concavesum = 0;
				float convexsum = 0;

				concavesum += CalculateDistance(concavetemp1);
				concavesum += CalculateDistance(concavetemp2);
				concavesum += CalculateDistance(concavetemp3);

				convexsum += CalculateDistance(convextemp1);
				convexsum += CalculateDistance(convextemp2);
				convexsum += CalculateDistance(convextemp3);

				if (concavesum < convexsum)
					convex = false;
				else
					concave = false;
			}

			if (concave)
			{
				switch (nearest[0])
				{
					case 0:
						CalculateTempHexagons(nearest[1], nearest[2], h1+1);
						break;
					case 1:
						CalculateTempHexagons(h1, nearest[1], nearest[2]+1);
						break;
					case 2:
						CalculateTempHexagons(nearest[2], h1, nearest[1]+1);
						break;
				}
				drawConcaveLines();
				if (Input.GetMouseButtonDown(0))
				{
					changecnt++;
					switch (nearest[0])
					{
						case 0:
							height[nearest[1], nearest[2]]++;
							break;
						case 1:
							height[h1, nearest[1]]++;
							break;
						case 2:
							height[nearest[2], h1]++;
							break;
					}
                    polygonCalculation();
					return;
				}
				
			}

			if (convex)
			{
				switch (nearest[0])
				{
					case 0:
						CalculateTempHexagons(nearest[1], nearest[2], h1);
						break;
					case 1:
						CalculateTempHexagons(h1-1, nearest[1], nearest[2]+1);
						break;
					case 2:
						CalculateTempHexagons(nearest[2], h1-1, nearest[1]+1);
						break;
				}
				drawConvexLines();
				if (Input.GetMouseButtonDown(0))
				{
					changecnt++;
					switch (nearest[0])
					{
						case 0:
							height[nearest[1], nearest[2]]--;
							break;
						case 1:
							height[h1-1, nearest[1]]--;
							break;
						case 2:
							height[nearest[2], h1-1]--;
							break;
					}
                    polygonCalculation();
					return;
				}
				
			}
		}
	}

	bool IsPointInPolygon(Vector3[] polygonVerts, Vector2 point)
	{
		int crossingCount = 0;
		int length = polygonVerts.Length;
		for (int i = 0; i < length; i++)
		{
			// 多边形当前边的起点、终点
			Vector3 p1 = polygonVerts[i];
			Vector3 p2 = polygonVerts[(i + 1) % length];

			// 判断是否在射线范围内
			bool condY = (p1.y <= point.y && p2.y > point.y) || (p1.y > point.y && p2.y <= point.y);
			if (condY)
			{
				// 计算交点 X 坐标
				float xCross = p1.x + (point.y - p1.y) * (p2.x - p1.x) / (p2.y - p1.y);
				// 如果交点在点右侧，则计为一次射线穿过
				if (xCross > point.x)
				{
					crossingCount++;
				}
			}
		}
		// 若穿过次数为奇数，点在多边形内
		return (crossingCount % 2 == 1);
	}

	public void CalculateNearest()
	{
		float minDistance = float.MaxValue;
		for (int i = 0; i <= 2; i++)
		{
			for(int j = 0; j < n; j++)
			{
				for(int k=0; k < n; k++)
				{
					int h = polygon[i, j, k];
					Vector3 target = new Vector3();
					switch (i)
					{
						case 0:
							target.x = -radius * j * Mathf.Cos(angleOffset) + radius * k * Mathf.Cos(1.0472f - angleOffset) - radius * (h - 0.5f) * Mathf.Sin(angleOffset - 0.5236f);
							target.y = -radius * j * Mathf.Sin(angleOffset) - radius * k * Mathf.Sin(1.0472f - angleOffset) + radius * (h - 0.5f) * Mathf.Cos(angleOffset - 0.5236f);
							break;
						case 2:
							target.x = -radius * k * Mathf.Cos(angleOffset) + radius * (h - 0.5f) * Mathf.Cos(1.0472f - angleOffset) - radius * j * Mathf.Sin(angleOffset - 0.5236f);
							target.y = -radius * k * Mathf.Sin(angleOffset) - radius * (h - 0.5f) * Mathf.Sin(1.0472f - angleOffset) + radius * j * Mathf.Cos(angleOffset - 0.5236f);
							break;
						case 1:
							target.x = -radius * (h - 0.5f) * Mathf.Cos(angleOffset) + radius * j * Mathf.Cos(1.0472f - angleOffset) - radius * k * Mathf.Sin(angleOffset - 0.5236f);
							target.y = -radius * (h - 0.5f) * Mathf.Sin(angleOffset) - radius * j * Mathf.Sin(1.0472f - angleOffset) + radius * k * Mathf.Cos(angleOffset - 0.5236f);
							break;
					}
					float distance = Vector3.Distance(target, mousePos);
					if (distance < minDistance)
					{
						minDistance = distance;
						nearest[0] = i;
						nearest[1] = j;
						nearest[2] = k;
					}
				}
			}
		}
	}

	public float CalculateDistance(int[] index)
	{
		Vector3 target = new Vector3();
		int h = polygon[index[0], index[1], index[2]];
		switch (index[0])
		{
			case 0:
				target.x = -radius * index[1] * Mathf.Cos(angleOffset) + radius * index[2] * Mathf.Cos(1.0472f - angleOffset) - radius * (h - 0.5f) * Mathf.Sin(angleOffset - 0.5236f);
				target.y = -radius * index[1] * Mathf.Sin(angleOffset) - radius * index[2] * Mathf.Sin(1.0472f - angleOffset) + radius * (h - 0.5f) * Mathf.Cos(angleOffset - 0.5236f);
				break;
			case 2:
				target.x = -radius * index[2] * Mathf.Cos(angleOffset) + radius * (h - 0.5f) * Mathf.Cos(1.0472f - angleOffset) - radius * index[1] * Mathf.Sin(angleOffset - 0.5236f);
				target.y = -radius * index[2] * Mathf.Sin(angleOffset) - radius * (h - 0.5f) * Mathf.Sin(1.0472f - angleOffset) + radius * index[1] * Mathf.Cos(angleOffset - 0.5236f);
				break;
			case 1:
				target.x = -radius * (h - 0.5f) * Mathf.Cos(angleOffset) + radius * index[1] * Mathf.Cos(1.0472f - angleOffset) - radius * index[2] * Mathf.Sin(angleOffset - 0.5236f);
				target.y = -radius * (h - 0.5f) * Mathf.Sin(angleOffset) - radius * index[1] * Mathf.Sin(1.0472f - angleOffset) + radius * index[2] * Mathf.Cos(angleOffset - 0.5236f);
				break;
		}
		return Vector3.Distance(target, mousePos);
	}

    public void drawConcaveLines()
    {
        // 使用 sin 函数平滑计算当前 alpha 值，blinkInterval 控制闪烁周期
        float alpha = 0.5f + 0.5f * Mathf.Sin(Time.time * (Mathf.PI * 2f / blinkInterval));
        Color blinkColor = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, alpha);

        lineMaterial.SetPass(0);
        GL.Begin(GL.LINES);
        GL.Color(blinkColor);

        GL.Vertex(tempvertices[0]);
        GL.Vertex(tempvertices[1]);

        GL.Vertex(tempvertices[1]);
        GL.Vertex(tempvertices[2]);

        GL.Vertex(tempvertices[2]);
        GL.Vertex(tempvertices[3]);

        GL.Vertex(tempvertices[3]);
        GL.Vertex(tempvertices[4]);

        GL.Vertex(tempvertices[4]);
        GL.Vertex(tempvertices[5]);

        GL.Vertex(tempvertices[5]);
        GL.Vertex(tempvertices[0]);

        GL.Vertex(tempcenter);
        GL.Vertex(tempvertices[1]);

        GL.Vertex(tempcenter);
        GL.Vertex(tempvertices[3]);

        GL.Vertex(tempcenter);
        GL.Vertex(tempvertices[5]);

        GL.End();
    }

    public void drawConvexLines()
    {
        // 同样使用平滑的 alpha 值
        float alpha = 0.5f + 0.5f * Mathf.Sin(Time.time * (Mathf.PI * 2f / blinkInterval));
        Color blinkColor = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, alpha);

        lineMaterial.SetPass(0);
        GL.Begin(GL.LINES);
        GL.Color(blinkColor);

        GL.Vertex(tempvertices[0]);
        GL.Vertex(tempvertices[1]);

        GL.Vertex(tempvertices[1]);
        GL.Vertex(tempvertices[2]);

        GL.Vertex(tempvertices[2]);
        GL.Vertex(tempvertices[3]);

        GL.Vertex(tempvertices[3]);
        GL.Vertex(tempvertices[4]);

        GL.Vertex(tempvertices[4]);
        GL.Vertex(tempvertices[5]);

        GL.Vertex(tempvertices[5]);
        GL.Vertex(tempvertices[0]);

        GL.Vertex(tempcenter);
        GL.Vertex(tempvertices[0]);

        GL.Vertex(tempcenter);
        GL.Vertex(tempvertices[2]);

        GL.Vertex(tempcenter);
        GL.Vertex(tempvertices[4]);

        GL.End();
    }

}
