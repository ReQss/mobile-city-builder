using UnityEditor;
using UnityEngine;

public class ConvertMaterialsToURP : EditorWindow
{
    [MenuItem("Tools/Convert All Standard Materials to URP")]
    public static void Convert()
    {
        string[] guids = AssetDatabase.FindAssets("t:Material");
        int count = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);

            if (mat.shader.name == "Standard")
            {
                mat.shader = Shader.Find("Universal Render Pipeline/Lit");
                EditorUtility.SetDirty(mat);
                Debug.Log($"Zmieniono shader na URP: {path}");
                count++;
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"Zmieniono shader dla {count} materiałów.");
    }
}
