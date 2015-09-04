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

	public void updateRotation(Vector3 view) {
		Quaternion rot = Quaternion.identity;
		rot.SetLookRotation(view);

		transform.rotation = rot;
	}

	public void updateCharacterSprite(Material mat, int x, int y, int w, int h, float offsetx, float offsety, int tw, int th) {
		//		Debug.Log("update character sprite: " + x + ", " + y + ", " + w + ", " + h + ", " + offsetx + ", " + offsety + ", " + tw + ", " + th);

		R.sharedMaterial = mat;

		M.MarkDynamic();

		changeSize(w, h, offsetx, offsety);
		changeUV(x, y, w, h, tw, th);

		M.UploadMeshData(false);
	}

	void generateMesh() {
		_filter.sharedMesh = new Mesh();

		M.vertices = new Vector3[4];
		M.uv = new Vector2[4];

		changeSize(1, 1, 0, 0);
		changeUV(0, 0, 1024, 1024, 1024, 1024);
		
		M.triangles = new int[] { 0, 2, 1, 0, 3, 2 };
	}
	
	void changeSize(float w, float h, float ox, float oy) {
		float x0 = -ox / SPRITE_CHARACTER_SCALE;
		float x1 = (w - ox) / SPRITE_CHARACTER_SCALE;
		float y0 = (-h + oy) / SPRITE_CHARACTER_SCALE;
		float y1 = oy / SPRITE_CHARACTER_SCALE;

		Vector3[] vertices = M.vertices;

		vertices[0].Set(x1, y0, 0);
		vertices[1].Set(x1, y1, 0);
		vertices[2].Set(x0, y1, 0);
		vertices[3].Set(x0, y0, 0);

		M.vertices = vertices;
	}
	
	void changeUV(int x, int y, int w, int h, int tw, int th) {
		float u0 = x / (float)tw;
		float u1 = (x + w) / (float)tw;
		float v0 = (th - y - h) / (float)th;
		float v1 = (th - y) / (float)th;

		Vector2[] uv = M.uv;

		uv[0].Set(u1, v0);
		uv[1].Set(u1, v1);
		uv[2].Set(u0, v1);
		uv[3].Set(u0, v0);

		M.uv = uv;
	}

	Mesh M { get { return _filter.sharedMesh; } }
	MeshRenderer R { get { return _renderer; } }
	
	private MeshFilter _filter = null;
	private MeshRenderer _renderer = null;
}
