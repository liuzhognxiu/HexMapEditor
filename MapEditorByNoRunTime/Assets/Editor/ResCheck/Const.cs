using System;
using System.Collections.Generic;

namespace FY.Tools.ResCheck
{
	public class Const
	{
		public const int CHECK_FILE_NOT_USED = 1;

		public const int CHECK_FILE_NAME_SPACE = 2;

		public const int CHECK_FILE_NAME_CAPS = 3;

		public const int CHECK_ASSET_TYPES = 4;

		public const int CHECK_FILE_GUI_REPEAT_NAME = 5;

		public const int CHECK_FILE_REPEAT_MD5 = 6;

		public const int CHECK_SCENE_ROOT_OBJ_NAME = 7;

		public const int CHECK_SCENE_ROOT_OBJ_COUNT = 8;

		public const int CHECK_MATERIAL_LOST = 9;

		public const int CHECK_BUILTIN_SHADER = 10;

		public const int CHECK_ANIM_CTROLLER = 11;

		public const int CHECK_FBX_OBJ_COUNT = 12;

		public const int CHECK_TEXTURE_SIZE = 13;

		public const int CHECK_TEXTURE_SQUARE = 14;

		public const int CHECK_DEPENDENCIES = 15;

		public const int CHECK_DEPENDEDBY = 16;

		public const int CHECK_FBX_IMPORT_MAT_ON = 17;

		public const int CHECK_CHAR_NO_ANIMATOR = 18;

		public const int CHECK_CHAR_ANIMCLIP_NOTUSED = 19;

		public const int CHECK_CHAR_ANIMATION_TYPE = 20;

		public const int CHECK_CHAR_FBX_ANIMATION = 21;

		public const int CHECK_RES_DEPENDENCIES = 22;

		public const int CHECK_PARTICLE_SYSTEM_COUNT = 23;

		public const int CHECK_EFFECT_RENDER_COUNT = 24;

		public const int CHECK_ALL_UNITY_FILES = 25;

		public const int CHECK_FBX_OBJ_NAME = 26;

		public const int CHECK_EMPTY_DIR = 27;

		public const int CHECK_DYNAMIC_ASSET_NOT_IN_RES = 28;

		public const int CHECK_TEXTURE_REF = 29;

		public const int CHECK_MESHCOLLIDER_VALID = 30;

		public const int CHECK_FXSHADER_NULL = 31;

		public const int CHECK_SKYBOX = 33;

		public const int CHECK_BIGWORLD_SCENE = 35;

		public const int CHECK_CONTAINS_CAMERA = 36;

		public const int CHECK_LAYER_MATCH = 37;

		public const int CHECK_MODEL_TRIANGLES = 38;

		public const int CHECK_COMPONENT_LOST = 39;

		public const int CHECK_COLLIDER_HIEARCHY = 40;

		public const int CHECK_COLLIDER_TRIANGLES = 41;

		public const int CHECK_MESH_REUSE = 42;

		public const int CHECK_CHARACTER_HPLOST = 43;

		public const int CHECK_BATCHMESH_VALID = 44;

		public static readonly int[] SCENE_CHECK_MASK = new int[]
		{
			7,
			8,
			9,
			29,
			30,
			33,
			35,
			36,
			37,
			39,
			40,
			41,
			42,
			44
		};

		public static readonly int[] PREFAB_CHECK_MASK = new int[]
		{
			7,
			9,
			23,
			31,
			36,
			37,
			38,
			39,
			40,
			41,
			42,
			43,
			44
		};

		public static readonly int[] CHECK_ALL = new int[]
		{
			1,
			2,
			3,
			6,
			7,
			8,
			9,
			10,
			11,
			12,
			26,
			13,
			14,
			17,
			18,
			19,
			20,
			21,
			22,
			23,
			24,
			27,
			28,
			30,
			31,
			33,
			35,
			36,
			38,
			39,
			40,
			41,
			42,
			43,
			44
		};

