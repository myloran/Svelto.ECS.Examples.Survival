using UnityEngine;

public interface IRayCaster {
    bool CheckHit(Ray ray, float range, int layer, int mask, out Vector3 point, out int instanceId);
    bool CheckHit(Ray ray, float range, int mask, out Vector3 point);
}

public class RayCaster : IRayCaster {
    public bool CheckHit(Ray ray, float range, int layer, int mask, out Vector3 point, out int instanceId) {
        if (Physics.Raycast(ray, out var shootHit, range, mask) && shootHit.collider != null) {
            var obj = shootHit.collider.gameObject;
            point = shootHit.point;
            instanceId = obj.layer == layer ? obj.GetInstanceID() : -1;
            return true;
        }

        point = new Vector3();
        instanceId = -1;
        return false;
    }

    public bool CheckHit(Ray ray, float range, int mask, out Vector3 point) {
        if (Physics.Raycast(ray, out var shootHit, range, mask) && shootHit.collider != null) {
            point = shootHit.point;
            return true;
        }

        point = new Vector3();
        return false;
    }
}