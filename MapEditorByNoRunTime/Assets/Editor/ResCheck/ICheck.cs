using System;
using System.Collections.Generic;
using UnityEngine;

namespace FY.Tools.ResCheck
{
	public interface ICheck
	{
		void Start();

		void Stop();

		bool CheckMatch(string asset);

		bool DoCheck(string asset);

		void DoFix(string asset);

		bool DoCheckHierarchy(string asset, params GameObject[] rootObjs);

		void DoFixHierarchy(string asset, params GameObject[] rootObjs);

		List<Drawing> DrawInfo();

		void SaveToFile(string fileName);

		bool IsEmpty();
	}
}
