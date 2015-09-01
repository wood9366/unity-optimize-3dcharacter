using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System.IO;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SpriteCharacter : MonoBehaviour {
	public TextAsset AnimConfig;

	[System.Serializable]
	public struct SpriteData {
		public Texture Tex;
		public TextAsset Config;
	}

	public List<SpriteData> SpriteDatas;

	private struct AnimationData {
		public string Name;
		public int StartFrame;
		public int EndFrame;
	}

	private struct FrameData {
		public Texture Tex;
		public int FrameNo;
		public int x;
		public int y;
		public int w;
		public int h;
		public Vector2 pivot;
	}

	private class CharacterData {
		public void load(TextAsset animData, List<SpriteData> spriteDatas) {
			_directFrames.Clear();

			foreach (SpriteData spriteData in spriteDatas) {
				loadSpriteData(spriteData);
			}

			_animDatas.Clear();

			loadAnimData(animData.text);
		}

		private void loadAnimData(string content) {
			StringReader r = new StringReader(content);

			string line = null;

			while ((line = r.ReadLine()) != null) {
				try {
					string[] items = line.Split(',');

					AnimationData animData;

					animData.Name = items[0];
					animData.StartFrame = int.Parse(items[1]);
					animData.EndFrame = int.Parse(items[2]);

					_animDatas.Add(animData);
				} catch (System.Exception e) {
					Debug.LogWarning("load character sprite anim data exception: " + e.ToString());
				}
			}
		}

		private void loadSpriteData(SpriteData spriteData) {
			JSONClass SpritesJSON = JSON.Parse(spriteData.Config.text).AsObject;

			foreach (KeyValuePair<string, JSONNode> frameJSON in SpritesJSON["frames"].AsObject) {
				addFrameData(spriteData.Tex, frameJSON.Key, frameJSON.Value);
			}
		}

		private void addFrameData(Texture tex, string name, JSONNode data) {
			try {
				// parse name
//				string characterName = "";
//				int angleA = 0;
				int angleB = 0;
				int frameNo = 0;

				string basename = name;

				int extPos = name.IndexOf('.');
				if (extPos > 0) basename = name.Substring(0, extPos);

				string[] items = basename.Split('_');

//				characterName = items[0];
//				angleA = int.Parse(items[1]);
				angleB = int.Parse(items[2]);
				frameNo = int.Parse(items[3]);

				// parse data
				Vector4 frame = asVector2(data["frame"]);
				Vector2 sourceOffset = asVector2(data["spriteSourceSize"]);
				Vector2 sourceSize = asVector2(data["sourceSize"]);

				// calculate pivot
				Vector2 pivot = new Vector2(sourceSize.x / 2 - sourceOffset.x, sourceSize.y / 2 - sourceOffset.y);

				// create frame data
				FrameData frameData = new FrameData();

				frameData.Tex = tex;
				frameData.FrameNo = frameNo;
				frameData.x = (int)frame.x;
				frameData.y = (int)frame.y;
				frameData.w = (int)frame.z;
				frameData.h = (int)frame.w;
				frameData.pivot = pivot;

				if (_directFrames.ContainsKey(angleB)) {
					if (_directFrames[angleB].ContainsKey(frameNo)) {
						_directFrames[angleB][frameNo] = frameData;
					} else {
						_directFrames[angleB].Add(frameNo, frameData);
					}
				} else {
					Dictionary<int, FrameData> frames = new Dictionary<int, FrameData>();
					frames.Add(frameNo, frameData);

					_directFrames.Add(angleB, frames);
				}
			} catch (System.Exception e) {
				Debug.Log("Load sprite config exception: " + e.ToString());
			}
		}

		private Vector2 asVector2(JSONNode node) {
			return new Vector2(node["x"].AsInt, node["y"].AsInt);
		}

		private Vector4 asVector4(JSONNode node) {
			return new Vector4(node["x"].AsInt, node["y"].AsInt, node["w"].AsInt, node["h"].AsInt);
		}

		private List<AnimationData> _animDatas = new List<AnimationData>();
		private Dictionary<int, Dictionary<int, FrameData>> _directFrames = new Dictionary<int, Dictionary<int, FrameData>>();
	}

	void updateCharacterSprite(int x, int y, int w, int h) {
		changeSize(w / 300.0f, h / 300.0f);
		changeUV(x, y, w, h, 1024, 1024);
	}

	void Awake() {
		_filter = GetComponent<MeshFilter>();

		generateMesh();

		updateCharacterSprite(104, 840, 88, 114);

		_characterData = new CharacterData();
		_characterData.load(AnimConfig, SpriteDatas);
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

		M.uv = new Vector2[] {
			new Vector2(u1, v0),
			new Vector2(u1, v1),
			new Vector2(u0, v1),
			new Vector2(u0, v0)
		};
	}

	Mesh M { get { return _filter.sharedMesh; } }

	private MeshFilter _filter = null;
	private CharacterData _characterData = null;
}
