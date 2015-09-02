using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ArmyFormation))]
public class ArmyFormationInspector : Editor {
	void OnEnable() {
		_formation = target as ArmyFormation;
	}

	public override void OnInspectorGUI () {
		int row = 4;
		int col = 4;

		GUILayout.BeginVertical();

		GUILayout.BeginHorizontal();
		row = EditorGUILayout.IntField(row);
		col = EditorGUILayout.IntField(col);
		GUILayout.EndHorizontal();

		if (GUILayout.Button("arrage")) {
			_formation.Template.SetActive(false);

			for (int r = 0; r < row; r++) {
				for (int c = 0; c < col; c++) {
					createSoldier(_formation.Template, 
					              r.ToString() + "_" + c.ToString(), 
					              new Vector3(r * _formation.Interval, 0.0f, c * _formation.Interval));
				}
			}
		}

		GUILayout.EndVertical();

		DrawDefaultInspector();
	}

	void createSoldier(GameObject template, string name, Vector3 pos) {
		GameObject obj = GameObject.Instantiate(template);

		obj.name = name;
		obj.transform.parent = null;
		obj.transform.localPosition = Vector3.zero;
		obj.transform.localRotation = Quaternion.identity;
		obj.transform.localScale = Vector3.one;

		obj.transform.position = pos;

		obj.SetActive(true);
	}

	private ArmyFormation _formation = null;
}
