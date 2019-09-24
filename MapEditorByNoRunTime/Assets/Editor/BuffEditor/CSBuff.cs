using System;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
public class CSBuff : SerializedScriptableObject
{
    
    [BoxGroup("Box")]
    [EnumLabel("Buff类型")]
    public Const.PassEventBuff MyTypeEnum = Const.PassEventBuff.OnStart;

    [BoxGroup("Box")]
    [EnumLabel("Buff触发事件")]
    public Const.BuffEventType BuffEventType = Const.BuffEventType.OnCountDuration;

    [BoxGroup("Box")]
    [EnumLabel("Buff触发事件")]
    public Const.BuffType BuffType = Const.BuffType.SetState;

    [BoxGroup("Box")]
    [HideIf("@this.BuffType != Const.BuffType.SetValue")]
    [EnumLabel("数值型buff")]
    public Const.BufferSetValue BuffSetValue = Const.BufferSetValue.Atk;

    [FoldoutGroup("Box/技能详细配置")]
    public string ID;

    [FoldoutGroup("Box/技能详细配置")]
    [Header("Buff描述")]
    public string Desc;

    [CustomValueDrawer("MyStaticCustomDrawerStatic")]
    [HideIf("@this.BuffEventType != Const.BuffEventType.OnRound")]
    [FoldoutGroup("Box/技能详细配置")]
    [OnValueChanged("ValueChanged")]
    public int RandValue;
    
    private static int MyStaticCustomDrawerStatic(int value, GUIContent label)
    {
        return (int)EditorGUILayout.Slider(label, value, 0f, 100f);
    }

    [FoldoutGroup("Box/技能详细配置")]
    [HideIf("@this.BuffEventType != Const.BuffEventType.OnRound")]
    [ProgressBar(0,100)]
    [ReadOnly]
    public int ValueBar = 0;

    [FoldoutGroup("Box/特效配置")]
    public GameObject Effect;

    [FoldoutGroup("Box/特效配置")]
    public Transform EffectTransform;

    [FoldoutGroup("Box/动画配置")]
    [OnValueChanged("IsDelete")]
    public Transform PlayerAnimatorTransform;

    [FoldoutGroup("Box/动画配置")]
    [OnInspectorGUI("DrawPreview", append: true)]
    public Animator PlayerAnimator;

    public Texture PlayTexture;

    void ValueChanged()
    {
        ValueBar = RandValue;
    }

    void IsDelete()
    {
        if (PlayerAnimatorTransform == null)
        {
            this.PlayerAnimator = null;
        }
    }

    private void DrawPreview()
    {
        Transform owner = this.PlayerAnimatorTransform;

        if (owner && owner.GetComponent<Animator>())
        {
            this.PlayerAnimator = owner.GetComponent<Animator>();
        }
        if (this.PlayerAnimator == null)
        {
            EditorGUILayout.LabelField(new GUIContent("Choose Animator!!!!", "check sequence owner"), "ShurikenModuleTitle", new GUILayoutOption[0]);
            this.animGOEditor = null;
            return;
        }
        this.DisplayAvatar();
    }

    private void DisplayAvatar()
    {
        if (PlayTexture!=null)
        {
              this.texturEditor = Editor.CreateEditor(PlayTexture);
              this.texturEditor.OnPreviewGUI(GUILayoutUtility.GetRect(16f, 100f), EditorStyles.helpBox);
        }
        if (this.PlayerAnimator != null && this.animGOEditor == null)
        {
            this.animGOEditor = Editor.CreateEditor(this.PlayerAnimator.gameObject);
        }
        if (this.animGOEditor != null)
        {
            EditorGUILayout.LabelField(new GUIContent("Animator:"), new GUILayoutOption[0]);
            this.animGOEditor.OnPreviewGUI(GUILayoutUtility.GetRect(16f, 100f), EditorStyles.helpBox);
        }
    }

    private Editor animGOEditor;
    private Editor texturEditor;
}
