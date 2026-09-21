using UnityEngine;

public class VMTest : MonoBehaviour
{
	private void Start()
	{
		MonoBehaviour.print("Product Name: " + VersionManager.productName);
		MonoBehaviour.print("Bundle Identifier: " + VersionManager.bundleIdentifier);
		MonoBehaviour.print("Bundle Version: " + VersionManager.bundleVersion);
		MonoBehaviour.print("Bundle Version Code: " + VersionManager.bundleVersionCode);
		MonoBehaviour.print("Full Version: " + VersionManager.fullVersion);
		MonoBehaviour.print("Test Version: " + VersionManager.testVersion);
	}
}
