using UnityEditor;


//public enum TargetAreaType { circle, line }

//[CustomEditor(typeof(Ability))]
//public class AbilityEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        DrawDefaultInspector();

//        Ability ability = (Ability)target;
//        ability.targetAreaType = (Ability.TargetAreaType)EditorGUILayout.EnumPopup("Target Area Type", ability.targetAreaType);

//        switch (ability.targetAreaType)
//        {
//            case Ability.TargetAreaType.circle:
//                CircleTargetArea circleTargetArea = ability.targetArea as CircleTargetArea;
//                if (circleTargetArea == null)
//                {
//                    circleTargetArea = new CircleTargetArea();
//                    Debug.Log("Create new circle target area");
//                }
//                //Create inspector fields for the properties unique to circle target area.
//                circleTargetArea.radius = EditorGUILayout.FloatField("Radius", circleTargetArea.radius);
//                //Stores changes to the ability property. Said property can then be accessed for all the information required to instantiate a target area.
//                ability.targetArea = circleTargetArea;
//                break;
//            case Ability.TargetAreaType.line:
//                LineTargetArea lineTargetArea = ability.targetArea as LineTargetArea;
//                if (lineTargetArea == null)
//                {
//                    lineTargetArea = new LineTargetArea();
//                }
//                lineTargetArea.width = EditorGUILayout.FloatField("Width", lineTargetArea.width);
//                lineTargetArea.length = EditorGUILayout.FloatField("Length", lineTargetArea.length);

//                ability.targetArea = lineTargetArea;
//                break;
//        }

//        //Allows the prefab to be set. This property is in the parent class "TargetArea", so all children share it. 
//        ability.targetArea.targetAreaPrefab = EditorGUILayout.ObjectField("Target Area Prefab", ability.targetArea.targetAreaPrefab, typeof(Transform), true) as Transform;
//    }
//}
//public class Ability
//{
//    public TargetAreaType targetAreaType;

//}