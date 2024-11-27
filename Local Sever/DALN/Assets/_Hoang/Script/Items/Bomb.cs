using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class Bomb : NetworkBehaviour
{
    public GameObject explosion; // Prefab của explosion (phải gắn NetworkObject)
    public GameObject zone;      // Prefab của zone (phải gắn NetworkObject)
    public int maxExplosionRadius;
    public LayerMask obstacleLayer;
    public LayerMask wallLayer;

    private void Awake()
    {
      
        StartCoroutine(Explore());
    
    }

    private IEnumerator Explore()
    {
        yield return new WaitForSeconds(0.2f);
        ExpandZone(); // Gọi phương thức mở rộng khu vực nổ

        yield return new WaitForSeconds(3.0f); // Thời gian chờ trước khi phát nổ

        Vector2 bombPosition = transform.position;
        int rightRadius = CalculateExplosionRadius(Vector2.right);
        int leftRadius = CalculateExplosionRadius(Vector2.left);
        int upRadius = CalculateExplosionRadius(Vector2.up);
        int downRadius = CalculateExplosionRadius(Vector2.down);

        SoundManger.Instance.PlayVFXSound(0); // Chơi âm thanh nổ

        // Tạo các vụ nổ
        for (int i = 0; i <= rightRadius; i += 3)
            SpawnExplosion(new Vector2(bombPosition.x + i, bombPosition.y));
        for (int i = 3; i <= leftRadius; i += 3)
            SpawnExplosion(new Vector2(bombPosition.x - i, bombPosition.y));
        for (int i = 3; i <= upRadius; i += 3)
            SpawnExplosion(new Vector2(bombPosition.x, bombPosition.y + i));
        for (int i = 3; i <= downRadius; i += 3)
            SpawnExplosion(new Vector2(bombPosition.x, bombPosition.y - i));

        yield return new WaitForSeconds(0.5f); // Thời gian chờ sau khi phát nổ

        RequestDestroyServerRpc(); // Yêu cầu server huỷ bom
    }

    private int CalculateExplosionRadius(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxExplosionRadius, wallLayer | obstacleLayer);

        if (hit.collider != null)
        {
            if ((1 << hit.collider.gameObject.layer & wallLayer) != 0)
            {
                return Mathf.Max(0, Mathf.FloorToInt(hit.distance) - 1);
            }
            else if ((1 << hit.collider.gameObject.layer & obstacleLayer) != 0)
            {
                return Mathf.FloorToInt(hit.distance + 3);
            }
        }

        return maxExplosionRadius;
    }

    private void SpawnExplosion(Vector2 position)
    {
        if (IsServer) // Chỉ server spawn explosion
        {
            GameObject explosionInstance = Instantiate(explosion, position, Quaternion.identity);
            NetworkObject explosionNetworkObject = explosionInstance.GetComponent<NetworkObject>();
            if (explosionNetworkObject != null)
            {
                explosionNetworkObject.Spawn(); // Đồng bộ đối tượng tới tất cả client
            }
        }
    }

    private void ExpandZone()
    {
        Vector2 bombPosition = transform.position;
        int rightRadius = CalculateExplosionRadius(Vector2.right);
        int leftRadius = CalculateExplosionRadius(Vector2.left);
        int upRadius = CalculateExplosionRadius(Vector2.up);
        int downRadius = CalculateExplosionRadius(Vector2.down);

        for (int i = 0; i <= rightRadius; i += 3)
            SpawnZone(new Vector2(bombPosition.x + i, bombPosition.y));
        for (int i = 3; i <= leftRadius; i += 3)
            SpawnZone(new Vector2(bombPosition.x - i, bombPosition.y));
        for (int i = 3; i <= upRadius; i += 3)
            SpawnZone(new Vector2(bombPosition.x, bombPosition.y + i));
        for (int i = 3; i <= downRadius; i += 3)
            SpawnZone(new Vector2(bombPosition.x, bombPosition.y - i));
    }

    private void SpawnZone(Vector2 position)
    {
        if (IsServer) // Chỉ server spawn zone
        {
            GameObject zoneInstance = Instantiate(zone, position, Quaternion.identity);
            NetworkObject zoneNetworkObject = zoneInstance.GetComponent<NetworkObject>();
            if (zoneNetworkObject != null)
            {
                zoneNetworkObject.Spawn(); // Đồng bộ đối tượng tới tất cả client
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestDestroyServerRpc()
    {
        NetworkObject.Despawn(true); // Huỷ đối tượng bom
    }

}
