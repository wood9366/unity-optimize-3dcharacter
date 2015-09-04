using UnityEngine;
using System.Collections;

public class ArmyFormation : MonoBehaviour {
	public GameObject Template;
	public float Interval = 5;

	[HideInInspector]
	public int FormationRow = 4;

	[HideInInspector]
	public int FormationCol = 4;
}
