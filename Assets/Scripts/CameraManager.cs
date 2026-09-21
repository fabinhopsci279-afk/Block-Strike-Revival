using UnityEngine;

public class CameraManager : MonoBehaviour
{
	public enum CameraType
	{
		None,
		Dead,
		Static,
		Spectate
	}

	public CameraType SelectCameraType;

	public Camera m_Camera;

	private Rigidbody m_Rigidbody;

	private Transform m_Transform;

	private BoxCollider m_BoxCollider;

	private GameObject[] SpectatePoints;

	private int SelectPoint;

	private Transform Point;

	public float Distance;

	public float DistanceMin;

	public float DistanceMax;

	public float SpeedRotation;

	public LayerMask Layers;

	private float X;

	private float Y;

	private static CameraManager instance;

	private void Awake()
	{
		instance = this;
		m_Rigidbody = m_Camera.GetComponent<Rigidbody>();
		m_Transform = m_Camera.transform;
		m_BoxCollider = m_Camera.GetComponent<BoxCollider>();
	}

	private void Update()
	{
		if (SelectCameraType == CameraType.Spectate && InputManager.GetButtonDown("Fire"))
		{
			UpdatePlayers();
		}
	}

	public static void ActiveDeadCamera(Vector3 position, Vector3 rotation, Vector3 force)
	{
		if (instance.SelectCameraType == CameraType.Spectate)
		{
			DeactiveSpectateCamera();
		}
		instance.SelectCameraType = CameraType.Dead;
		instance.m_Transform.gameObject.SetActive(value: true);
		instance.m_BoxCollider.enabled = true;
		instance.m_Rigidbody.isKinematic = false;
		instance.m_Transform.position = position;
		instance.m_Transform.eulerAngles = rotation;
		instance.m_Rigidbody.AddForce(force);
		instance.m_Rigidbody.AddRelativeForce(force);
	}

	public static void DeactiveDeadCamera()
	{
		instance.SelectCameraType = CameraType.None;
		instance.m_Rigidbody.isKinematic = true;
		instance.m_BoxCollider.enabled = false;
		instance.m_Transform.gameObject.SetActive(value: false);
	}

	public static void ActiveSpectateCamera()
	{
		if (instance.SelectCameraType == CameraType.Dead)
		{
			DeactiveDeadCamera();
		}
		instance.SelectCameraType = CameraType.Spectate;
		instance.m_BoxCollider.enabled = false;
		instance.m_Transform.gameObject.SetActive(value: true);
		instance.UpdatePlayers();
	}

	public static void DeactiveSpectateCamera()
	{
		instance.SelectCameraType = CameraType.None;
		instance.m_Transform.gameObject.SetActive(value: false);
	}

	public static void ActiveStaticCamera()
	{
		if (instance.SelectCameraType == CameraType.Dead)
		{
			DeactiveDeadCamera();
		}
		if (instance.SelectCameraType == CameraType.Spectate)
		{
			DeactiveSpectateCamera();
		}
		instance.SelectCameraType = CameraType.Static;
		instance.m_Transform.gameObject.SetActive(value: true);
		Transform transform = GameObject.FindGameObjectWithTag("StaticPoint").transform;
		instance.m_Transform.position = transform.position;
		instance.m_Transform.rotation = transform.rotation;
	}

	public static void DeactiveStaticCamera()
	{
		instance.SelectCameraType = CameraType.None;
		instance.m_Transform.gameObject.SetActive(value: false);
	}

	public static void DeactiveAll()
	{
		DeactiveDeadCamera();
		DeactiveSpectateCamera();
		DeactiveStaticCamera();
	}

	public static Transform GetActiveCamera()
	{
		if (instance.SelectCameraType != 0)
		{
			return instance.m_Transform;
		}
		if (PlayerInput.instance == null)
		{
			return null;
		}
		return PlayerInput.instance.FPCamera.Transform;
	}

	private void UpdatePlayers()
	{
		SpectatePoints = GameObject.FindGameObjectsWithTag("SpectatePoint");
		SelectPoint++;
		if (SelectPoint > SpectatePoints.Length - 1)
		{
			SelectPoint = 0;
		}
		if (SpectatePoints.Length != 0)
		{
			Point = SpectatePoints[SelectPoint].transform;
		}
		else
		{
			ActiveStaticCamera();
		}
	}

	private void LateUpdate()
	{
		if ((bool)Point && SelectCameraType == CameraType.Spectate)
		{
			Distance = DistanceMax;
			X += UnityEngine.Input.GetAxis("Mouse X") * SpeedRotation * Distance * 0.02f;
			Y -= UnityEngine.Input.GetAxis("Mouse Y") * SpeedRotation * 0.02f;
			Y = Utils.ClampAngle(Y, -20f, 80f);
			Quaternion rotation = Quaternion.Euler(Y, X, 0f);
			Ray ray = new Ray(direction: (m_Transform.position - Point.position).normalized, origin: Point.position);
			if (Physics.SphereCast(ray.origin, 0.25f, ray.direction, out RaycastHit hitInfo, Distance, Layers))
			{
				MonoBehaviour.print(hitInfo.transform.name);
				Distance = hitInfo.distance;
				Distance = Mathf.Clamp(Distance, DistanceMin, DistanceMax);
			}
			Vector3 point = new Vector3(0f, 0f, 0f - Distance);
			Vector3 position = rotation * point + Point.position;
			m_Transform.rotation = rotation;
			m_Transform.position = position;
		}
	}
}
