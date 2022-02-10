using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainEdit : MonoBehaviour
{
	Vector3 hitPoint;

	private void LateUpdate() {

		if (Input.GetMouseButton(0)) {
			RaycastHit hit;


			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000)) {
				hitPoint = hit.point;

				var chunk = hit.collider.GetComponentInParent<Chunk>();
				chunk.EditWeights(hitPoint);
			}
		}
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(hitPoint, 4);
	}
}
