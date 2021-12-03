using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Player : MonoBehaviour
{

    // UI ------------------
    public GameObject _menu;

    public GameObject passcodePanel;
    public Text passcodeText;

    public GameObject levelCompleteText;

    public GameObject namechooser;
    public GameObject inputField;
    public GameObject nameError;

    public GameObject lassoIcon;
    public GameObject shovelIcon;
    public GameObject flashlightIcon;


    public Text gadgetText;

    public Text collectedText;


    //-----

    public Animator animator;

    //Player Movement
    public float speed = 40;

    public float jumpHeight = 5f;
    public bool _isJumping = false;


    private static Rigidbody2D _rb;

    //Facing direction of player;
    private int direction;



    private bool _hasLasso = false;
    private bool _hasShovel = false;
    private bool _hasFlashlight = false;

    private int keysCollected = 0;
    private int sandCollected = 0;
    private int batteriesCollected = 0;

    //Boolean to check if key is within reach to collect with lasso
    private bool _canCollectKey = false;
    private GameObject nearbyKey;

    //Boolean to check if sand is within reach to collect
    private bool _canCollectSand = false;
    //Game object variable to hold sand which sand object is nearby to pickup
    private GameObject nearbySand;

    //Game object variable to hold currently held sand
    public GameObject heldSand;

    //boolean to check if near passcode panel
    private bool _canViewPanel;

    //boolean to check if player is near tube to add sand to container
    private bool nearTube;


    private bool _isPaused = false;

    private string _username;


    private bool _levelCompleted = false;


    // Start is called before the first frame update
    void Start()
    {

        Pause();
        _menu.SetActive(false);


        _rb = GetComponent<Rigidbody2D>();

        SetLevelText();

    }

    // Update is called once per frame
    void Update()
    {
        
        //Running movement
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0f, 0f) * speed;
        transform.position += movement * Time.deltaTime;

        //Flipping Character when turning left and right
        Vector3 charecterScale = transform.localScale;
        if (Input.GetAxis("Horizontal") < 0)
        {
            charecterScale.x = -3;
            direction = -1;
        }
        if (Input.GetAxis("Horizontal") > 0)
        {
            charecterScale.x = 3;
            direction = 1;
        }
        transform.localScale = charecterScale;
        // ==========================================

        //Jump
        if (Input.GetButtonDown("Jump") && !_isJumping)
        {
            _rb.AddForce(new Vector2(0f, jumpHeight), ForceMode2D.Impulse);
            _isJumping = true;
        }


        //Setting animator
        animator.SetFloat("Speed", movement.magnitude);
        animator.SetBool("isJumping", _isJumping);


        UseAbility();



        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }

        }
        
        //Enable gadget indicators
        if(_hasLasso)
        {
            lassoIcon.SetActive(_canCollectKey);
        } 
        else if (_hasShovel)
        {
            shovelIcon.SetActive(_canCollectSand);
        }
        else if (_hasFlashlight)
        {
            flashlightIcon.SetActive(_canViewPanel);
        }
        
        



    }

    private void UseAbility()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (_hasShovel)
            {
                //Places sand infront of player if not near the tube
                if (sandCollected == 1 && !nearTube)
                {
                    //place sand
                    Vector3 playerPos = _rb.transform.position;
                    Vector3 spawnPos = new Vector3(playerPos.x + 4 * direction, playerPos.y, playerPos.z);
                    Instantiate(heldSand, spawnPos, Quaternion.identity);

                    sandCollected = 0;
                    collectedText.text = "Sandpiles Collected: 0";
                }
                // if theres no sand in inventory, checks if the user is beside sand and collects it
                else if (sandCollected == 0)
                {
                    if (_canCollectSand)
                    {
                        Destroy(nearbySand);
                        nearbySand = null;
                        sandCollected = 1;

                        collectedText.text = "Sandpiles Collected: 1";
                    }
                }
                // places sand in container if near the tube
                else if (sandCollected == 1 && nearTube)
                {
                    Level2Controller.instance.FillCointainer();
                    sandCollected = 0;
                    collectedText.text = "Sandpiles Collected: 0";
                }

            }
            else if (_hasLasso)
            {
                if (_canCollectKey)
                    {
                        Destroy(nearbyKey);
                        nearbyKey = null;
                        keysCollected += 1;

                        collectedText.text = "Keys Collected: 1";
                    }
            }
            else if (_hasFlashlight)
            {
                if(!_levelCompleted && _canViewPanel)
                {
                    OpenPasscodePanel();
                }

            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals("shovel"))
        {
            gadgetText.text = "Gadget: Shovel (Press the 'K' key when the Shovel Icon Appears above the Player's head to collect sand piles! Activate again when sandpile is held to drop it!)";
            _hasShovel = true;
            Destroy(other.gameObject);
        }

        if (other.gameObject.tag.Equals("lasso"))
        {
            gadgetText.text = "Gadget: Lasso (Press the 'K' key when the Lasso Icon Appears above the Player's head to grab the key!)";
            _hasLasso = true;
            Destroy(other.gameObject);
        }

        if (other.gameObject.tag.Equals("flashlight"))
        {
            gadgetText.text = "Gadget: Flashlight (Press the 'K' key when the flashlight Icon Appears above the Player's head to view the passcode panel!)";
            _hasFlashlight = true;
            Destroy(other.gameObject);
        }
        
        if (other.gameObject.tag.Equals("door") && keysCollected > 0)
        {
            keysCollected -= 1;
            Destroy(other.gameObject);
            collectedText.text = "Keys Collected: 0";
        }

         if (other.gameObject.tag.Equals("spikes"))
        {
            Die();

        }

        if (other.gameObject.tag.Equals("battery"))
        {
            batteriesCollected += 1;
            Destroy(other.gameObject);
            collectedText.text = "Batteries Collected: " + batteriesCollected.ToString();
        }

        if (other.gameObject.tag.Equals("portal"))
        {
            levelCompleteText.SetActive(true);
            StartCoroutine(FinishedLevel());
            string timeScore = TimeController.instance.EndTimer();
            SaveScore(_username, timeScore);
        }

        


    }

    private void OnTriggerStay2D(Collider2D other)
    {
        //Check if you are within range of a collectable sand
        if (other.gameObject.tag.Equals("collectable_sand"))
        {
            _canCollectSand = true;
            nearbySand = other.gameObject;
        }

        //Check if you are within range of a collectable key
        if (other.gameObject.tag.Equals("key"))
        {
            _canCollectKey = true;
            nearbyKey = other.gameObject;
        }

        //Check if you are within range of the passcode panel
        if (other.gameObject.tag.Equals("panel"))
        {
            _canViewPanel = true;
        }

        if (other.gameObject.tag.Equals("tube"))
        {
            nearTube = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals("collectable_sand"))
        {
            _canCollectSand = false;
            nearbySand = null;
        }

        if (other.gameObject.tag.Equals("key"))
        {
            _canCollectKey = false;
            nearbyKey = null;
        }

        if (other.gameObject.tag.Equals("panel"))
        {
            _canViewPanel = false;
        }

        if (other.gameObject.tag.Equals("tube"))
        {
            nearTube = false;
        }
    }

    //Checks if player is grounded
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag.Equals("ground") || col.gameObject.tag.Equals("collectable_sand"))
        {
            _isJumping = false;
        }

    }

    private void OnCollisionStay2D(Collision2D other) 
    {
        if (other.gameObject.tag.Equals("ground") || other.gameObject.tag.Equals("collectable_sand"))
        {
            _isJumping = false;
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.tag.Equals("ground"))
        {
            _isJumping = true;
        }
    }


    public void RestartLevel()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void ReturnToTitle()
    {
        Resume();
        PlayerPrefs.SetString("lastPlayed", SceneManager.GetActiveScene().name);
        SceneManager.LoadScene("Title");
    }


    private void Die()
    {
        RestartLevel();
        
    }

    private void Resume()
    {
        Time.timeScale = 1;
        _menu.SetActive(false);
        _isPaused = false;
    }

    private void Pause()
    {
        Time.timeScale = 0;
        _menu.SetActive(true);
        _isPaused = true;
    }


    private void SetLevelText(){
        Scene scene = SceneManager.GetActiveScene();
        if(scene.name == "Level1")
        {
            collectedText.text = "Keys Collected: 0";
        }
        else if (scene.name == "Level2")
        {
            collectedText.text = "Sandpiles Collected: 0";
        }
        else if (scene.name =="Level3")
        {
            collectedText.text = "Batteries Collected: 0";
        }
    }

    public void EnterName()
    {
        _username = inputField.GetComponent<Text>().text;

        if (_username.Length < 3)
        {
            nameError.SetActive(true);
        }
        else
        {
            Resume();
            TimeController.instance.BeginTimer();
            namechooser.SetActive(false);
        }

    }


    private void SaveScore(string name, string timeScore)
    {
        int size;
        if (!PlayerPrefs.HasKey("size"))
        {
            PlayerPrefs.SetInt("size", 0);
        }

        size = PlayerPrefs.GetInt("size");

        PlayerPrefs.SetString(size.ToString(), name + "," + SceneManager.GetActiveScene().name + "," + timeScore);
        size += 1;
        PlayerPrefs.SetInt("size", size);


    }

    public void OpenPasscodePanel()
    {
        passcodePanel.SetActive(true);
        if(batteriesCollected == 3){
            passcodeText.text = "AH I can finally see the passcode! *Beep Boop Beep Boop*";
        }

    }

    public void LeavePasscodePanel()
    {
        passcodePanel.SetActive(false);

        if(batteriesCollected == 3)
        {
            _levelCompleted = true;
            levelCompleteText.SetActive(true);
            StartCoroutine(FinishedLevel());
            string timeScore = TimeController.instance.EndTimer();
            SaveScore(_username, timeScore);
        }
    }

    private IEnumerator FinishedLevel()
    {
        yield return new WaitForSeconds(2f);
        PlayerPrefs.SetString("lastPlayed", SceneManager.GetActiveScene().name);
        ReturnToTitle();
    }
}