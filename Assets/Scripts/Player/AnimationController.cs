using System.Collections; 
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator anim;

    [Header("Speed Settings")]
    public float currentSpeed = 0f;
    public float maxSpeed = 1f;
    public float acceleration = 0.3f;

    [Header("Game State")]
    public bool isGameOver = false;      
    private bool isStunned = false;      

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        
        if (isGameOver) return;

        if (!isStunned && currentSpeed < maxSpeed)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.deltaTime);
        }

        transform.Translate(Vector3.forward * currentSpeed * 5f * Time.deltaTime);

        if (anim != null)
        {
            anim.SetFloat("Speed", currentSpeed);
        }
    }

    public void HitObstacle()
    {
        StartCoroutine(SlowDownRoutine());
    }

    private IEnumerator SlowDownRoutine()
    {
        //Debug.Log("â´¹ª¹ËÑÇáµ¡ ªÐÅÍ¤ÇÒÁàÃçÇÅ§ªÑèÇ¤ÃÒÇ");

        isStunned = true;       
        currentSpeed = 0.25f;   

        yield return new WaitForSeconds(1.5f);

        isStunned = false;       
    }
}