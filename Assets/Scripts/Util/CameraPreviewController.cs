using UnityEngine;
using System.Collections;

public class CameraPreviewController : MonoBehaviour {
	public Camera Cam;

	public Vector3 Target = Vector3.zero;
	public float Distance = 100;
	public float AngleA = 45;
	public float RotateSpeed = 10;

	private float _angleB = 0;

	public void play() {
		_isPlay = true;
	}

	public void stop() {
		_isPlay = false;
	}

	public float AngleB {
		get { return _angleB * Mathf.Rad2Deg; }
		set {
			_angleB = value * Mathf.Deg2Rad;
			updateCam();
		}
	}

	void Start () {
		_angleB = 0;
	}
	
	void Update () {
		if (_isPlay) {
			_angleB += RotateSpeed * Time.deltaTime;

			if (_angleB > 360) {
				_angleB -= 360;
			}

			updateCam();
		}
	}

	void updateCam() {
		float sina = Mathf.Sin(AngleA * Mathf.Deg2Rad);
		float cosa = Mathf.Cos(AngleA * Mathf.Deg2Rad);
		float sinb = Mathf.Sin(_angleB);
		float cosb = Mathf.Cos(_angleB);
		
		Cam.transform.position = new Vector3(Distance * cosa * cosb,
		                                     Distance * sina,
		                                     Distance * cosa * sinb);
		
		Quaternion rot = Quaternion.identity;
		rot.SetLookRotation(Target - Cam.transform.position);
		
		Cam.transform.rotation = rot;
	}

	bool _isPlay = false;
}
