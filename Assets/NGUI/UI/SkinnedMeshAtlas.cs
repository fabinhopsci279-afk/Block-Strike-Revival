using System;
using UnityEngine;

// Token: 0x020000FC RID: 252
[AddComponentMenu("NGUI/Atlas3D/SkinnedMeshAtlas")]
[ExecuteInEditMode]
[RequireComponent(typeof(SkinnedMeshRenderer))]
public class SkinnedMeshAtlas : MonoBehaviour
{
	// Token: 0x0600095D RID: 2397 RVA: 0x00009155 File Offset: 0x00007355
	private void OnEnable()
	{
		this.UpdateMesh();
	}

	// Token: 0x0600095E RID: 2398 RVA: 0x0000915D File Offset: 0x0000735D
	private void Reset()
	{
		this.DisableMesh();
		if (Application.isEditor)
		{
			this.UpdateMesh();
		}
	}

	// Token: 0x170001B3 RID: 435
	// (get) Token: 0x0600095F RID: 2399 RVA: 0x00009172 File Offset: 0x00007372
	public SkinnedMeshRenderer mf
	{
		get
		{
			if (this._mf == null)
			{
				this._mf = base.gameObject.GetComponent<SkinnedMeshRenderer>();
			}
			return this._mf;
		}
	}

	// Token: 0x06000960 RID: 2400 RVA: 0x0004D134 File Offset: 0x0004B334
	public void UpdateMesh()
	{
		if (this.originalMesh == null)
		{
			this.originalMesh = base.gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh;
			this.originalMaterial = this.mf.GetComponent<Renderer>().sharedMaterial;
			if (this.atlas == null && SkinnedMeshAtlas.lastUsedAtlas != null)
			{
				this.atlas = SkinnedMeshAtlas.lastUsedAtlas;
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

	// Token: 0x06000961 RID: 2401 RVA: 0x0004D1B8 File Offset: 0x0004B3B8
	public void EnableMesh()
	{
		if (this.mesh == null)
		{
			this.mesh = (Mesh)UnityEngine.Object.Instantiate(this.originalMesh);
			this.mesh.name = this.originalMesh.name + "_Atlas";
			this.UpdateUVs();
			if (this.customMaterial != null)
			{
				this.mf.GetComponent<Renderer>().sharedMaterial = this.customMaterial;
			}
			this.mf.sharedMesh = this.mesh;
		}
	}

	// Token: 0x06000962 RID: 2402 RVA: 0x0004D244 File Offset: 0x0004B444
	public void DisableMesh()
	{
		if (this.mesh != null)
		{
			this.customMaterial = this.mf.GetComponent<Renderer>().sharedMaterial;
			UnityEngine.Object.DestroyImmediate(this.mesh);
			this.mesh = null;
		}
		if (this.originalMesh != null)
		{
			this.mf.sharedMesh = this.originalMesh;
		}
		if (this.originalMaterial != null)
		{
			this.mf.GetComponent<Renderer>().sharedMaterial = this.originalMaterial;
		}
	}

	// Token: 0x06000963 RID: 2403 RVA: 0x0004D2CC File Offset: 0x0004B4CC
	public void UpdateUVs()
	{
		if (this.mesh == null)
		{
			return;
		}
		if (this.atlas == null)
		{
			return;
		}
		if (string.IsNullOrEmpty(this.spriteName))
		{
			return;
		}
		UISpriteData sprite = this.atlas.GetSprite(this.spriteName);
		if (sprite == null)
		{
			return;
		}
		if (this.atlas.texture == null)
		{
			return;
		}
		this.mSprite = sprite;
		Rect rect = new Rect((float)this.mSprite.x, (float)this.mSprite.y, (float)this.mSprite.width, (float)this.mSprite.height);
		Rect rect2 = NGUIMath.ConvertToTexCoords(rect, this.atlas.texture.width, this.atlas.texture.height);
		Vector2[] uv = this.originalMesh.uv;
		for (int i = 0; i < uv.Length; i++)
		{
			uv[i].x = uv[i].x * rect2.width + rect2.x;
			uv[i].y = uv[i].y * rect2.height + rect2.y;
		}
		this.mesh.uv = uv;
	}

	// Token: 0x170001B4 RID: 436
	// (get) Token: 0x06000964 RID: 2404 RVA: 0x00009199 File Offset: 0x00007399
	// (set) Token: 0x06000965 RID: 2405 RVA: 0x0004D414 File Offset: 0x0004B614
	[SerializeField]
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
				this.mSpriteSet = false;
				this.mSprite = null;
				if (this.mAtlas != null)
				{
					SkinnedMeshAtlas.lastUsedAtlas = value;
					this.customMaterial = this.mAtlas.spriteMaterial;
					this.mf.GetComponent<Renderer>().sharedMaterial = this.customMaterial;
					if (this.originalMaterial != null && !string.IsNullOrEmpty(this.originalMaterial.name))
					{
						for (int i = 0; i < this.mAtlas.spriteList.Count; i++)
						{
							if (this.mAtlas.spriteList[i].name == this.originalMaterial.name)
							{
								this.SetAtlasSprite(this.mAtlas.spriteList[i]);
								this.mSpriteName = this.mSprite.name;
								break;
							}
						}
					}
					if (string.IsNullOrEmpty(this.mSpriteName) && this.mAtlas != null && this.mAtlas.spriteList.Count > 0)
					{
						this.SetAtlasSprite(this.mAtlas.spriteList[0]);
						this.mSpriteName = this.mSprite.name;
					}
					if (!string.IsNullOrEmpty(this.mSpriteName))
					{
						string spriteName = this.mSpriteName;
						this.mSpriteName = string.Empty;
						this.spriteName = spriteName;
					}
				}
			}
		}
	}

