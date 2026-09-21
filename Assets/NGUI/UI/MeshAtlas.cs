using System;
using System.Linq;
using UnityEngine;

// Token: 0x020000FB RID: 251
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[AddComponentMenu("NGUI/Atlas3D/MeshAtlas")]
[ExecuteInEditMode]
public class MeshAtlas : MonoBehaviour
{
	// Token: 0x170001A5 RID: 421
	// (get) Token: 0x0600093A RID: 2362 RVA: 0x00008F69 File Offset: 0x00007169
	public GameObject cachedGameObject
	{
		get
		{
			if (this.mGameObject == null)
			{
				this.mGameObject = base.gameObject;
			}
			return this.mGameObject;
		}
	}

	// Token: 0x170001A6 RID: 422
	// (get) Token: 0x0600093B RID: 2363 RVA: 0x00008F8B File Offset: 0x0000718B
	public Transform cachedTransform
	{
		get
		{
			if (this.mTransform == null)
			{
				this.mTransform = base.transform;
			}
			return this.mTransform;
		}
	}

	// Token: 0x170001A7 RID: 423
	// (get) Token: 0x0600093C RID: 2364 RVA: 0x00008FAD File Offset: 0x000071AD
	// (set) Token: 0x0600093D RID: 2365 RVA: 0x00008FB5 File Offset: 0x000071B5
	public Vector3 scale
	{
		get
		{
			return this.mScale;
		}
		set
		{
			if (this.mScale != value)
			{
				this.mScale = value;
				this.UpdateVertices();
			}
		}
	}

	// Token: 0x170001A8 RID: 424
	// (get) Token: 0x0600093E RID: 2366 RVA: 0x00008FD2 File Offset: 0x000071D2
	// (set) Token: 0x0600093F RID: 2367 RVA: 0x00008FDA File Offset: 0x000071DA
	public Vector3 pivot
	{
		get
		{
			return this.mPivot;
		}
		set
		{
			if (this.mPivot != value)
			{
				this.mPivot = value;
				this.UpdateVertices();
			}
		}
	}

	// Token: 0x170001A9 RID: 425
	// (get) Token: 0x06000940 RID: 2368 RVA: 0x00008FF7 File Offset: 0x000071F7
	// (set) Token: 0x06000941 RID: 2369 RVA: 0x00008FFF File Offset: 0x000071FF
	public Rect spriteSize
	{
		get
		{
			return this.mSpriteSize;
		}
		set
		{
			if (this.mSpriteSize != value)
			{
				this.mSpriteSize = value;
				this.UpdateUVs();
			}
		}
	}

	// Token: 0x170001AA RID: 426
	// (get) Token: 0x06000942 RID: 2370 RVA: 0x0000901C File Offset: 0x0000721C
	// (set) Token: 0x06000943 RID: 2371 RVA: 0x00009024 File Offset: 0x00007224
	public bool mirrorX
	{
		get
		{
			return this.mMirrorX;
		}
		set
		{
			if (this.mMirrorX != value)
			{
				this.mMirrorX = value;
				this.UpdateVertices();
			}
		}
	}

	// Token: 0x170001AB RID: 427
	// (get) Token: 0x06000944 RID: 2372 RVA: 0x0000903C File Offset: 0x0000723C
	// (set) Token: 0x06000945 RID: 2373 RVA: 0x00009044 File Offset: 0x00007244
	public bool mirrorY
	{
		get
		{
			return this.mMirrorY;
		}
		set
		{
			if (this.mMirrorY != value)
			{
				this.mMirrorY = value;
				this.UpdateVertices();
			}
		}
	}

	// Token: 0x170001AC RID: 428
	// (get) Token: 0x06000946 RID: 2374 RVA: 0x0000905C File Offset: 0x0000725C
	// (set) Token: 0x06000947 RID: 2375 RVA: 0x00009064 File Offset: 0x00007264
	public bool mirrorZ
	{
		get
		{
			return this.mMirrorZ;
		}
		set
		{
			if (this.mMirrorZ != value)
			{
				this.mMirrorZ = value;
				this.UpdateVertices();
			}
		}
	}

	// Token: 0x170001AD RID: 429
	// (get) Token: 0x06000948 RID: 2376 RVA: 0x0000907C File Offset: 0x0000727C
	// (set) Token: 0x06000949 RID: 2377 RVA: 0x00009084 File Offset: 0x00007284
	public bool flip
	{
		get
		{
			return this.mFlip;
		}
		set
		{
			if (this.mFlip != value)
			{
				this.mFlip = value;
				this.UpdateFlip();
			}
		}
	}

	// Token: 0x170001AE RID: 430
	// (get) Token: 0x0600094A RID: 2378 RVA: 0x0000909C File Offset: 0x0000729C
	// (set) Token: 0x0600094B RID: 2379 RVA: 0x000090A4 File Offset: 0x000072A4
	public Color color
	{
		get
		{
			return this.mColor;
		}
		set
		{
			if (this.mColor != value)
			{
				this.mColor = value;
				this.UpdateColor();
			}
		}
	}

