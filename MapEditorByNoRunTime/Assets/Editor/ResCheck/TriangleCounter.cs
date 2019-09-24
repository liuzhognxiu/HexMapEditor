using System;
using UnityEngine;

namespace FY.Tools.ResCheck
{
	public class TriangleCounter : MonoBehaviour
	{
		public int triangleCount
		{
			get
			{
				if (this.rendering)
				{
					return this.triAngles;
				}
				return 0;
			}
		}

		public bool rendering
		{
			get
			{
				return this.isRender;
			}
			set
			{
				this.isRender = value;
				this.OnTrianglesChanged();
			}
		}

		private void OnWillRenderObject()
		{
			this.renderCurrent = Time.time;
		}

		private void Start()
		{
			MeshFilter component = base.GetComponent<MeshFilter>();
			if (component != null)
			{
				this.triAngles = this.GetTriAngleCount(component.sharedMesh);
				return;
			}
			SkinnedMeshRenderer component2 = base.GetComponent<SkinnedMeshRenderer>();
			if (component2 != null)
			{
				this.triAngles = this.GetTriAngleCount(component2.sharedMesh);
			}
		}

		private void Update()
		{
			if (this.renderLast != this.renderCurrent)
			{
				this.rendering = true;
			}
			else
			{
				this.rendering = false;
			}
			this.renderLast = this.renderCurrent;
		}

		private void OnDisable()
		{
			this.rendering = false;
		}

		private int GetTriAngleCount(Mesh mesh)
		{
			if (mesh == null)
			{
				return 0;
			}
			uint num = 0u;
			for (int i = 0; i < mesh.subMeshCount; i++)
			{
				uint num2 = mesh.GetIndexCount(i) / 3u;
				num += num2;
			}
			return (int)num;
		}

		private void OnTrianglesChanged()
		{
		}

		private float renderLast;

		private float renderCurrent;

		private int triAngles;

		private bool isRender;
	}
}
