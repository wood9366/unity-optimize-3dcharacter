using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SpriteCharacterRender : MonoBehaviour {
	const float SPRITE_CHARACTER_SCALE = 300;

	void Awake() {
		_renderer = GetComponent<MeshRenderer>();
		_filter = GetComponent<MeshFilter>();
		
		generateMesh();
		
		R.sharedMaterial = new Material(R.sharedMaterial);
	}

	public void updateRotation(float angle) {
		transform.rotation = Quaternion.AngleAxis(-angle, Vector3.up);
	}

	public void updateCharacterSprite(Texture tex, int x, int y, int w, int h, float offsetx, float offsety, int tw, int th) {
		//		Debug.Log("update character sprite: " + x + ", " + y + ", " + w + ", " + h + ", " + offsetx + ", " + offsety + ", " + tw + ", " + th);

		R.sharedMaterial.mainTexture = tex;

		changeSize(w, h, offsetx, offsety);
		changeUV(x, y, w, h, tw, th);
	}

	void generateMesh() {
		_filter.sharedMesh = new Mesh();
		
		changeSize(1, 1, 0, 0);
		changeUV(0, 0, 1024, 1024, 1024, 1024);
		
		M.triangles = new int[] { 0, 2, 1, 0, 3, 2 };
	}
	
	void changeSize(float w, float h, float ox, float oy) {
		float x0 = -ox / SPRITE_CHARACTER_SCALE;
		float x1 = (w - ox) / SPRITE_CHARACTER_SCALE;
		float y0 = (-h + oy) / SPRITE_CHARACTER_SCALE;
		float y1 = oy / SPRITE_CHARACTER_SCALE;
		
		M.vertices = new Vector3[] {
			new Vector3(x1, y0, 0),
			new Vector3(x1, y1, 0),
			new Vector3(x0, y1, 0),
			new Vector3(x0, y0, 0)
		};
	}
	
	void changeUV(int x, int y, int w, int h, int tw, int th) {
		float u0 = x / (float)tw;
		float u1 = (x + w) / (float)tw;
		float v0 = (th - y - h) / (float)th;
		float v1 = (th - y) / (float)th;
		
		M.uv = new Vector2[] {
			new Vector2(u1, v0),
			new Vector2(u1, v1),
			new Vector2(u0, v1),
			new Vector2(u0, v0)
		};
	}

	Mesh M { get { return _filter.sharedMesh; } }
	MeshRenderer R { get { return _renderer; } }
	
	private MeshFilter _filter = null;
	private MeshRenderer _renderer = null;
}
