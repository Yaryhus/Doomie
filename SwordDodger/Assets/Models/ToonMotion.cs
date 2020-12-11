using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToonMotion : MonoBehaviour 
{
    private class Snapshot
    {
        public Transform transform;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public Snapshot(Transform transform)
        {
            this.transform = transform;
            this.Update();
        }

        public void Update()
        {
            this.position = this.transform.position;
            this.rotation = this.transform.rotation;
            this.scale = this.transform.localScale;
        }
    }

    private Dictionary<int, Snapshot> snapshots = new Dictionary<int, Snapshot>();
    private float updateTime = 0f;

    [Range(1, 60)] public int fps = 20;

    private void LateUpdate()
    {
        if (Time.time - this.updateTime > 1f/this.fps)
        {
            this.SaveSnapshot(transform);
            this.updateTime = Time.time;
        }

        foreach(KeyValuePair<int, Snapshot> item in this.snapshots)
        {
            if (item.Value.transform != null)
            {
                item.Value.transform.position = item.Value.position;
                item.Value.transform.rotation = item.Value.rotation;
                item.Value.transform.localScale = item.Value.scale;
            }
        }
    }

    private void SaveSnapshot(Transform parent)
    {
        if (parent == null) return;
        int childrenCount = parent.childCount;

        for (int i = 0; i < childrenCount; ++i)
        {
            Transform target = parent.GetChild(i);
            int uid = target.GetInstanceID();

            this.snapshots[uid] = new Snapshot(target);
            this.SaveSnapshot(target);
        }
    }
}