using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using System;

public class GravityCalculation : MonoBehaviour
{
	public HexagonPattern hexagonPattern;
	public ObjectData objectData;

	private int n;
	private int[,] height;
	private int[,,] polygon;

	private string sceneName;
	private string objectName;

	private int[] start;
	private int[] end;
	private bool doubleGravity;

	private float radius;
	private float angleOffset;
	private int status;

	private Vector3 startcenter;
	private Vector3 endcenter;
	
	private Vector3 velocity;
	public float smoothTime;
	public float lastAngle;

	public int downfallcnt = 0;
	void Start()
	{
		n = hexagonPattern.n;
		polygon = new int[3, n, n];
		sceneName = SceneManager.GetActiveScene().name;
		objectName = gameObject.name;

		switch (sceneName)
		{
			case "TutorialScene":
				start= objectData.tutorialStart;
				end = objectData.tutorialEnd;
				doubleGravity = objectData.tutorialDoubleGravity;
				break;
			case "Level1Scene":
				start = objectData.level1Start;
				end = objectData.level1End;
				doubleGravity = objectData.level1DoubleGravity;
				break;
			case "Level2Scene":
				start = objectData.level2Start;
				end = objectData.level2End;
				doubleGravity = objectData.level2DoubleGravity;
				break;
			case "Level3Scene":
				start = objectData.level3Start;
				end = objectData.level3End;
				doubleGravity = objectData.level3DoubleGravity;
				break;
			case "Level4Scene":
				start = objectData.level4Start;
				end = objectData.level4End;
				doubleGravity = objectData.level4DoubleGravity;
				break;
			case "Level5Scene":
				start = objectData.level5Start;
				end = objectData.level5End;
				doubleGravity = objectData.level5DoubleGravity;
				break;
			case "Level6Scene":
				start = objectData.level6Start;
				end = objectData.level6End;
				doubleGravity = objectData.level6DoubleGravity;
				break;
			case "Level7Scene":
				start = objectData.level7Start;
				end = objectData.level7End;
				doubleGravity = objectData.level7DoubleGravity;
				break;
			case "Level8Scene":
				start = objectData.level8Start;
				end = objectData.level8End;
				doubleGravity = objectData.level8DoubleGravity;
				break;
			case "Level9Scene":
				start = objectData.level9Start;
				end = objectData.level9End;
				doubleGravity = objectData.level9DoubleGravity;
				break;
			case "Level10Scene":
				start = objectData.level10Start;
				end = objectData.level10End;
				doubleGravity = objectData.level10DoubleGravity;
				break;
			case "Level11Scene":
				start = objectData.level11Start;
				end = objectData.level11End;
				doubleGravity = objectData.level11DoubleGravity;
				break;
			case "Level12Scene":
				start = objectData.level12Start;
				end = objectData.level12End;
				doubleGravity = objectData.level12DoubleGravity;
				break;
		}
	}
	void Update()
	{
		polygonCalculation();

		switch (objectName)
		{
			case "Start":
				positionCalculation(ref start, true, ref startcenter);
				break;
			case "End":
				positionCalculation(ref end, false, ref endcenter);
				break;
		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			SceneManager.LoadScene("LevelSelectScene");
		}
		if (Input.GetKeyDown(KeyCode.S))
		{
			if(objectName == "Start")
			{
				downfallcnt++;
				status = hexagonPattern.status;
				gravityCalculation(ref start);
			}
		}

		if (start[0] == end[0] && start[1] == end[1] && start[2] == end[2])
		{
			StartCoroutine(LoadWinningSceneDelay());
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

	public void positionCalculation(ref int[] index,bool gravity,ref Vector3 target)
	{

		radius = hexagonPattern.radius;
		angleOffset = Mathf.Deg2Rad * (hexagonPattern.transform.eulerAngles.z);
		int h = polygon[index[0], index[1], index[2]];

		switch (index[0])
		{
			case 0:
				target.x = - radius * index[1] * Mathf.Cos(angleOffset) + radius * index[2] * Mathf.Cos(1.0472f - angleOffset) - radius * (h - 0.5f) * Mathf.Sin(angleOffset - 0.5236f);
				target.y = - radius * index[1] * Mathf.Sin(angleOffset) - radius * index[2] * Mathf.Sin(1.0472f - angleOffset) + radius * (h - 0.5f) * Mathf.Cos(angleOffset - 0.5236f);
				break;
			case 2:
				target.x = - radius * index[2] * Mathf.Cos(angleOffset) + radius * (h - 0.5f) * Mathf.Cos(1.0472f - angleOffset) - radius * index[1] * Mathf.Sin(angleOffset - 0.5236f);
				target.y = - radius * index[2] * Mathf.Sin(angleOffset) - radius * (h - 0.5f) * Mathf.Sin(1.0472f - angleOffset) + radius * index[1] * Mathf.Cos(angleOffset - 0.5236f);
				break;
			case 1:
				target.x = - radius * (h - 0.5f) * Mathf.Cos(angleOffset) + radius * index[1] * Mathf.Cos(1.0472f - angleOffset) - radius * index[2] * Mathf.Sin(angleOffset - 0.5236f);
				target.y = - radius * (h - 0.5f) * Mathf.Sin(angleOffset) - radius * index[1] * Mathf.Sin(1.0472f - angleOffset) + radius * index[2] * Mathf.Cos(angleOffset - 0.5236f);
				break;
		}

		target.z = -2;
		if (gravity && lastAngle - angleOffset < 0.001f && lastAngle - angleOffset > -0.001f)
		{
			transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, smoothTime);
		}
		else
		{
			transform.position = target;
		}
		lastAngle = angleOffset;
	}
	private System.Collections.IEnumerator LoadWinningSceneDelay()
	{
		yield return new WaitForSeconds(0.3f);
		SceneManager.LoadScene("WinningScene");
	}

	public void gravityCalculation(ref int[] index)
	{
		int h=polygon[index[0], index[1], index[2]];
		switch (status)
		{
			case 0:
				switch (index[0])
				{
					case 0:
						downfallcnt--;
						break;
					case 1:
						if (h < n)
						{
							index[2] = index[1];
							index[1] = h;
							index[0] = status % 3;
						}
						else{
							SceneManager.LoadScene("LoseScene");
						}
						break;
					case 2:
						if (h < n)
						{
							index[1] = index[2];
							index[2] = h;
							index[0] = status % 3;
						}
						else{
							SceneManager.LoadScene("LoseScene");
						}
						break;
				}
				break;
			case 1:
				switch (index[0])
				{
					case 0:
						if (h > 0)
						{
							index[1] = index[2];
							index[2] = h - 1;
							index[0] = status % 3;
						}
						else{
							SceneManager.LoadScene("LoseScene");
						}
						break;
					case 1:
						downfallcnt--;
						break;
					case 2:
						if (h > 0)
						{
							index[2] = index[1];
							index[1] = h - 1;
							index[0] = status % 3;
						}
						else{
							SceneManager.LoadScene("LoseScene");
						}
						break;
				}
				break;
			case 2:
				switch (index[0])
				{
					case 0:
						if(h < n)
						{
							index[2] = index[1];
							index[1] = h;
							index[0] = status % 3;
						}
						else{
							SceneManager.LoadScene("LoseScene");
						}
						break;
					case 1:
						if(h < n)
						{
							index[1] = index[2];
							index[2] = h;
							index[0] = status % 3;
						}
						else{
							SceneManager.LoadScene("LoseScene");
						}
						break;
					case 2:
						downfallcnt--;
						break;
				}
				break;
			case 3:
				switch (index[0])
				{
					case 0:
						downfallcnt--;
						break;
					case 1:
						if (h > 0)
						{
							index[2] = index[1];
							index[1] = h - 1;
							index[0] = status % 3;
						}
						else{
							SceneManager.LoadScene("LoseScene");
						}
						break;
					case 2:
						if (h > 0)
						{
							index[1] = index[2];
							index[2] = h - 1;
							index[0] = status % 3;
						}
						else{
							SceneManager.LoadScene("LoseScene");
						}
						break;
				}
				break;
			case 4:
				switch (index[0])
				{
					case 0:
						if(h < n)
						{
							index[1] = index[2];
							index[2] = h;
							index[0] = status % 3;
						}
						else{
							SceneManager.LoadScene("LoseScene");
						}
						break;
					case 1:
						downfallcnt--;
						break;
					case 2:
						if (h < n)
						{
							index[2] = index[1];
							index[1] = h;
							index[0] = status % 3;
						}
						else{
							SceneManager.LoadScene("LoseScene");
						}
						break;
				}
				break;
			case 5:
				switch (index[0])
				{
					case 0:
						if (h > 0)
						{
							index[2] = index[1];
							index[1] = h - 1;
							index[0] = status % 3;
						}
						else{
							SceneManager.LoadScene("LoseScene");
						}
						break;
					case 1:
						if (h > 0)
						{
							index[1] = index[2];
							index[2] = h - 1;
							index[0] = status % 3;
						}
						else{
							SceneManager.LoadScene("LoseScene");
						}
						break;
					case 2:
						downfallcnt--;
						break;
				}
				break;
		}
		positionCalculation(ref index,true, ref startcenter);
	}

	public void OnGUI()
	{
		GUIStyle style = new GUIStyle(GUI.skin.label);
		style.fontSize = 36;
		GUI.Label(new Rect(10, 10, 300, 60), "下落次数：" + downfallcnt, style);
	}
}
