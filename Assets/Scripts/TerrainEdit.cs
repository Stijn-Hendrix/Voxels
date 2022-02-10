using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainEdit : MonoBehaviour
{
	private Vector3 _hitPoint;

	public float BrushSize = 2;

	private void LateUpdate() {
		if (Input.GetMouseButton(0)) {
			RaycastHit hit;

			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000)) {
				ChunkManager.instance.EditWeights(hit.point, BrushSize);
			}
		}
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(_hitPoint, BrushSize);
	}
}