		public static readonly Dictionary<int, Type> CHECKER_MAP = new Dictionary<int, Type>
		{
            {
				4,
				typeof(CheckFileTypes)
			},
			{
				2,
				typeof(CheckFileNameSpace)
			},
			{
				3,
				typeof(CheckFileNameCaps)
			},
			{
				5,
				typeof(CheckRepeatGuiResName)
			},
            {
				7,
				typeof(CheckSceneObjName)
			},
			{
				8,
				typeof(CheckSceneObjCount)
			},
			{
				9,
				typeof(CheckMaterialLost)
			},
			{
				10,
				typeof(CheckFileBuiltInShader)
			},
			{
				11,
				typeof(CheckFileAnimController)
			},
			{
				12,
				typeof(CheckFileFbxObjectsCount)
			},
			{
				17,
				typeof(CheckFileFbxImportMatON)
			},
			{
				13,
				typeof(CheckFileTextureSize)
			},
			{
				14,
				typeof(CheckFileTextureSquare)
			},
			{
				15,
				typeof(CheckFileDependencies)
			},
			{
				16,
				typeof(CheckFileDependedBy)
			},
			{
				18,
				typeof(CheckCharAnimPrefabError)
			},
			{
				19,
				typeof(ChechCharAnimclipNotUsed)
			},
			{
				20,
				typeof(CheckCharAnimTypeError)
			},
			{
				21,
				typeof(CheckCharAnimFbxError)
			},
			{
				22,
				typeof(CheckDependenciesInRes)
			},
			{
				23,
				typeof(CheckEffectParticleCount)
			},
			{
				24,
				typeof(CheckEffectRenderCount)
			},
			{
				25,
				typeof(CheckFileAllUnityFiles)
			},
			{
				26,
				typeof(CheckFileFbxObjectsName)
			},
			{
				27,
				typeof(CheckEmptyDir)
			},
            {
				29,
				typeof(CheckTextureRef)
			},
            {
				33,
				typeof(CheckSkybox)
			},
            {
				36,
				typeof(CheckCameraContains)
			},
			{
				37,
				typeof(CheckLayerMatch)
			},
            {
				39,
				typeof(CheckComponentLost)
			},
			{
				40,
				typeof(CheckColliderHierarchy)
			},
			{
				41,
				typeof(CheckColliderTriangles)
			},
			{
				42,
				typeof(CheckMeshReuse)
			},
			{
				43,
				typeof(CheckCharacterHangpoint)
			},
			{
				44,
				typeof(CheckBatchmeshValid)
			}
		};

		public static readonly Dictionary<int, string> RECORD_FILE_NAME = new Dictionary<int, string>
		{
			{
				6,
				"Dependence"
			},
			{
				4,
				"Dependence"
			},
			{
				1,
				"Dependence"
			},
			{
				9,
				"Dependence"
			},
			{
				10,
				"Dependence"
			},
			{
				15,
				"Dependence"
			},
			{
				16,
				"Dependence"
			},
			{
				22,
				"Dependence"
			},
			{
				28,
				"Dependence"
			},
			{
				27,
				"Dependence"
			},
			{
				29,
				"Dependence"
			},
			{
				31,
				"Dependence"
			},
			{
				33,
				"Dependence"
			},
			{
				37,
				"Dependence"
			},
			{
				39,
				"Dependence"
			},
			{
				42,
				"Dependence"
			},
			{
				2,
				"FileNames"
			},
			{
				25,
				"FileNames"
			},
			{
				26,
				"FileNames"
			},
			{
				3,
				"FileNames"
			},
			{
				5,
				"FileNames"
			},
			{
				7,
				"FileStructure"
			},
			{
				8,
				"FileStructure"
			},
			{
				30,
				"FileStructure"
			},
			{
				11,
				"FileStructure"
			},
			{
				12,
				"FileStructure"
			},
			{
				17,
				"FileStructure"
			},
			{
				35,
				"FileStructure"
			},
			{
				36,
				"FileStructure"
			},
			{
				38,
				"FileStructure"
			},
			{
				40,
				"FileStructure"
			},
			{
				41,
				"FileStructure"
			},
			{
				44,
				"FileStructure"
			},
			{
				13,
				"Texture"
			},
			{
				14,
				"Texture"
			},
			{
				18,
				"Character"
			},
			{
				19,
				"Character"
			},
			{
				20,
				"Character"
			},
			{
				21,
				"Character"
			},
			{
				43,
				"Character"
			},
			{
				23,
				"Effect"
			},
			{
				24,
				"Effect"
			}
		};
	}
}
