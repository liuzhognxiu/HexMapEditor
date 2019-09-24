using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FY.Tools.ResCheck
{
	public class ResCheckMenu
	{
		[MenuItem("Assets/Menu/ResCheck/CheckDependence", false, 1)]
		public static void CheckDependence()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				15
			});
			inst.StartCheck();
		}

		[MenuItem("Assets/Menu/ResCheck/CheckDependedBy", false, 2)]
		public static void CheckDependedBy()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				16
			});
			inst.StartCheck();
		}

		[MenuItem("Assets/Menu/ResCheck/CheckTextureRef", false, 3)]
		public static void AssetsCheckTextureRef()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				29
			});
			inst.StartCheckSelection();
		}

		[MenuItem("Assets/Menu/ResCheck/CheckBigWorldRes", false, 4)]
		public static void AssetsCheckBigWorldScene()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				35
			});
			inst.StartCheckSelection();
		}

		[MenuItem("Assets/Menu/ResCheck/CheckAll", false, 10)]
		public static void AssetsCheckAll()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(Const.CHECK_ALL);
			inst.StartCheckSelection();
		}

		[MenuItem("ResCheck/依赖检查/文件未使用", false, 101)]
		public static void CheckFileNotUsed()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				1
			});
			inst.StartCheck();
		}

		[MenuItem("ResCheck/依赖检查/材质丢失", false, 102)]
		public static void CheckMaterialLost()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				9
			});
			inst.StartCheck();
		}

		[MenuItem("ResCheck/依赖检查/脚本丢失", false, 102)]
		public static void CheckComponentLost()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				39
			});
			inst.StartCheck();
		}

		[MenuItem("ResCheck/依赖检查/文件拷贝", false, 103)]
		public static void CheckRepeatResMd5()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				6
			});
			inst.StartCheck();
		}

		[MenuItem("ResCheck/依赖检查/内置Shader", false, 104)]
		public static void CheckBuiltInShader()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				10
			});
			inst.StartCheck();
		}

		[MenuItem("ResCheck/依赖检查/Res引用非Res目录", false, 105)]
		public static void CheckDependenciesInRes()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				22
			});
			inst.StartCheck();
		}

		[MenuItem("ResCheck/依赖检查/空目录", false, 106)]
		public static void CheckEmptyDirectory()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				27
			});
			inst.StartCheck();
		}

		[MenuItem("ResCheck/依赖检查/动态加载资源在ResTemp", false, 107)]
		public static void CheckDynamicAssetInResTemp()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				28
			});
			inst.StartCheck();
		}

		[MenuItem("ResCheck/依赖检查/配置内置天空盒", false, 109)]
		public static void CheckSkybox()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				33
			});
			inst.StartCheck();
		}

		[MenuItem("ResCheck/依赖检查/脚本成员Shder空", false, 109)]
		public static void CheckShaderNull()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				31
			});
			inst.StartCheck();
		}

		[MenuItem("ResCheck/依赖检查/Layer检查", false, 110)]
		public static void CheckLayerMatch()
		{
			LayerWindow.Open();
		}

		[MenuItem("ResCheck/依赖检查/碰撞渲染相同mesh", false, 110)]
		public static void CheckMeshReuse()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				42
			});
			inst.StartCheck();
		}

		[MenuItem("ResCheck/文件名检查/资源类型", false, 201)]
		public static void CheckAssetsTypes()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				4
			});
			inst.StartCheck();
		}

		[MenuItem("ResCheck/文件名检查/Unity文件", false, 202)]
		public static void CheckAllUnityFiles()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				25
			});
			inst.StartCheck();
		}

		[MenuItem("ResCheck/文件名检查/文件名带空格", false, 203)]
		public static void CheckFileWithSpace()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				2
			});
			inst.StartCheck();
		}

		[MenuItem("ResCheck/文件名检查/文件名有大写", false, 204)]
		public static void CheckFileWithCaps()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				3
			});
			inst.StartCheck();
		}

		[MenuItem("ResCheck/文件名检查/GUI图片重名", false, 205)]
		public static void CheckRepeatGUIResName()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				5
			});
			inst.StartCheck();
		}

		[MenuItem("ResCheck/文件名检查/Fbx中物件名", false, 206)]
		public static void CheckObjsNameInFbx()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				26
			});
			inst.StartCheck();
		}

		[MenuItem("ResCheck/文件结构检查/根节点名称", false, 301)]
		public static void CheckSceneRootObjName()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				7
			});
			inst.StartCheck();
		}

		[MenuItem("ResCheck/文件结构检查/根节点数目", false, 302)]
		public static void CheckSceneRootObjCount()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				8
			});
			inst.StartCheck();
		}

		[MenuItem("ResCheck/文件结构检查/AnimController检查", false, 303)]
		public static void CheckAnimController()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				11
			});
			inst.StartCheck();
		}

		[MenuItem("ResCheck/文件结构检查/Fbx中物件数", false, 304)]
		public static void CheckObjsCountInFbx()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				12
			});
			inst.StartCheck();
		}

		[MenuItem("ResCheck/文件结构检查/导入材质开关", false, 305)]
		public static void CheckImportMaterialOn()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				17
			});
			inst.StartCheck();
		}

		[MenuItem("ResCheck/文件结构检查/碰撞盒无效检查", false, 306)]
		public static void CheckMeshCollider()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				30
			});
			inst.StartCheck();
		}

		[MenuItem("ResCheck/文件结构检查/资源包含相机", false, 307)]
		public static void CheckContainsCamera()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				36
			});
			inst.StartCheck();
		}

		[MenuItem("ResCheck/文件结构检查/模型面数", false, 308)]
		public static void CheckModelTriangles()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				38
			});
			inst.StartCheck();
		}

		[MenuItem("ResCheck/文件结构检查/碰撞结构", false, 309)]
		public static void CheckColliderHiearchy()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				40
			});
			inst.StartCheck();
		}

		[MenuItem("ResCheck/文件结构检查/碰撞面数", false, 310)]
		public static void CheckColliderTriangles()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				41
			});
			inst.StartCheck();
		}

		[MenuItem("ResCheck/文件结构检查/合批Mesh规范", false, 311)]
		public static void CheckBatchMeshValid()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				44
			});
			inst.StartCheck();
		}

		[MenuItem("ResCheck/贴图检查/贴图大小", false, 401)]
		public static void CheckTextureSize()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				13
			});
			inst.StartCheck();
		}

		[MenuItem("ResCheck/贴图检查/贴图宽高", false, 402)]
		public static void CheckTextureSquare()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				14
			});
			inst.StartCheck();
		}

		[MenuItem("ResCheck/角色检查/缺Animator", false, 501)]
		public static void CheckCharNoAnimator()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				18
			});
			inst.StartCheck();
		}

		[MenuItem("ResCheck/角色检查/检查未使用的AnimClip", false, 502)]
		public static void CheckCharAnimClipNotUsed()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				19
			});
			inst.StartCheck();
		}

		[MenuItem("ResCheck/角色检查/检查Anim类型", false, 503)]
		public static void CheckCharAnimType()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				20
			});
			inst.StartCheck();
		}

		[MenuItem("ResCheck/角色检查/检查Anim错误", false, 504)]
		public static void CheckAvatarDefine()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				21
			});
			inst.StartCheck();
		}

		[MenuItem("ResCheck/角色检查/角色相机挂点", false, 505)]
		public static void CheckCharacterHpLost()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				43
			});
			inst.StartCheck();
		}

		[MenuItem("ResCheck/特效检查/粒子个数检查", false, 601)]
		public static void CheckParticleSystemCount()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				23
			});
			inst.StartCheck();
		}

		[MenuItem("ResCheck/特效检查/粒子Render数检查", false, 602)]
		public static void CheckRenderCount()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				24
			});
			inst.StartCheck();
		}

		[MenuItem("ResCheck/检查所有", false, 9000)]
		public static void CheckAll()
		{
			if (!EditorUtility.DisplayDialog("ResCheck", "检查所有", "确认", "取消"))
			{
				return;
			}
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.EnableCheck(Const.CHECK_ALL);
			inst.StartCheck();
		}

		[MenuItem("ResCheck/高级/面数统计/开始", false, 9101)]
		public static void StartCountTriangles()
		{
			if (!Application.isPlaying)
			{
				EditorUtility.DisplayDialog("面数统计", "此功能需要在游戏运行时使用", "确定");
				return;
			}
            TriangleCountManager mgr = TriangleCountManager.mgr;
			int sceneCount = SceneManager.sceneCount;
			for (int i = 0; i < sceneCount; i++)
			{
				foreach (GameObject root in SceneManager.GetSceneAt(i).GetRootGameObjects())
				{
					mgr.SetupTriangleCounter(root);
				}
			}
			EditorApplication.hierarchyWindowItemOnGUI = (EditorApplication.HierarchyWindowItemCallback)Delegate.Combine(EditorApplication.hierarchyWindowItemOnGUI, new EditorApplication.HierarchyWindowItemCallback(TriangleCountManager.HierarchyWindowItemOnGUI));
		}

		[MenuItem("ResCheck/高级/面数统计/结束", false, 9102)]
		public static void StopCountTriangles()
		{
			TriangleCountManager mgr = TriangleCountManager.mgr;
			int sceneCount = SceneManager.sceneCount;
			if (mgr == null)
			{
				return;
			}
			for (int i = 0; i < sceneCount; i++)
			{
				foreach (GameObject root in SceneManager.GetSceneAt(i).GetRootGameObjects())
				{
					mgr.RemoveTriangleCounter(root);
				}
			}
            UnityEngine.Object.DestroyImmediate(mgr);
			EditorApplication.hierarchyWindowItemOnGUI = (EditorApplication.HierarchyWindowItemCallback)Delegate.Remove(EditorApplication.hierarchyWindowItemOnGUI, new EditorApplication.HierarchyWindowItemCallback(TriangleCountManager.HierarchyWindowItemOnGUI));
		}

		[MenuItem("ResCheck/高级/Fix/FileNotUsed", false, 9201)]
		public static void FixFileNotUsed()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				1
			});
			inst.StartFix();
		}

		[MenuItem("ResCheck/高级/Fix/SpaceInFileName", false, 9202)]
		public static void FixFileWithSpace()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				2
			});
			inst.StartFix();
		}

		[MenuItem("ResCheck/高级/Fix/CapsInFileName", false, 9203)]
		public static void FixFileWithCaps()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				3
			});
			inst.StartFix();
		}

		[MenuItem("ResCheck/高级/Fix/BuiltInShader", false, 9204)]
		public static void FixBuiltInShader()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				10
			});
			inst.StartFix();
		}

		[MenuItem("ResCheck/高级/Fix/DependenceNotInRes", false, 9205)]
		public static void FixDependenceNotInRes()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				22
			});
			inst.StartFix();
		}

		[MenuItem("ResCheck/高级/Fix/CloseImportMaterials", false, 9206)]
		public static void FixImportMaterialsON()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				17
			});
			inst.StartFix();
		}

		[MenuItem("ResCheck/高级/Fix/EmptyDirectory", false, 9207)]
		public static void FixEmptyDirectory()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				27
			});
			inst.StartFix();
		}

		[MenuItem("ResCheck/高级/Fix/DynamicAssetInResTemp", false, 9208)]
		public static void FixDynamicAssetInResTemp()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				28
			});
			inst.StartFix();
		}

		[MenuItem("ResCheck/高级/Fix/MeshColliderError", false, 9209)]
		public static void FixMeshColliderError()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.EnableCheck(new int[]
			{
				30
			});
			inst.StartFix();
		}

		[MenuItem("ResCheck/高级/Fix/All", false, 9299)]
		public static void FixAll()
		{
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.StartFix();
		}

		public static void CommandLineCheckAll()
		{
			string path = Application.dataPath + "/ToolEditor/ResCheck/CheckInfoCsv/";
			//Utils.DeletePath(path);
			Directory.CreateDirectory(path);
			ResCheckMgr inst = ResCheckMgr.inst;
			inst.ResetCheckFlag();
			inst.cmdlineMode = true;
			inst.EnableCheck(Const.CHECK_ALL);
			inst.DisableCheck(16);
			inst.DisableCheck(15);
			inst.StartCheck();
		}

		[MenuItem("ResCheck/帮助", false, 99999)]
		public static void ResCheckHelp()
		{
			Application.OpenURL("http://note.youdao.com/groupshare/?token=464B2537522B488486C9BC38B8B2209A&gid=29382815");
		}
	}
}
