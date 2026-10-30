using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Animator anim;
    private Rigidbody rb;
    private AudioSource audioSource;

    [Header("Score Settings")]
    public int feedBagCount = 0;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI heartText;

    [Header("Health & Invincible Settings")]
    public int currentHearts = 3;
    private bool isInvincible = false;
    public GameObject chickenModel;
    public float blinkInterval = 0.1f;

    [Header("Camera FOV Settings")]
    public CinemachineCamera cmCamera;
    public float normalFOV = 40f;
    public float maxSpeedFOV = 60f;
    public float fovChangeSpeed = 5f;

    [Header("Lane Settings")]
    private int currentLane = 1;
    public float laneDistance = 1.5f;
    public float laneSwitchSpeed = 15f;

    [Header("Speed Settings")]
    public float currentSpeed = 0f;
    public float maxSpeed = 1f;
    public float acceleration = 0.4f;
    public float forwardMoveMultiplier = 5f;

    [Header("Jump Settings")]
    public float jumpForce = 6f;
    private bool isGrounded = true;

    [Header("Audio Settings")]
    public AudioClip hitSound;
    public AudioClip jumpSound;

    [Header("Game State")]
    public bool isGameStarted = false;
    public bool isGameOver = false;
    private bool isStunned = false;

    [Header("UI Panels Settings")]
    public GameObject mainMenuPanel;
    public GameObject gameOverPanel;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (rb != null) rb.constraints = RigidbodyConstraints.FreezeRotation;

        if (chickenModel == null) chickenModel = this.gameObject;
        if (cmCamera == null) cmCamera = FindAnyObjectByType<CinemachineCamera>();

        isGameStarted = false;
        isGameOver = false;

        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        Time.timeScale = 0f;

        UpdateScoreUI();
        UpdateHeartsUI();
    }

    private void Update()
    {
        if (isGameOver) return;

        if (!isStunned && currentSpeed < maxSpeed)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.deltaTime);
        }

        float targetX = 0f;
        if (currentLane == 0) targetX = -laneDistance;
        else if (currentLane == 2) targetX = laneDistance;

        float targetZ = transform.position.z + (currentSpeed * forwardMoveMultiplier * Time.deltaTime);

        Vector3 targetPosition = new Vector3(targetX, transform.position.y, targetZ);
        transform.position = Vector3.Lerp(transform.position, targetPosition, laneSwitchSpeed * Time.deltaTime);

        if (anim != null)
        {
            anim.SetFloat("Speed", currentSpeed);
        }

        if (cmCamera != null)
        {
            float targetFOV = Mathf.Lerp(normalFOV, maxSpeedFOV, currentSpeed / maxSpeed);
            cmCamera.Lens.FieldOfView = Mathf.MoveTowards(cmCamera.Lens.FieldOfView, targetFOV, fovChangeSpeed * Time.deltaTime);
        }
    }

    public void StartGame()
    {
        isGameStarted = true;

        Time.timeScale = 1f;

        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (isGameOver || Time.timeScale == 0f) return;

        if (context.started)
        {
            Vector2 inputVector = context.ReadValue<Vector2>();

            if (inputVector.x > 0 && currentLane < 2)
            {
                currentLane++;
            }
            else if (inputVector.x < 0 && currentLane > 0)
            {
                currentLane--;
            }
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (isGameOver || Time.timeScale == 0f) return;

        if (context.performed && isGrounded && rb != null)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;

            if (audioSource != null && jumpSound != null)
            {
                audioSource.PlayOneShot(jumpSound);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isGameOver || Time.timeScale == 0f) return;

        if (other.CompareTag("Obstacle") && !isInvincible)
        {
            StartCoroutine(SlowDownRoutine());
        }

        if (other.CompareTag("Coin"))
        {
            feedBagCount++;
            //Debug.Log("คะแนนปัจจุบัน: " + feedBagCount);
            UpdateScoreUI();
            other.gameObject.SetActive(false);
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Feed: " + feedBagCount;
        }
    }

    private void UpdateHeartsUI()
    {
        if (heartText != null)
        {
            heartText.text = "X" + currentHearts;
        }
    }

    private IEnumerator SlowDownRoutine()
    {
        //Debug.Log("โดนชน Bonk!");

        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        isInvincible = true;
        isStunned = true;
        currentHearts--;

        UpdateHeartsUI();

        //Debug.Log("หัวใจเหลือ: " + currentHearts);

        if (currentHearts <= 0)
        {
            //Debug.Log("GAME OVER");
            isGameOver = true;
            currentSpeed = 0f;
            if (anim != null) anim.SetFloat("Speed", 0f);

            Time.timeScale = 0f;

            if (gameOverPanel != null) gameOverPanel.SetActive(true);
            yield break;
        }

        currentSpeed = 0.25f;

        Renderer[] renderers = chickenModel.GetComponentsInChildren<Renderer>();
        float timer = 0f;

        while (timer < 3f)
        {
            foreach (Renderer ren in renderers)
            {
                ren.enabled = !ren.enabled;
            }
            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
        }

        foreach (Renderer ren in renderers)
        {
            ren.enabled = true;
        }

        isStunned = false;
        isInvincible = false;
    }
}