	// Token: 0x170001AF RID: 431
	// (get) Token: 0x0600094C RID: 2380 RVA: 0x000090C1 File Offset: 0x000072C1
	public MeshFilter meshFilter
	{
		get
		{
			if (this.mMeshFilter == null)
			{
				this.mMeshFilter = base.GetComponent<MeshFilter>();
			}
			return this.mMeshFilter;
		}
	}

	// Token: 0x170001B0 RID: 432
	// (get) Token: 0x0600094D RID: 2381 RVA: 0x000090E3 File Offset: 0x000072E3
	public MeshRenderer meshRenderer
	{
		get
		{
			if (this.mMeshRenderer == null)
			{
				this.mMeshRenderer = base.GetComponent<MeshRenderer>();
			}
			return this.mMeshRenderer;
		}
	}

	// Token: 0x170001B1 RID: 433
	// (get) Token: 0x0600094E RID: 2382 RVA: 0x00009105 File Offset: 0x00007305
	// (set) Token: 0x0600094F RID: 2383 RVA: 0x0004CB28 File Offset: 0x0004AD28
	public UIAtlas atlas
	{
		get
		{
			return this.mAtlas;
		}
		set
		{
			if (this.mAtlas != value)
			{
				this.mAtlas = value;
				if (this.mAtlas != null)
				{
					MeshAtlas.lastAtlas = value;
					this.atlasMaterial = this.mAtlas.spriteMaterial;
					this.meshRenderer.sharedMaterial = this.atlasMaterial;
					if (string.IsNullOrEmpty(this.mSpriteName))
					{
						this.spriteName = MeshAtlas.lastSpriteName;
					}
					else
					{
						this.UpdateUVs();
					}
				}
			}
		}
	}

	// Token: 0x170001B2 RID: 434
	// (get) Token: 0x06000950 RID: 2384 RVA: 0x0000910D File Offset: 0x0000730D
	// (set) Token: 0x06000951 RID: 2385 RVA: 0x00009115 File Offset: 0x00007315
	public string spriteName
	{
		get
		{
			return this.mSpriteName;
		}
		set
		{
			if (this.mSpriteName != value)
			{
				this.mSpriteName = value;
				MeshAtlas.lastSpriteName = value;
				this.UpdateUVs();
			}
		}
	}

	// Token: 0x06000952 RID: 2386 RVA: 0x00009138 File Offset: 0x00007338
	private void OnEnable()
	{
		this.UpdateMesh();
	}

	// Token: 0x06000953 RID: 2387 RVA: 0x00009140 File Offset: 0x00007340
	private void Reset()
	{
		this.DisableMesh();
		if (Application.isEditor)
		{
			this.UpdateMesh();
		}
	}

	// Token: 0x06000954 RID: 2388 RVA: 0x0004CBA0 File Offset: 0x0004ADA0
	public void UpdateMesh()
	{
		if (this.originalMesh == null)
		{
			this.originalMesh = this.meshFilter.mesh;
			this.originalMaterial = this.meshFilter.GetComponent<Renderer>().sharedMaterial;
			if (this.atlas == null && MeshAtlas.lastAtlas != null)
			{
				this.atlas = MeshAtlas.lastAtlas;
			}
		}
		if (base.enabled)
		{
			this.EnableMesh();
		}
		else
		{
			this.DisableMesh();
		}
	}

	// Token: 0x06000955 RID: 2389 RVA: 0x0004CC20 File Offset: 0x0004AE20
	public void EnableMesh()
	{
		if (this.atlasMesh != null)
		{
			return;
		}
		this.atlasMesh = new Mesh();
		this.atlasMesh.name = this.originalMesh.name + "_Atlas";
		this.UpdateVertices();
		this.UpdateFlip();
		this.atlasMesh.uv2 = this.originalMesh.uv2;
		this.atlasMesh.normals = this.originalMesh.normals;
		this.UpdateColor();
		this.atlasMesh.tangents = this.originalMesh.tangents;
		this.UpdateUVs();
		if (this.atlasMaterial == null)
		{
			this.atlasMaterial = this.atlas.spriteMaterial;
		}
		this.meshFilter.GetComponent<Renderer>().sharedMaterial = this.atlasMaterial;
		this.meshFilter.mesh = this.atlasMesh;
	}

	// Token: 0x06000956 RID: 2390 RVA: 0x0004CD08 File Offset: 0x0004AF08
	public void DisableMesh()
	{
		if (this.atlasMesh != null)
		{
			this.atlasMaterial = this.meshFilter.GetComponent<Renderer>().sharedMaterial;
			UnityEngine.Object.DestroyImmediate(this.atlasMesh);
			this.atlasMesh = null;
		}
		if (this.originalMesh != null)
		{
			this.meshFilter.mesh = this.originalMesh;
		}
		if (this.originalMaterial != null)
		{
			this.meshFilter.GetComponent<Renderer>().sharedMaterial = this.originalMaterial;
		}
	}

