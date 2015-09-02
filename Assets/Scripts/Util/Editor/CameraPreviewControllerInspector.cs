using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(CameraPreviewController))]
public class CameraPreviewControllerInspector : Editor {
	void OnEnable() {
		_preview = target as CameraPreviewController;
	}

	public override void OnInspectorGUI() {
		GUILayout.BeginVertical();

		GUILayout.BeginHorizontal();
		if (GUILayout.Button("play")) _preview.play();
		if (GUILayout.Button("stop")) _preview.stop();
		GUILayout.EndHorizontal();

		_preview.AngleB = GUILayout.HorizontalSlider(_preview.AngleB, 0, 359);

		GUILayout.EndVertical();

		DrawDefaultInspector();
	}

	private CameraPreviewController _preview = null;
}
