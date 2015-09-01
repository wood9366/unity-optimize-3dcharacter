using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SpriteCharacter : MonoBehaviour {
	[System.Serializable]
	public struct SpriteData {
		public Texture Tex;
		public TextAsset Config;
	}

	public List<SpriteData> Sprites;

	private struct AnimationData {
		public string Name;
		public int StartFrame;
		public int EndFrame;
	}

	private class CharacterData {
		List<AnimationData> AnimDatas;
	}

	void updateCharacterSprite(int x, int y, int w, int h) {
		changeSize(w / 100.0f, h / 100.0f);
		changeUV(x, y, w, h, 1024, 1024);
	}

	void Awake() {
		_filter = GetComponent<MeshFilter>();

		generateMesh();

		updateCharacterSprite(104, 840, 88, 114);
	}

	void generateMesh() {
		_filter.sharedMesh = new Mesh();
		
		changeSize(1, 1);
		changeUV(0, 0, 1024, 1024, 1024, 1024);
		
		M.triangles = new int[] { 0, 2, 1, 0, 3, 2 };
	}

	void changeSize(float w, float h) {
		float hw = w / 2;
		
		M.vertices = new Vector3[] {
			new Vector3(hw, 0, 0),
			new Vector3(hw, h, 0),
			new Vector3(-hw, h, 0),
			new Vector3(-hw, 0, 0)
		};
	}

	void changeUV(int x, int y, int w, int h, int tw, int th) {
		float u0 = x / (float)tw;
		float u1 = (x + w) / (float)tw;
		float v0 = (th - y - h) / (float)th;
		float v1 = (th - y) / (float)th;

		Debug.Log("uv, " + u0 + ", " + u1 + ", " + v0 + ", " + v1);

		M.uv = new Vector2[] {
			new Vector2(u1, v0),
			new Vector2(u1, v1),
			new Vector2(u0, v1),
			new Vector2(u0, v0)
		};
	}

	Mesh M { get { return _filter.mesh; } }

	private MeshFilter _filter = null;
}