	// Token: 0x06000957 RID: 2391 RVA: 0x0004CD90 File Offset: 0x0004AF90
	public void UpdateSettings()
	{
		this.UpdateVertices();
		this.UpdateFlip();
		this.atlasMesh.uv2 = this.originalMesh.uv2;
		this.atlasMesh.normals = this.originalMesh.normals;
		this.UpdateColor();
		this.atlasMesh.tangents = this.originalMesh.tangents;
		this.UpdateUVs();
	}

	// Token: 0x06000958 RID: 2392 RVA: 0x0004CDF8 File Offset: 0x0004AFF8
	private void UpdateUVs()
	{
		if (this.atlasMesh == null || this.atlas == null || string.IsNullOrEmpty(this.spriteName))
		{
			return;
		}
		UISpriteData sprite = this.atlas.GetSprite(this.spriteName);
		if (sprite != null && !(this.atlas.texture == null))
		{
			Rect rect = new Rect((float)sprite.x, (float)sprite.y, (float)sprite.width, (float)sprite.height);
			Rect rect2 = NGUIMath.ConvertToTexCoords(rect, this.atlas.texture.width, this.atlas.texture.height);
			Vector2[] uv = this.originalMesh.uv;
			for (int i = 0; i < uv.Length; i++)
			{
				uv[i].x = uv[i].x * rect2.width * this.spriteSize.width + rect2.x + this.spriteSize.x;
				uv[i].y = uv[i].y * rect2.height * this.spriteSize.height + rect2.y + this.spriteSize.y;
			}
			this.atlasMesh.uv = uv;
			return;
		}
	}

	// Token: 0x06000959 RID: 2393 RVA: 0x0004CF78 File Offset: 0x0004B178
	private void UpdateFlip()
	{
		if (this.atlasMesh == null)
		{
			return;
		}
		if (this.flip)
		{
			this.atlasMesh.triangles = this.originalMesh.triangles.Reverse<int>().ToArray<int>();
		}
		else
		{
			this.atlasMesh.triangles = this.originalMesh.triangles;
		}
	}

	// Token: 0x0600095A RID: 2394 RVA: 0x0004CFD4 File Offset: 0x0004B1D4
	private void UpdateVertices()
	{
		if (this.atlasMesh == null)
		{
			return;
		}
		Vector3[] vertices = this.originalMesh.vertices;
		for (int i = 0; i < vertices.Length; i++)
		{
			vertices[i].x = (vertices[i].x + this.mPivot.x) * this.mScale.x * (float)((!this.mMirrorX) ? 1 : -1);
			vertices[i].y = (vertices[i].y + this.mPivot.y) * this.mScale.y * (float)((!this.mMirrorY) ? 1 : -1);
			vertices[i].z = (vertices[i].z + this.mPivot.z) * this.mScale.z * (float)((!this.mMirrorZ) ? 1 : -1);
		}
		this.atlasMesh.vertices = vertices;
	}

	// Token: 0x0600095B RID: 2395 RVA: 0x0004D0D8 File Offset: 0x0004B2D8
	private void UpdateColor()
	{
		if (this.atlasMesh == null)
		{
			return;
		}
		Color[] array = new Color[this.atlasMesh.uv.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = this.mColor;
		}
		this.atlasMesh.colors = array;
	}

	// Token: 0x04000668 RID: 1640
	public static UIAtlas lastAtlas;

	// Token: 0x04000669 RID: 1641
	public static string lastSpriteName;

	// Token: 0x0400066A RID: 1642
	public Mesh originalMesh;

	// Token: 0x0400066B RID: 1643
	private Mesh atlasMesh;

	// Token: 0x0400066C RID: 1644
	private MeshFilter mMeshFilter;

	// Token: 0x0400066D RID: 1645
	private MeshRenderer mMeshRenderer;

	// Token: 0x0400066E RID: 1646
	public Vector3 mScale = Vector3.one;

	// Token: 0x0400066F RID: 1647
	public Vector3 mPivot = Vector3.zero;

	// Token: 0x04000670 RID: 1648
	public Rect mSpriteSize = new Rect(0f, 0f, 1f, 1f);

	// Token: 0x04000671 RID: 1649
	public bool mMirrorX;

	// Token: 0x04000672 RID: 1650
	public bool mMirrorY;

	// Token: 0x04000673 RID: 1651
	public bool mMirrorZ;

	// Token: 0x04000674 RID: 1652
	public bool mFlip;

	// Token: 0x04000675 RID: 1653
	public Color mColor = Color.white;

	// Token: 0x04000676 RID: 1654
	public Material originalMaterial;

	// Token: 0x04000677 RID: 1655
	public Material atlasMaterial;

	// Token: 0x04000678 RID: 1656
	public UIAtlas mAtlas;

	// Token: 0x04000679 RID: 1657
	public string mSpriteName;

	// Token: 0x0400067A RID: 1658
	private GameObject mGameObject;

	// Token: 0x0400067B RID: 1659
	private Transform mTransform;
}
