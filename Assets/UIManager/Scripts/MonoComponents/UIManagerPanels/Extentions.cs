using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;

public static partial class Functions
{
    public static string GetFullName(this GameObject go)
    {
        string name = go.name;
        while (go.transform.parent != null)
        {

            go = go.transform.parent.gameObject;
            name = go.name + "/" + name;
        }
        return name;
    }
    public static void RemoveNull<T>(List<T> list) where T : class
    {
        for (int i = 0; i < list.Count; i++)
            if (list[i] == null)
            {
                list.RemoveAt(i);
                i--;
            }
    }
    public static GameObject FindObject(this GameObject parent, string name)
    {
        Transform[] trs = parent.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in trs)
        {
            if (t.name == name)
            {
                return t.gameObject;
            }
        }
        return null;
    }
    public static void SetActiveAllComponents(this GameObject content, bool active)
    {
        List<Behaviour> components = new List<Behaviour>(content.GetComponentsInChildren<Behaviour>(true));
        components.AddRange(content.GetComponents<Behaviour>());
        for (int i = 0; i < components.Count; i++)
            components[i].enabled = active;
    }
    public static void ChangeLayerForAll(this IEnumerable<SpriteRenderer> renders, int increment)
    {
        foreach (SpriteRenderer renderer in renders)
            renderer.sortingOrder += increment;
    }
    public static List<T> GetAllComponents<T>(this GameObject obj)
    {
        List<T> result = new List<T>();
        result.AddRange(obj.GetComponents<T>());

        for (int i = 0; i < obj.transform.childCount; i++)
            result.AddRange(obj.transform.GetChild(i).gameObject.GetAllComponents<T>());

        return result;
    }
    public static T FindNearest<T>(this Transform obj, List<T> objects, params MonoBehaviour[] exclude) where T : MonoBehaviour
    {
        List<Transform> listExclude = new List<Transform>(exclude.Length);
        List<Transform> _objects = new List<Transform>(objects.Count);
        foreach (MonoBehaviour mb in exclude)
            listExclude.Add(mb.transform);
        listExclude.Add(obj.transform);
        foreach (MonoBehaviour mb in objects)
            _objects.Add(mb.transform);
        return FindNearest(obj.transform, _objects, listExclude.ToArray()).GetComponent<T>();
    }
    public static Transform FindNearest(this Transform obj, List<Transform> objects, params Transform[] exclude)
    {
        List<Transform> listExclude = new List<Transform>(exclude);
        listExclude.Add(obj);
        return FindNearest(obj.position, objects, exclude);
    }
    public static Transform FindNearest(this Vector2 pointFrom, List<Transform> objects, params Transform[] exclude)
    {
        return FindNearest((Vector3)pointFrom, objects, exclude);
    }
    public static Transform FindNearest(this Vector3 pointFrom, List<Transform> objects, params Transform[] exclude)
    {
        List<Transform> listExclude = new List<Transform>(exclude);
        Transform nearestObject = null;
        for (int i = 0; i < objects.Count; i++)
        {
            if (listExclude.Contains(objects[i]))
                continue;
            if (nearestObject == null)
                nearestObject = objects[i];
            else if
                ((Vector3.Distance(pointFrom, objects[i].position) <
                Vector3.Distance(pointFrom, nearestObject.position)))
                nearestObject = objects[i];
        }
        return nearestObject;
    }
    public static Transform FindNearest(this Transform obj, List<Transform> objects, float maxSearchDistance, params Transform[] exclude)
    {
        Transform nearestObject = obj.FindNearest(objects, exclude);

        if (nearestObject != null &&
            Vector2.Distance(nearestObject.position, obj.position) > maxSearchDistance)
            return null;
        return nearestObject;
    }
    public static bool IsObjectVisible(this Camera camera, Renderer renderer)
    {
        return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(camera), renderer.bounds);
    }
    public static List<T> GetAllPrefabsWithComponent<T>(string path) where T : MonoBehaviour
    {
        List<GameObject> temp = GetAllPrefabs(path);
        List<T> result = new List<T>();
#if UNITY_EDITOR
        for (int i = 0; i < temp.Count; i++)
            if (temp[i].GetComponent<T>() != null)
                result.Add(temp[i].GetComponent<T>());
#endif
        return result;
    }
    public static List<GameObject> GetAllPrefabs(string path)
    {
        List<GameObject> result = new List<GameObject>();
#if UNITY_EDITOR
        if (!path.Contains(Application.dataPath))
            path = path.Insert(0, Application.dataPath + "/");
        if (path[path.Length - 1] != '/')
            path += '/';
        string[] aFilePaths = Directory.GetFiles(path);

        string[] directoriesInPath = Directory.GetDirectories(path);

        for (int i = 0; i < directoriesInPath.Length; i++)
        {
            Debug.Log("Search in: " + directoriesInPath[i]);
            result.AddRange(GetAllPrefabs(directoriesInPath[i]));
        }
        foreach (string sFilePath in aFilePaths)
        {
            if (!sFilePath.Contains(".prefab") || sFilePath.Contains(".meta"))
                continue;
            string sAssetPath = sFilePath.Substring(Application.dataPath.Length - 6);

            GameObject objAsset = UnityEditor.AssetDatabase.LoadAssetAtPath(sAssetPath, typeof(GameObject)) as GameObject;

            if (objAsset != null)
                result.Add(objAsset);
        }
#endif
        return result;
    }
    public static void AddState(this Dictionary<int, StateDefault> statesMap,
        int stateId, Action OnStart = default, Action<int> OnEnd = default)
    {
        StateDefault state = new StateDefault();
        state.OnStart = OnStart;
        state.OnEnd = OnEnd;
        statesMap.Add(stateId, state);
    }
    public static void SetLayerRecursively(this GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            child.gameObject.SetLayerRecursively(layer);
        }
    }
    public static DG.Tweening.Tween DOVolume(this AudioSource audioSource, float endValue, float duration)
    {
        return DG.Tweening.DOTween.To(
            () => audioSource.volume,
            (float value) => audioSource.volume = value,
            endValue, duration);
    }
}

