using System;
using UnityEngine;

// Token: 0x02000434 RID: 1076
public class UIButtonPosition : MonoBehaviour
{
	// Token: 0x06002418 RID: 9240 RVA: 0x000BCAEC File Offset: 0x000BACEC
	private void Start()
	{
		this.sprite = base.GetComponent<UISprite>();
		this.defaultPosition = this.sprite.transform.localPosition;
		this.defaultSize = this.sprite.width;
		EventManager.AddListener("DefaultButton", new EventManager.Callback(this.OnDefaultButton));
		EventManager.AddListener("SaveButton", new EventManager.Callback(this.OnSaveButton));
		TimerManager.In(0.1f, delegate()
		{
			if (PlayerPrefs.HasKey("ButtonX" + this.Button))
			{
				this.sprite.cachedTransform.localPosition = Utils.GetVector3(PlayerPrefs.GetString("ButtonX" + this.Button));
				int @int = PlayerPrefs.GetInt("ButtonSizeX" + this.Button);
				this.sprite.width = @int;
				this.sprite.height = @int;
			}
		});
	}

	// Token: 0x06002419 RID: 9241 RVA: 0x00018A3E File Offset: 0x00016C3E
	private void OnDrag(Vector2 delta)
	{
		this.sprite.cachedTransform.localPosition += new Vector3(delta.x, delta.y, 0f);
	}

	// Token: 0x0600241A RID: 9242 RVA: 0x000BCB74 File Offset: 0x000BAD74
	private void OnDoubleClick()
	{
		this.sprite.width += 5;
		this.sprite.height += 5;
		if ((float)this.sprite.width >= this.Size.y)
		{
			this.sprite.width = (int)this.Size.x;
			this.sprite.height = (int)this.Size.x;
		}
	}

	// Token: 0x0600241B RID: 9243 RVA: 0x00018A73 File Offset: 0x00016C73
	private void OnDefaultButton()
	{
		this.sprite.transform.localPosition = this.defaultPosition;
		this.sprite.width = this.defaultSize;
		this.sprite.height = this.defaultSize;
	}

	// Token: 0x0600241C RID: 9244 RVA: 0x000BCBF0 File Offset: 0x000BADF0
	private void OnSaveButton()
	{
        PlayerPrefs.SetString("ButtonX" + this.Button, this.sprite.cachedTransform.localPosition.ToString());
		PlayerPrefs.SetInt("ButtonSizeX" + this.Button, this.sprite.width);
	}

	// Token: 0x040015E8 RID: 5608
	public string Button;

	// Token: 0x040015E9 RID: 5609
	public Vector2 Size = new Vector2(25f, 80f);

	// Token: 0x040015EA RID: 5610
	private Vector3 defaultPosition;

	// Token: 0x040015EB RID: 5611
	private int defaultSize;

	// Token: 0x040015EC RID: 5612
	private UISprite sprite;
}
