using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terraforming : MonoBehaviour
{
	[SerializeField]
	ChunkManager ChunkManager;

	Vector3 _hitPoint;
	Camera _cam;

	private void Awake() {
		_cam = GetComponent<Camera>();
	}

	public float BrushSize = 2;

	private void LateUpdate() {
		if (Input.GetMouseButton(0)) {
			RaycastHit hit;

			if (Physics.Raycast(_cam.ScreenPointToRay(Input.mousePosition), out hit, 1000)) {
				ChunkManager.EditWeights(hit.point, BrushSize);
			}
		}
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(_hitPoint, BrushSize);
	}
}
