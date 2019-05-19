using System.Linq;
//using System.Reflection;
//using System.Reflection.Emit;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using VIS.ObjectDescription.ScriptableObjects;
using Object = UnityEngine.Object;

namespace VIS.ObjectDescription.Editor
{
    public static class StaticStickyNotes
    {
        private const string _addMenuPath = "Assets/VIS/Add Sticky Note";
        private const string _removeMenuPath = "Assets/VIS/Remove Sticky Note";

        //static StaticStickyNotes()
        //{
        //    Debug.Log($"StaticStickyNotes");
        //    emit();
        //}

        //[MenuItem("Tools/Emit")]
        //private static void emit()
        //{
        //    Debug.Log($"emit");
        //    var assembly = typeof(UnityEditor.Editor).Assembly;
        //    var allTypes = assembly.GetTypes();
        //    for (int i = 0; i < allTypes.Length; i++)
        //    {
        //        switch (allTypes[i].FullName)
        //        {
        //            case "UnityEditor.RenderTextureEditor":
        //                var tb = GetTypeBuilder();
        //                var type = allTypes[i];
        //                Debug.Log($"type.IsPublic = {type.IsPublic}  type.IsVisible = {type.IsVisible}");
        //                break;
        //        }
        //    }
        //}

        //private static TypeBuilder GetTypeBuilder()
        //{
        //    var typeSignature = "MyDynamicType";
        //    var an = new AssemblyName(typeSignature);
        //    AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
        //    ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
        //    TypeBuilder tb = moduleBuilder.DefineType(typeSignature,
        //            TypeAttributes.Public |
        //            TypeAttributes.Class |
        //            TypeAttributes.AutoClass |
        //            TypeAttributes.AnsiClass |
        //            TypeAttributes.BeforeFieldInit |
        //            TypeAttributes.AutoLayout,
        //            null);
        //    return tb;
        //}

        [MenuItem(_addMenuPath, false)]
        public static void AddStickyNoteToAsset()
        {
            var targetAsset = getAddableAsset();
            var newStickyAsset = ScriptableObject.CreateInstance<StickyNote>();
            newStickyAsset.name = "Note";
            //newStickyAsset.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector | HideFlags.DontUnloadUnusedAsset;
            AssetDatabase.AddObjectToAsset(newStickyAsset, targetAsset);
            EditorUtility.SetDirty(targetAsset);
            //AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(newStickyAsset));
            AssetDatabase.SaveAssets();

            notifyOnStickNotes();
        }

        [MenuItem(_addMenuPath, true)]
        public static bool AddableAssetValidation()
        {
            return getAddableAsset() != null;
        }

        [MenuItem(_removeMenuPath, false)]
        public static void RemoveStickyNoteFromAsset()
        {
            var targetAsset = getRemovableAsset();
            var mainAsset = (Object)null;
            if (AssetDatabase.IsSubAsset(targetAsset))
                mainAsset = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(targetAsset));
            AssetDatabase.RemoveObjectFromAsset(targetAsset);
            Debug.Log($"mainAsset = {mainAsset.name} of type {mainAsset.GetType().FullName}");
            EditorUtility.SetDirty(mainAsset);
            AssetDatabase.SaveAssets();
            Object.DestroyImmediate(targetAsset);

            notifyOnUnstickNotes();
        }

        [MenuItem(_removeMenuPath, true)]
        public static bool RemovableAssetValidation()
        {
            return getRemovableAsset() != null;
        }

        private static Object getAddableAsset()
        {
            var selected = Selection.objects;
            if (selected == null || selected.Length > 1)
                return null;

            var candidate = selected[0];
#if StickyDebug
            Debug.Log($"candidate = {candidate.GetType().FullName}");
#endif
            
            if (candidate is DefaultAsset || candidate is StickyNote || candidate is MonoScript || candidate is SceneAsset ||
                candidate is Shader || candidate is AssemblyDefinitionAsset)
                return null;

            if (!AssetDatabase.IsMainAsset(candidate) && !AssetDatabase.IsSubAsset(candidate))
                return null;

            return candidate;
        }

        private static Object getRemovableAsset()
        {
            var selected = Selection.objects;

            if (selected == null || selected.Length > 1)
                return null;

            if (!(selected[0] is StickyNote))
                return null;

            return selected[0];
        }

        private static void notifyOnStickNotes()
        {
            var allEditors = Resources.FindObjectsOfTypeAll<UnityEditor.Editor>();
            var listeners = allEditors.Where(o => o is IAssetsStickedEventsListener).Select(o => o as IAssetsStickedEventsListener).ToArray();
            for (int i = 0; i < listeners.Length; i++)
                listeners[i].OnSticked();
        }

        private static void notifyOnUnstickNotes()
        {
            var allEditors = Resources.FindObjectsOfTypeAll<UnityEditor.Editor>();
            var listeners = allEditors.Where(o => o is IAssetsStickedEventsListener).Select(o => o as IAssetsStickedEventsListener).ToArray();
            for (int i = 0; i < listeners.Length; i++)
                listeners[i].OnUnsticked();
        }
    }
}
