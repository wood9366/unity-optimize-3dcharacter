using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ArmyFormation))]
public class ArmyFormationInspector : Editor {
	void OnEnable() {
		_formation = target as ArmyFormation;
	}

	public override void OnInspectorGUI () {
		GUILayout.BeginVertical();

		GUILayout.BeginHorizontal();
		_formation.FormationRow = EditorGUILayout.IntField(_formation.FormationRow);
		_formation.FormationCol = EditorGUILayout.IntField(_formation.FormationCol);
		GUILayout.EndHorizontal();

		if (GUILayout.Button("arrage")) {
			_formation.Template.SetActive(false);

			for (int r = 0; r < _formation.FormationRow; r++) {
				for (int c = 0; c < _formation.FormationCol; c++) {
					Vector3 pos = new Vector3(r * _formation.Interval - ((_formation.FormationRow - 1) * _formation.Interval) / 2, 
					                          0.0f, 
					                          c * _formation.Interval - ((_formation.FormationCol - 1) * _formation.Interval) / 2);

					createSoldier(_formation.Template, r.ToString() + "_" + c.ToString(), pos);
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