	// Token: 0x170001B5 RID: 437
	// (get) Token: 0x06000966 RID: 2406 RVA: 0x000091A1 File Offset: 0x000073A1
	// (set) Token: 0x06000967 RID: 2407 RVA: 0x0004D590 File Offset: 0x0004B790
	[SerializeField]
	public string spriteName
	{
		get
		{
			return this.mSpriteName;
		}
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				if (string.IsNullOrEmpty(this.mSpriteName))
				{
					return;
				}
				this.mSpriteName = string.Empty;
				this.mSprite = null;
				this.mSpriteSet = false;
			}
			else if (this.mSpriteName != value)
			{
				this.mSpriteName = value;
				this.mSprite = null;
				this.mSpriteSet = false;
				this.UpdateUVs();
			}
		}
	}

	// Token: 0x170001B6 RID: 438
	// (get) Token: 0x06000968 RID: 2408 RVA: 0x000091A9 File Offset: 0x000073A9
	public bool isValid
	{
		get
		{
			return this.GetAtlasSprite() != null;
		}
	}

	// Token: 0x06000969 RID: 2409 RVA: 0x0004D5F8 File Offset: 0x0004B7F8
	public UISpriteData GetAtlasSprite()
	{
		if (!this.mSpriteSet)
		{
			this.mSprite = null;
		}
		if (this.mSprite == null && this.mAtlas != null)
		{
			if (!string.IsNullOrEmpty(this.mSpriteName))
			{
				UISpriteData sprite = this.mAtlas.GetSprite(this.mSpriteName);
				if (sprite == null)
				{
					return null;
				}
				this.SetAtlasSprite(sprite);
			}
			if (this.mSprite == null && this.mAtlas.spriteList.Count > 0)
			{
				UISpriteData uispriteData = this.mAtlas.spriteList[0];
				if (uispriteData == null)
				{
					return null;
				}
				this.SetAtlasSprite(uispriteData);
				if (this.mSprite == null)
				{
					Debug.LogError(this.mAtlas.name + " seems to have a null sprite!");
					return null;
				}
				this.mSpriteName = this.mSprite.name;
			}
		}
		return this.mSprite;
	}

	// Token: 0x0600096A RID: 2410 RVA: 0x0004D6D0 File Offset: 0x0004B8D0
	private void SetAtlasSprite(UISpriteData sp)
	{
		this.mSpriteSet = true;
		if (sp != null)
		{
			this.mSprite = sp;
			this.mSpriteName = this.mSprite.name;
		}
		else
		{
			this.mSpriteName = ((this.mSprite == null) ? string.Empty : this.mSprite.name);
			this.mSprite = sp;
		}
	}

	// Token: 0x0600096B RID: 2411 RVA: 0x0004D728 File Offset: 0x0004B928
	public void UpdateAllMeshes()
	{
		MeshAtlas[] array = NGUITools.FindActive<MeshAtlas>();
		int i = 0;
		int num = array.Length;
		while (i < num)
		{
			MeshAtlas meshAtlas = array[i];
			if (meshAtlas.enabled && this.originalMesh == meshAtlas.originalMesh)
			{
				meshAtlas.UpdateMesh();
			}
			i++;
		}
	}

	// Token: 0x0600096C RID: 2412 RVA: 0x0004D770 File Offset: 0x0004B970
	public void UpdateMeshTextures()
	{
		MeshAtlas[] array = NGUITools.FindActive<MeshAtlas>();
		int i = 0;
		int num = array.Length;
		while (i < num)
		{
			MeshAtlas meshAtlas = array[i];
			if (UIAtlas.CheckIfRelated(this.atlas, meshAtlas.atlas))
			{
				meshAtlas.UpdateMesh();
			}
			i++;
		}
	}

	// Token: 0x0400067C RID: 1660
	public static UIAtlas lastUsedAtlas;

	// Token: 0x0400067D RID: 1661
	public Mesh originalMesh;

	// Token: 0x0400067E RID: 1662
	private SkinnedMeshRenderer _mf;

	// Token: 0x0400067F RID: 1663
	private Mesh mesh;

	// Token: 0x04000680 RID: 1664
	public Material originalMaterial;

	// Token: 0x04000681 RID: 1665
	public Material customMaterial;

	// Token: 0x04000682 RID: 1666
	public UIAtlas mAtlas;

	// Token: 0x04000683 RID: 1667
	public string mSpriteName;

	// Token: 0x04000684 RID: 1668
	public UISpriteData mSprite;

	// Token: 0x04000685 RID: 1669
	private bool mSpriteSet;
}
