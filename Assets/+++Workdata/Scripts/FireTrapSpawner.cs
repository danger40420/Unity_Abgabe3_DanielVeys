using System.Collections;
using UnityEngine;

public class FireTrapSpawner : MonoBehaviour
{
    [SerializeField] private GameObject firePrefab;
    [SerializeField] private int amount = 6;
    [SerializeField] private float spacing = 1f;
    [SerializeField] private float flameDuration = 1.5f;
    [SerializeField] private float delayBetweenFlames = 0.1f;

    private void Start()
    {
        Debug.Log("Flame spawner started!");
        StartCoroutine(FlameBurstLoop());
    }


    IEnumerator FlameBurstLoop()
    {
        while (true)
        {
            for (int i = 0; i < amount; i++)
            {
                GameObject flame = Instantiate(firePrefab);
                flame.transform.position = transform.position + new Vector3(i * spacing, 0, 0);
                Destroy(flame, flameDuration); // destroy after time
                yield return new WaitForSeconds(delayBetweenFlames);
            }

            yield return new WaitForSeconds(2f); // wait before next burst
        }
    }
}