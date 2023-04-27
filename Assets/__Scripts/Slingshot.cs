using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slingshot : MonoBehaviour {
	static private Slingshot S;
	// поля, устанавливаемые в инспекторе Unity
	[Header("Set in Inspector")]
	public GameObject prefabProjectile;
	public float velocityMult = 8f;
	public GameObject[] shapes;

	//поля, устанавливаемые динамически
	[Header("Set Dynamically")]
	public GameObject launchPoint;
	public Vector3 launchPos;
	public GameObject projectile;
	public bool aimingMode;
	public Button changeProjectile;
	private Rigidbody projectileRigidbody;

	private int appearance = 0;

	static public Vector3 LAUNCH_POS {
		get {
			if (S == null) return Vector3.zero;
				return S.launchPos;
		}
	}

	void Awake() {
		S = this;
		Transform launchPointTrans = transform.Find("LaunchPoint");
		launchPoint = launchPointTrans.gameObject;
		launchPoint.SetActive ( false );
		launchPos = launchPointTrans.position;
		Button btn = changeProjectile.GetComponent<Button>();
		btn.onClick.AddListener(change_projectile);
	}

	void OnMouseEnter() {
		// print ("Slingshot:OnMouseEnter()");
		launchPoint.SetActive( true );
	}
	
	void OnMouseExit() {
		// print ("Slingshot:OnMouseExit()");
		launchPoint.SetActive( false );
	}

	void OnMouseDown() {
	// игрок нажал кнопку мыши, когда указатель находился над рогаткой
		aimingMode = true;
		// создать снаряд
		projectile = Instantiate(prefabProjectile) as GameObject;
		// поместить в точку launchPoint
		projectile.transform.position = launchPos;
		// сделать его кинематическим
		projectileRigidbody = projectile.GetComponent<Rigidbody>();
		projectileRigidbody.isKinematic = true;
		projectileRigidbody.GetComponent<MeshRenderer>().sharedMaterial = shapes[appearance].GetComponent<MeshRenderer>().sharedMaterial;
		projectileRigidbody.GetComponent<MeshFilter>().sharedMesh = shapes[appearance].GetComponent<MeshFilter>().sharedMesh;
	}

	void Update() {
	// если рогатка не в режиме прицеливания, не выполнять этот код
		if (!aimingMode) return;

		// получить текущие экранные координаты указателя мыши
		Vector3 mousePos2D = Input.mousePosition;
		mousePos2D.z = -Camera.main.transform.position.z;
		Vector3 mousePos3D = Camera.main.ScreenToWorldPoint (mousePos2D);

		// найти разность координат между launchPos и mousePos3D
		Vector3 mouseDelta = mousePos3D-launchPos;
		// ограничить mouseDelta радиусом коллайдера объекта Slingshot
		float MaxMagnitude = this.GetComponent<SphereCollider>().radius;
		if (mouseDelta.magnitude > MaxMagnitude) {
			mouseDelta.Normalize ();
			mouseDelta *= MaxMagnitude;
		}

		// передвинуть снаряд в новую позицию
		Vector3 projPos = launchPos + mouseDelta;
		projectile.transform.position = projPos;
		if (Input.GetMouseButtonUp (0)) {
			// кнопка мыши отпущена
			aimingMode = false;
			projectileRigidbody.isKinematic = false;
			projectileRigidbody.velocity = -mouseDelta * velocityMult;
			FollowCam.POI = projectile;
			projectile = null;
			MissionDemolition.ShotFired();
			ProjectileLine.S.poi = projectile;
		}
	}

	public void change_projectile() {
		if(appearance == 0) {
			appearance = 1;
		} else {
			appearance = 0;
		}
	}
}
