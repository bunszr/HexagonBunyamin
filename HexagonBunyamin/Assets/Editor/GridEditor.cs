// #if UNITY_EDITOR
// using UnityEngine;
// using UnityEditor;

// [CustomEditor(typeof(GridManager))]
// public class GridEditor : Editor
// {
//     GridManager gridManager;

//     public override void OnInspectorGUI()
//     {
//         if (DrawDefaultInspector())
//         {
//             gridManager.CreateMap();
//         }

//         if (GUILayout.Button("Update"))
//         {
//             gridManager.CreateMap();
//         }

//     }

//     private void OnEnable()
//     {
//         gridManager = target as GridManager;
//     }
// }
// #endif
