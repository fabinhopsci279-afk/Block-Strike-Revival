using System;
using System.Linq;
using UnityEngine;

// Token: 0x02000533 RID: 1331
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[ExecuteInEditMode]
public class MeshEditor : MonoBehaviour
{
	// Token: 0x1700040C RID: 1036
	// (get) Token: 0x06002B80 RID: 11136 RVA: 0x0001D931 File Offset: 0x0001BB31
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

	// Token: 0x1700040D RID: 1037
	// (get) Token: 0x06002B81 RID: 11137 RVA: 0x0001D953 File Offset: 0x0001BB53
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

	// Token: 0x1700040E RID: 1038
	// (get) Token: 0x06002B82 RID: 11138 RVA: 0x0001D975 File Offset: 0x0001BB75
	// (set) Token: 0x06002B83 RID: 11139 RVA: 0x0001D97D File Offset: 0x0001BB7D
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

	// Token: 0x1700040F RID: 1039
	// (get) Token: 0x06002B84 RID: 11140 RVA: 0x0001D99A File Offset: 0x0001BB9A
	// (set) Token: 0x06002B85 RID: 11141 RVA: 0x0001D9A2 File Offset: 0x0001BBA2
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

	// Token: 0x17000410 RID: 1040
	// (get) Token: 0x06002B86 RID: 11142 RVA: 0x0001D9BF File Offset: 0x0001BBBF
	// (set) Token: 0x06002B87 RID: 11143 RVA: 0x0001D9C7 File Offset: 0x0001BBC7
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

	// Token: 0x17000411 RID: 1041
	// (get) Token: 0x06002B88 RID: 11144 RVA: 0x0001D9DF File Offset: 0x0001BBDF
	// (set) Token: 0x06002B89 RID: 11145 RVA: 0x0001D9E7 File Offset: 0x0001BBE7
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

	// Token: 0x17000412 RID: 1042
	// (get) Token: 0x06002B8A RID: 11146 RVA: 0x0001D9FF File Offset: 0x0001BBFF
	// (set) Token: 0x06002B8B RID: 11147 RVA: 0x0001DA07 File Offset: 0x0001BC07
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

	// Token: 0x17000413 RID: 1043
	// (get) Token: 0x06002B8C RID: 11148 RVA: 0x0001DA1F File Offset: 0x0001BC1F
	// (set) Token: 0x06002B8D RID: 11149 RVA: 0x0001DA27 File Offset: 0x0001BC27
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

	// Token: 0x17000414 RID: 1044
	// (get) Token: 0x06002B8E RID: 11150 RVA: 0x0001DA3F File Offset: 0x0001BC3F
	// (set) Token: 0x06002B8F RID: 11151 RVA: 0x0001DA47 File Offset: 0x0001BC47
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

	// Token: 0x17000415 RID: 1045
	// (get) Token: 0x06002B90 RID: 11152 RVA: 0x0001DA64 File Offset: 0x0001BC64
	// (set) Token: 0x06002B91 RID: 11153 RVA: 0x0001DA6C File Offset: 0x0001BC6C
	public Rect uvRect
	{
		get
		{
			return this.mUVRect;
		}
		set
		{
			if (this.mUVRect != value)
			{
				this.mUVRect = value;
				this.UpdateUV();
			}
		}
	}

	// Token: 0x17000416 RID: 1046
	// (get) Token: 0x06002B92 RID: 11154 RVA: 0x0001DA89 File Offset: 0x0001BC89
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

	// Token: 0x17000417 RID: 1047
	// (get) Token: 0x06002B93 RID: 11155 RVA: 0x0001DAAB File Offset: 0x0001BCAB
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

	// Token: 0x06002B94 RID: 11156 RVA: 0x0001DACD File Offset: 0x0001BCCD
	private void OnEnable()
	{
		this.UpdateMesh();
	}

	// Token: 0x06002B95 RID: 11157 RVA: 0x0001DAD5 File Offset: 0x0001BCD5
	private void Reset()
	{
		this.DisableMesh();
		if (Application.isEditor)
		{
			this.UpdateMesh();
		}
	}

