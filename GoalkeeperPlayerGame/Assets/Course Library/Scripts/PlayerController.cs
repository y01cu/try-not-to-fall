using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb;
    public float speed = 5;
    private float powerupStrength = 25;
    private GameObject focalPoint;
    public GameObject powerupIndicator;
    public GameObject pauseMenu;
    public GameObject player;
    public AudioClip bumpSound;
    public AudioClip buzzerSound;
    // Added AudioSource in order to use audioclips.
    private AudioSource playerAudio;
    private float powerupIndicatorTurnSpeed = 300;
    public bool hasPowerup = false;
    public bool isAlive = true;
    public GameManager gameManager;
    public bool isPaused = false;
    // Assigned powerupCoroutine here in order to solve scope issues.
    private Coroutine powerupCoroutine;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();        
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");
        playerAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        float forwardInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalPoint.transform.forward * speed * forwardInput);
        powerupIndicator.transform.position = transform.position - new Vector3(0, 0.5f, 0);
        powerupIndicator.gameObject.transform.Rotate(Vector3.up * Time.deltaTime * powerupIndicatorTurnSpeed);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame(); 
        }

        if (player.transform.position.y < -12)
        {            
            Destroy(player);
            gameManager.GameOver();
        }
    }

    // Pauses game.
    public void PauseGame()
    {

        if (isPaused)
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
            isPaused = false;

        }
        else
        { 
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
            isPaused = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Powerup") && !hasPowerup)
        {
            playerAudio.PlayOneShot(buzzerSound, 1);
            powerupIndicator.gameObject.SetActive(true);
            hasPowerup = true;
            Destroy(other.gameObject);
            powerupCoroutine = StartCoroutine(PowerupCountdownRoutine());
        }
        else if (other.CompareTag("Powerup") && hasPowerup)
        {
            StopCoroutine(powerupCoroutine);
            powerupIndicator.gameObject.SetActive(true);
            hasPowerup = true;
            Destroy(other.gameObject);
            powerupCoroutine = StartCoroutine(PowerupCountdownRoutine());
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && hasPowerup)
        {
            //Rigidbody enemyRb = GameObject.Find("Enemy").GetComponent<Rigidbody>();
            Rigidbody enemyRb = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = (collision.gameObject.transform.position - transform.position);
            enemyRb.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse);
            playerAudio.PlayOneShot(bumpSound, 1);
        }
    }

    IEnumerator PowerupCountdownRoutine()
    {
        yield return new WaitForSeconds(7);
        hasPowerup = false;
        powerupIndicator.gameObject.SetActive(false);
    }
}
