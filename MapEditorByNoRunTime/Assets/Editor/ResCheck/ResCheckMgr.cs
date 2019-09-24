using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FY.Tools.ResCheck
{
	public class ResCheckMgr
	{
		public static ResCheckMgr inst
		{
			get
			{
				if (ResCheckMgr._inst == null)
				{
					ResCheckMgr._inst = new ResCheckMgr();
				}
				return ResCheckMgr._inst;
			}
		}

		private ResCheckMgr()
		{
			foreach (KeyValuePair<int, Type> keyValuePair in Const.CHECKER_MAP)
			{
				this.checkers.Add(keyValuePair.Key, (ICheck)Activator.CreateInstance(keyValuePair.Value));
			}
		}

		public void ResetCheckFlag()
		{
			this.selectionPattern = string.Empty;
			this.checkFlag.Clear();
		}

		public void EnableCheck(params int[] masks)
		{
			foreach (int item in masks)
			{
				this.checkFlag.Add(item);
			}
		}

		public void DisableCheck(int mask)
		{
			this.checkFlag.Remove(mask);
		}

		public bool NeedCheck(params int[] masks)
		{
			foreach (int item in masks)
			{
				if (this.checkFlag.Contains(item))
				{
					return true;
				}
			}
			return false;
		}

		public void StartCheck()
		{
			this.checkMode = CheckMode.CHECK_ONLY;
			this.DoCheckAndFix();
		}

		public void StartFix()
		{
			this.checkMode = CheckMode.CHECK_FIX;
			this.DoCheckAndFix();
		}

		public void StartCheckSelection()
		{
			int num = 0;
            UnityEngine.Object[] objects = Selection.objects;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("(");
			foreach (UnityEngine.Object @object in objects)
			{
				if (num != 0)
				{
					stringBuilder.Append("|");
				}
				string text = AssetDatabase.GetAssetPath(@object);
				if (!Path.HasExtension(text))
				{
					text += "/";
				}
				stringBuilder.Append(text);
				num++;
			}
			stringBuilder.Append(")");
			this.selectionPattern = stringBuilder.ToString();
			this.checkMode = CheckMode.CHECK_ONLY;
			this.DoCheckAndFix();
		}

		public Dictionary<string, List<string>> resDependReverse
		{
			get
			{
				this.InitResDependReverse();
				return this._resDependReverse;
			}
		}

		private void DoCheckAndFix()
		{
			this.PreCheck();
			string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
			for (int i = 0; i < allAssetPaths.Length; i++)
			{
				string text = allAssetPaths[i];
                UnityEngine.Debug.Log(text);
				if ((string.IsNullOrEmpty(this.selectionPattern) || Regex.IsMatch(text, this.selectionPattern)) && !ResCheckWhiteList.IsInWhiteList(text))
				{
					foreach (KeyValuePair<int, ICheck> keyValuePair in this.checkers)
					{
						if (this.NeedCheck(new int[]
						{
							keyValuePair.Key
						}))
						{
							ICheck value = keyValuePair.Value;
							if (value.CheckMatch(text) && (Directory.Exists(text) || File.Exists(text)))
							{
								if (this.checkMode == CheckMode.CHECK_ONLY)
								{
									if (!this.cmdlineMode && EditorUtility.DisplayCancelableProgressBar(text, string.Format("检查中 : {0}/{1} -- {2}", i, allAssetPaths.Length, keyValuePair.Key), (float)i / (float)allAssetPaths.Length))
									{
										EditorUtility.ClearProgressBar();
										return;
									}
									value.DoCheck(text);
								}
								else
								{
									if (!this.cmdlineMode && EditorUtility.DisplayCancelableProgressBar(text, string.Format("修理中 : {0}/{1} -- {2}", i, allAssetPaths.Length, keyValuePair.Key), (float)i / (float)allAssetPaths.Length))
									{
										EditorUtility.ClearProgressBar();
										return;
									}
									value.DoFix(text);
								}
							}
						}
					}
					if (this.NeedCheck(Const.SCENE_CHECK_MASK) && Regex.IsMatch(text, "Assets/(Res|ResTemp)/.*?\\.(unity|UNITY)$"))
					{
						Scene scene = EditorSceneManager.OpenScene(text, 0);
						GameObject[] rootGameObjects = scene.GetRootGameObjects();
						foreach (KeyValuePair<int, ICheck> keyValuePair2 in this.checkers)
						{
							if (this.NeedCheck(new int[]
							{
								keyValuePair2.Key
							}))
							{
								ICheck value2 = keyValuePair2.Value;
								if (value2.CheckMatch(text))
								{
									if (this.checkMode == CheckMode.CHECK_ONLY)
									{
										value2.DoCheckHierarchy(text, rootGameObjects);
									}
									else
									{
										value2.DoFixHierarchy(text, rootGameObjects);
										EditorSceneManager.MarkSceneDirty(scene);
									}
								}
							}
						}
						EditorSceneManager.SaveOpenScenes();
					}
					else if (this.NeedCheck(Const.PREFAB_CHECK_MASK) && Regex.IsMatch(text, "Assets/(Res|ResTemp)/.*?\\.(prefab|PREFAB)$"))
					{
						GameObject gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(text);
						if (gameObject)
						{
							foreach (KeyValuePair<int, ICheck> keyValuePair3 in this.checkers)
							{
								if (this.NeedCheck(new int[]
								{
									keyValuePair3.Key
								}))
								{
									ICheck value3 = keyValuePair3.Value;
									if (value3.CheckMatch(text))
									{
										value3.DoCheckHierarchy(text, new GameObject[]
										{
											gameObject
										});
									}
								}
							}
						}
					}
				}
			}
			if (!this.cmdlineMode)
			{
				EditorUtility.ClearProgressBar();
			}
			if (this.checkMode == CheckMode.CHECK_FIX)
			{
				this.PostFix();
				return;
			}
			this.PostCheck();
		}

		private void PreCheck()
		{
			foreach (KeyValuePair<int, ICheck> keyValuePair in this.checkers)
			{
				if (this.NeedCheck(new int[]
				{
					keyValuePair.Key
				}))
				{
					keyValuePair.Value.Start();
				}
			}
		}

		private void PostCheck()
		{
			BaseWindow baseWindow = null;
			if (!this.cmdlineMode)
			{
				baseWindow = (EditorWindow.GetWindow(typeof(BaseWindow), false, "CheckResult") as BaseWindow);
				baseWindow.ClearChecker();
			}
			foreach (KeyValuePair<int, ICheck> keyValuePair in this.checkers)
			{
				if (this.NeedCheck(new int[]
				{
					keyValuePair.Key
				}))
				{
					ICheck value = keyValuePair.Value;
					value.Stop();
					if (this.cmdlineMode)
					{
						value.SaveToFile(string.Format(this.CMDLINE_SAVE_FILE_FMT, Const.RECORD_FILE_NAME[keyValuePair.Key]));
					}
					else
					{
						baseWindow.AddChecker(keyValuePair.Key, value);
					}
				}
			}
		}

		private void PostFix()
		{
			this.PostCheck();
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		private void InitResDependReverse()
		{
			if (this._resDependReverse != null)
			{
				return;
			}
			this._resDependReverse = new Dictionary<string, List<string>>();
			string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
			for (int i = 0; i < allAssetPaths.Length; i++)
			{
				string text = allAssetPaths[i];
				if (!this.cmdlineMode)
				{
					EditorUtility.DisplayProgressBar(text, string.Format("初始化资源引用关系 : {0}/{1}", i, allAssetPaths.Length), (float)i / (float)allAssetPaths.Length);
				}
				string[] dependencies = AssetDatabase.GetDependencies(text);
				if (dependencies.Length != 1 || !(dependencies[0] == text))
				{
					foreach (string key in dependencies)
					{
						List<string> list;
						if (this._resDependReverse.ContainsKey(key))
						{
							list = this._resDependReverse[key];
						}
						else
						{
							list = new List<string>();
							this._resDependReverse[key] = list;
						}
						list.Add(text);
					}
				}
			}
			EditorUtility.ClearProgressBar();
		}

		private static ResCheckMgr _inst;

		private readonly string CMDLINE_SAVE_FILE_FMT = Application.dataPath + "/ToolEditor/ResCheck/CheckInfoCsv/{0}.csv";

		public bool cmdlineMode;

		private HashSet<int> checkFlag = new HashSet<int>();

		private CheckMode checkMode;

		private Dictionary<int, ICheck> checkers = new Dictionary<int, ICheck>();

		private Dictionary<string, List<string>> _resDependReverse;

		private string selectionPattern = string.Empty;
	}
}
