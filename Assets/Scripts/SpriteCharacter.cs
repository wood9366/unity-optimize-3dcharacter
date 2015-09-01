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
	public class SpriteData {
		public Texture Tex;
		public TextAsset Config;
	}

	public List<SpriteData> SpriteDatas;

	const float SPRITE_CHARACTER_SCALE = 300;

	private class AnimationData {
		public string Name;
		public int StartFrame;
		public int EndFrame;
	}

	private class FrameData {
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

		public AnimationData getAnimData(string name) {
			try {
				return _animDatas[name];
			} catch (System.Exception) {
				return null;
			}
		}

		public FrameData getFrameData(int angleB, int frame) {
			try {
				return _directFrames[angleB][frame];
			} catch (System.Exception) {
				return null;
			}
		}

		private void loadAnimData(string content) {
			StringReader r = new StringReader(content);

			string line = null;

			while ((line = r.ReadLine()) != null) {
				try {
					string[] items = line.Split(',');

					AnimationData animData = new AnimationData();

					animData.Name = items[0];
					animData.StartFrame = int.Parse(items[1]);
					animData.EndFrame = int.Parse(items[2]);

					_animDatas.Add(animData.Name, animData);
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
				Vector4 frame = asRect(data["frame"]);
				Vector2 sourceOffset = asPoint(data["spriteSourceSize"]);
				Vector2 sourceSize = asSize(data["sourceSize"]);

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

		private Vector2 asSize(JSONNode node) {
			return new Vector2(node["w"].AsInt, node["h"].AsInt);
		}

		private Vector2 asPoint(JSONNode node) {
			return new Vector2(node["x"].AsInt, node["y"].AsInt);
		}

		private Vector4 asRect(JSONNode node) {
			return new Vector4(node["x"].AsInt, node["y"].AsInt, node["w"].AsInt, node["h"].AsInt);
		}

		private Dictionary<string, AnimationData> _animDatas = new Dictionary<string, AnimationData>();
		private Dictionary<int, Dictionary<int, FrameData>> _directFrames = new Dictionary<int, Dictionary<int, FrameData>>();
	}

	void updateCharacterSprite(int x, int y, int w, int h, float offsetx, float offsety, int tw, int th) {
//		Debug.Log("update character sprite: " + x + ", " + y + ", " + w + ", " + h + ", " + offsetx + ", " + offsety + ", " + tw + ", " + th);
		changeSize(w, h, offsetx, offsety);
		changeUV(x, y, w, h, tw, th);
	}

	void Awake() {
		_renderer = GetComponent<MeshRenderer>();
		_filter = GetComponent<MeshFilter>();

		generateMesh();

		R.material = new Material(R.material);

		_characterData = new CharacterData();
		_characterData.load(AnimConfig, SpriteDatas);
	}

	void Start() {
		play("idle");
	}

	const int FPS = 30;
	const float FRAME_DURATION = 1.0f / FPS;

	int playAnimationAngleB = 0;

	bool _isPlaying = false;
	float _playTime = 0.0f;
	float _playFrameTime = 0.0f;
	int _playFrame = 0;

	AnimationData _playAnimData = null;

	void play(string name) {
		AnimationData animData = _characterData.getAnimData(name);

		if (animData != null) {
			_playAnimData = animData;
			_playTime = _playFrameTime = 0.0f;
			_playFrame = animData.StartFrame;
			_isPlaying = true;

			updateCharacterFrame();
		}
	}

	void Update() {
		if (_isPlaying) {
			_playTime += Time.deltaTime;
			_playFrameTime += Time.deltaTime;

			if (_playFrameTime > FRAME_DURATION) {
				_playFrameTime -= FRAME_DURATION;
				_playFrame++;

				// loop
				if (_playFrame > _playAnimData.EndFrame) {
					_playFrame = _playAnimData.StartFrame;
				}

				updateCharacterFrame();
			}
		}
	}

	void updateCharacterFrame() {
		FrameData frameData = _characterData.getFrameData(playAnimationAngleB, _playFrame);
		
		if (frameData != null) {
			R.sharedMaterial.mainTexture = frameData.Tex;
			updateCharacterSprite(frameData.x, frameData.y, frameData.w, frameData.h, frameData.pivot.x, frameData.pivot.y, frameData.Tex.width, frameData.Tex.height);
		}
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
	private CharacterData _characterData = null;
}
