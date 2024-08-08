using UnityEditor;
#if UNITY_2019_3_OR_NEWER
using UnityEditor.Compilation;
#elif UNITY_2017_1_OR_NEWER
using System.Reflection;
#endif
using UnityEngine;

public class CompilationWindow : EditorWindow
{
    [MenuItem("Window/Compilation")]
    private static void ShowWindow()
    {
        var window = GetWindow<CompilationWindow>();
        window.titleContent = new GUIContent("Compilation");
        window.Show();
    }

    private void OnGUI()
    {
        // Add space at the top
        GUILayout.Space(10);

        // Button to request script compilation
        if (GUILayout.Button("Request Script Compilation"))
        {
#if UNITY_2019_3_OR_NEWER
            // Request script compilation for Unity 2019.3 or newer
            CompilationPipeline.RequestScriptCompilation();
#elif UNITY_2017_1_OR_NEWER
            // Request script compilation for Unity 2017.1 or newer
            try
            {
                var editorAssembly = Assembly.GetAssembly(typeof(Editor));
                var editorCompilationInterfaceType = editorAssembly.GetType("UnityEditor.Scripting.ScriptCompilation.EditorCompilationInterface");
                var dirtyAllScriptsMethod = editorCompilationInterfaceType.GetMethod("DirtyAllScripts", BindingFlags.Static | BindingFlags.Public);
                dirtyAllScriptsMethod.Invoke(editorCompilationInterfaceType, null);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error requesting script compilation: {ex.Message}");
            }
#endif
        }
    }
}