	// Token: 0x06002B96 RID: 11158 RVA: 0x0001DAEA File Offset: 0x0001BCEA
	public void UpdateMesh()
	{
		if (this.originalMesh == null)
		{
			this.originalMesh = this.meshFilter.mesh;
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

	// Token: 0x06002B97 RID: 11159 RVA: 0x000DB4F8 File Offset: 0x000D96F8
	public void EnableMesh()
	{
		this.editMesh = new Mesh();
		this.editMesh.name = this.originalMesh.name + "_Editor";
		this.UpdateVertices();
		this.UpdateFlip();
		this.editMesh.uv = this.originalMesh.uv;
		this.editMesh.uv2 = this.originalMesh.uv2;
		this.editMesh.normals = this.originalMesh.normals;
		this.editMesh.tangents = this.originalMesh.tangents;
		this.UpdateUV();
		this.UpdateColor();
		this.meshFilter.mesh = this.editMesh;
	}

	// Token: 0x06002B98 RID: 11160 RVA: 0x000DB5B4 File Offset: 0x000D97B4
	public void DisableMesh()
	{
		if (this.editMesh != null)
		{
			UnityEngine.Object.DestroyImmediate(this.editMesh);
			this.editMesh = null;
		}
		if (this.originalMesh != null)
		{
			this.meshFilter.mesh = this.originalMesh;
		}
	}

	// Token: 0x06002B99 RID: 11161 RVA: 0x0001DB21 File Offset: 0x0001BD21
	public void UpdateSettings()
	{
		this.UpdateVertices();
		this.UpdateFlip();
		this.UpdateUV();
		this.UpdateColor();
	}

	// Token: 0x06002B9A RID: 11162 RVA: 0x000DB600 File Offset: 0x000D9800
	private void UpdateFlip()
	{
		if (this.editMesh == null)
		{
			return;
		}
		if (this.flip)
		{
			this.editMesh.triangles = this.originalMesh.triangles.Reverse<int>().ToArray<int>();
		}
		else
		{
			this.editMesh.triangles = this.originalMesh.triangles;
		}
	}

	// Token: 0x06002B9B RID: 11163 RVA: 0x000DB65C File Offset: 0x000D985C
	private void UpdateVertices()
	{
		if (this.editMesh == null)
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
		this.editMesh.vertices = vertices;
	}

	// Token: 0x06002B9C RID: 11164 RVA: 0x000DB760 File Offset: 0x000D9960
	private void UpdateUV()
	{
		if (this.editMesh == null)
		{
			return;
		}
		Vector2[] uv = this.originalMesh.uv;
		for (int i = 0; i < uv.Length; i++)
		{
			uv[i].x = uv[i].x * this.uvRect.width + this.uvRect.x;
			uv[i].y = uv[i].y * this.uvRect.height + this.uvRect.y;
		}
		this.editMesh.uv = uv;
	}

	// Token: 0x06002B9D RID: 11165 RVA: 0x000DB810 File Offset: 0x000D9A10
	private void UpdateColor()
	{
		if (this.editMesh == null)
		{
			return;
		}
		Color[] array = new Color[this.originalMesh.uv.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = this.mColor;
		}
		this.editMesh.colors = array;
	}

	// Token: 0x04001BAF RID: 7087
	public Mesh originalMesh;

	// Token: 0x04001BB0 RID: 7088
	private Mesh editMesh;

	// Token: 0x04001BB1 RID: 7089
	private MeshFilter mMeshFilter;

	// Token: 0x04001BB2 RID: 7090
	public Vector3 mScale = Vector3.one;

	// Token: 0x04001BB3 RID: 7091
	public Vector3 mPivot = Vector3.zero;

	// Token: 0x04001BB4 RID: 7092
	public bool mMirrorX;

	// Token: 0x04001BB5 RID: 7093
	public bool mMirrorY;

	// Token: 0x04001BB6 RID: 7094
	public bool mMirrorZ;

	// Token: 0x04001BB7 RID: 7095
	public bool mFlip;

	// Token: 0x04001BB8 RID: 7096
	public Rect mUVRect = new Rect(0f, 0f, 1f, 1f);

	// Token: 0x04001BB9 RID: 7097
	public Color mColor = Color.white;

	// Token: 0x04001BBA RID: 7098
	private GameObject mGameObject;

	// Token: 0x04001BBB RID: 7099
	private Transform mTransform;

	// Token: 0x04001BBC RID: 7100
	private MeshRenderer mMeshRenderer;
}
