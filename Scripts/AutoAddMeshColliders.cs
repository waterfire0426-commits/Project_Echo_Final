using UnityEngine;

#if UNITY_EDITOR
[ExecuteAlways]   // 에디터에서도 동작
#endif
public class AutoAddMeshColliders : MonoBehaviour
{
#if UNITY_EDITOR
    [ContextMenu("Add MeshColliders To Children")]
    void AddMeshColliders()
    {
        MeshFilter[] meshes = GetComponentsInChildren<MeshFilter>(true);

        int count = 0;
        foreach (MeshFilter mf in meshes)
        {
            // 자기 자신(Ducts)에 메쉬 없으면 패스
            if (!mf.sharedMesh) continue;

            MeshCollider col = mf.gameObject.GetComponent<MeshCollider>();
            if (col == null)
            {
                col = mf.gameObject.AddComponent<MeshCollider>();
                col.convex = false;
                count++;
            }
        }

        Debug.Log($"[AutoAddMeshColliders] 추가된 MeshCollider 개수: {count}");
    }
#endif
}
