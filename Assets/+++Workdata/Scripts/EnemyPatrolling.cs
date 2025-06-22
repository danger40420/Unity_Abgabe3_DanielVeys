using System.Collections;
using UnityEngine;

public class EnemyPatrolCoroutine : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float speed = 2f;

    private Transform target;

    private void Start()
    {
        target = pointB;
        StartCoroutine(MoveBetweenPoints());
    }

    IEnumerator MoveBetweenPoints()
    {
        while (true) // Endless patrol loop
        {
            // Move toward the current target
            while (Vector2.Distance(transform.position, target.position) > 0.05f)
            {
                // Move enemy
                transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

                // Flip sprite depending on direction
                Vector3 scale = transform.localScale;
                // Only flip if X movement is significant (prevents flipping on vertical patrols)
                float directionX = target.position.x - transform.position.x;
                if (Mathf.Abs(directionX) > 0.01f)
                {
                    scale.x = directionX < 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
                }
                transform.localScale = scale;

                yield return null; // Wait for next frame
            }

            // Switch target point
            target = (target == pointA) ? pointB : pointA;

            // Optional pause at turning point
            yield return new WaitForSeconds(0.5f);
        }
    }
}