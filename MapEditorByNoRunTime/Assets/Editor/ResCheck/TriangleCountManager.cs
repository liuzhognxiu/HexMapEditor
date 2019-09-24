using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FY.Tools.ResCheck
{
	public class TriangleCountManager : MonoBehaviour
	{
		private void Awake()
		{
			TriangleCountManager.mgr = this;
		}

		private void Update()
		{
			this.triangleCount = 0;
			foreach (TriangleCounter triangleCounter in this.couterList)
			{
				this.triangleCount += triangleCounter.triangleCount;
			}
		}

		public static void HierarchyWindowItemOnGUI(int instanceId, Rect selectionRect)
		{
			if (!Application.isPlaying)
			{
				return;
			}
			GameObject gameObject = (GameObject)EditorUtility.InstanceIDToObject(instanceId);
			if (gameObject == null)
			{
				return;
			}
			int num = 0;
			foreach (TriangleCounter triangleCounter in gameObject.GetComponentsInChildren<TriangleCounter>())
			{
				num += triangleCounter.triangleCount;
			}
			if (num > 0)
			{
				int num2 = 100;
				Rect rect = new Rect(selectionRect);
				rect.x = selectionRect.xMax - (float)num2;
				rect.width = (float)num2;
				EditorGUI.LabelField(rect, num.ToString());
			}
		}

		public void SetupTriangleCounter(GameObject root)
		{
			MeshFilter[] componentsInChildren = root.GetComponentsInChildren<MeshFilter>(true);
			SkinnedMeshRenderer[] componentsInChildren2 = root.GetComponentsInChildren<SkinnedMeshRenderer>(true);
			foreach (MeshFilter meshFilter in componentsInChildren)
			{
				TriangleCounter triangleCounter = meshFilter.gameObject.GetComponent<TriangleCounter>();
				if (triangleCounter == null)
				{
					triangleCounter = meshFilter.gameObject.AddComponent<TriangleCounter>();
				}
				this.couterList.Add(triangleCounter);
			}
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren2)
			{
				TriangleCounter triangleCounter2 = skinnedMeshRenderer.gameObject.GetComponent<TriangleCounter>();
				if (triangleCounter2 == null)
				{
					triangleCounter2 = skinnedMeshRenderer.gameObject.AddComponent<TriangleCounter>();
				}
				this.couterList.Add(triangleCounter2);
			}
		}

		public void RemoveTriangleCounter(GameObject root)
		{
			foreach (TriangleCounter triangleCounter in root.GetComponentsInChildren<TriangleCounter>(true))
			{
				DestroyImmediate(triangleCounter);
				this.couterList.Remove(triangleCounter);
			}
		}

		public static TriangleCountManager mgr;

		public int triangleCount;

		private List<TriangleCounter> couterList = new List<TriangleCounter>();
	}
}
