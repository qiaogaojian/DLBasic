using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameTweenBase), true)]
class UITweenEditor : Editor
{
    SerializedObject serObj;
    SerializedProperty time;
    SerializedProperty delay;
    SerializedProperty playOnAwake;

    SerializedProperty loop;
    SerializedProperty loopCount;
    SerializedProperty loopType;

    SerializedProperty space;
    SerializedProperty itsLocation;

    SerializedProperty startValue;
    SerializedProperty endValue;

    SerializedProperty curveType;
    SerializedProperty tweenCurve;
    SerializedProperty animCurve;
    SerializedProperty onStart;
    SerializedProperty onUpdate;
    SerializedProperty OnComplete;

    void OnEnable()
    {
        serObj = new SerializedObject(target);

        time = serObj.FindProperty("time");
        delay = serObj.FindProperty("delay");
        playOnAwake = serObj.FindProperty("playOnAwake");
        loop = serObj.FindProperty("loop");

        loopCount = serObj.FindProperty("loopCount");
        loopType = serObj.FindProperty("loopType");

        space = serObj.FindProperty("space");
        itsLocation = serObj.FindProperty("itsLocation");

        startValue = serObj.FindProperty("startValue");
        endValue = serObj.FindProperty("endValue");

        curveType = serObj.FindProperty("curveType");

        tweenCurve = serObj.FindProperty("tweenCurve");
        animCurve = serObj.FindProperty("animCurve");
        onStart = serObj.FindProperty("onStart");
        onUpdate = serObj.FindProperty("onUpdate");
        OnComplete = serObj.FindProperty("onComplete");
    }
    //float knob = 0;
    public override void OnInspectorGUI()
    {
        serObj.Update();

        //knob = EditorGUILayout.Knob(new Vector2(100, 100), knob, 1, 10, "斤", Color.black, Color.blue, true);

        EditorGUILayout.PropertyField(time, new GUIContent("Time"));
        EditorGUILayout.PropertyField(delay, new GUIContent("Delay"));
        EditorGUILayout.PropertyField(playOnAwake, new GUIContent("PlayOnAwake"));
        EditorGUILayout.PropertyField(loop, new GUIContent("Loop"));
        if (loop.boolValue)
        {
            GUILayout.Label("[ loopCount=0 Is EndlessLoop ]", EditorStyles.miniBoldLabel);
            loopCount.intValue = EditorGUILayout.IntSlider("LoopCount", loopCount.intValue, 0, 100);
            loopType.intValue = (int)((DG.Tweening.LoopType)EditorGUILayout.EnumPopup("LoopType", (DG.Tweening.LoopType)loopType.intValue));
        }

        EditorGUILayout.Separator(); EditorGUILayout.PropertyField(itsLocation, new GUIContent("ItsLocation"));
        if (!itsLocation.boolValue)
        {
            EditorGUILayout.Separator(); startValue.vector3Value = EditorGUILayout.Vector3Field("StartValue", startValue.vector3Value);
        }

        EditorGUILayout.Separator(); endValue.vector3Value = EditorGUILayout.Vector3Field("EndValue", endValue.vector3Value);

        EditorGUILayout.Separator(); space.intValue = (int)((GameTweenBase.Space)EditorGUILayout.EnumPopup("SpaceType", (GameTweenBase.Space)space.intValue));

        EditorGUILayout.Separator(); curveType.intValue = (int)((GameTweenBase.CurveType)EditorGUILayout.EnumPopup("CurveType", (GameTweenBase.CurveType)curveType.intValue));
        if (curveType.intValue == 1)
        {
            EditorGUILayout.Separator(); tweenCurve.intValue = (int)((DG.Tweening.Ease)EditorGUILayout.EnumPopup("TweenCurve", (DG.Tweening.Ease)tweenCurve.intValue));

        }
        else if (curveType.intValue == 2)
        {
            EditorGUILayout.Separator(); EditorGUILayout.PropertyField(animCurve, new GUIContent("AnimCurve"));
        }

        EditorGUILayout.Separator(); EditorGUILayout.PropertyField(onStart, new GUIContent("OnStart"));
        EditorGUILayout.Separator(); EditorGUILayout.PropertyField(onUpdate, new GUIContent("OnUpdate"));
        EditorGUILayout.Separator(); EditorGUILayout.PropertyField(OnComplete, new GUIContent("OnComplete"));

        serObj.ApplyModifiedProperties();
    }
}
