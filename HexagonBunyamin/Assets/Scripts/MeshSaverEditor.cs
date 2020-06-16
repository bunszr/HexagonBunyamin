#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Oluşturulan altıgen mesh'imizi kaydetmemize yarıyor. Dışardan kullandığım bir script.
public class MeshSaverEditor : MonoBehaviour
{
    public int nextPizzaNum;
    
    [Tooltip("meshToSave listesi boşken çalışır")]
    public bool thisMeshIsSave = true;

    public List<MeshFilter> meshToSave = new List<MeshFilter> ();

    private void Start() {
         if (thisMeshIsSave) {
            meshToSave.Add(GetComponent<MeshFilter>());
        }
    }

    [ContextMenu("Save Mesh")]
    void MeshSaver ()
    {
        for (int i = 0; i < meshToSave.Count; i++)
        {
            MenuCommand command = new MenuCommand (meshToSave[i]);
            SaveMeshInPlace (command);
        }
    }

    private void Update () {
        if (Input.GetKeyDown (KeyCode.H)) {
            MeshSaver ();
        }
    }

    public void SaveMeshInPlace (MenuCommand menuCommand)
    {
        MeshFilter mf = menuCommand.context as MeshFilter;
        Mesh m = mf.sharedMesh;
        SaveMesh (m , "Pizza" + nextPizzaNum.ToString("D2") , false , false);
    }

    public void SaveMesh (Mesh mesh , string name , bool makeNewInstance , bool optimizeMesh)
    {
        string path =  "Assets/Mesh/" + name + ".asset";
        if (string.IsNullOrEmpty (path)) return;
        Mesh meshToSave = (makeNewInstance) ? Object.Instantiate (mesh) as Mesh : mesh;

        if (optimizeMesh)
            MeshUtility.Optimize (meshToSave);

        AssetDatabase.CreateAsset (meshToSave , path);
        AssetDatabase.SaveAssets ();
    }
}
#endif