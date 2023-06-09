﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Slingshot : MonoBehaviour {
	static private Slingshot S;
	// поля, устанавливаемые в инспекторе Unity
	[Header("Set in Inspector")]
	public GameObject prefabProjectile;
	public GameObject prefabSecondProjectile;
	public float velocityMult = 8f;
	public Text uitButton; // ссылка на дочерний объект Text в UIButton_View

	//поля, устанавливаемые динамически
	[Header("Set Dynamically")]
	public GameObject launchPoint;
	public Vector3 launchPos;
	public GameObject projectile;
	public bool aimingMode;

	private Rigidbody projectileRigidbody;
	private GameObject SetPrefab;

	static public Vector3 LAUNCH_POS {
		get {
			if (S == null) return Vector3.zero;
				return S.launchPos;
		}
	}
	void Start()
    {
		prefabProjectile.SetActive(true);
		prefabSecondProjectile.SetActive(false);
		SetPrefab = prefabProjectile;
		uitButton.text = "Switch To Sphere";
	}

	public void SwitchProjectile()
    {
		if (prefabProjectile.activeSelf)
		{
			// Switch to the spear
			prefabProjectile.SetActive(false);
			prefabSecondProjectile.SetActive(true);
			SetPrefab = prefabSecondProjectile;
			uitButton.text = "Switch To Ball";



			// Enable spear physics
			prefabSecondProjectile.GetComponent<Rigidbody>().isKinematic = false;
		}
		else
		{
			// Switch to the ball
			prefabSecondProjectile.SetActive(false);
			prefabProjectile.SetActive(true);
			SetPrefab = prefabProjectile;
			uitButton.text = "Switch To Sphere";


			// Enable ball physics
			prefabProjectile.GetComponent<Rigidbody>().isKinematic = false;
		}
	}

	void Awake() {
		S = this;
		Transform launchPointTrans = transform.Find("LaunchPoint");
		launchPoint = launchPointTrans.gameObject;
		launchPoint.SetActive ( false );
		launchPos = launchPointTrans.position;
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
		projectile = Instantiate(SetPrefab) as GameObject;
		// поместить в точку launchPoint
		projectile.transform.position = launchPos;
		// сделать его кинематическим
		projectileRigidbody = projectile.GetComponent<Rigidbody>();
		projectileRigidbody.isKinematic = true;
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
}
