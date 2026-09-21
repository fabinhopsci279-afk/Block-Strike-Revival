public class Test : TimerBehaviour
{
	private void Start()
	{
		EventManager.AddListener<object[]>("Test", Tes);
		EventManager.Dispatch("Test", new object[2]
		{
			32,
			2.5f
		});
	}

	private void Tes(object[] w)
	{
	}
}
