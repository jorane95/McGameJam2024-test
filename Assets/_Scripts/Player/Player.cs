using System;
using Unity.VisualScripting;
using UnityEngine;

namespace _Scripts.Units.Player
{
    /// <summary>
    /// Main class for the player
    /// </summary>

//-------------------------------------------------------------------------------------------//
    public class Player : MonoBehaviour
    {
        public LayerMask platformLayerMask;
        public CharacterController2D _characterController;
        public GameObject[] gadgets;
        private UIManager _uiManager;

        public GameObject currentlySelectedGadget;
        public int currentlySelectedGadgetNumber;

        private Rigidbody2D rb;

        public float mass = 0f;
        public float speed = 20f;
        public float airDrag = 0f;
        public float adherence;
        
        float _horizontalInput;
        int _currentHealth=100;
        bool _jump;
        bool _isDead;

        private Transform topPos;
        private Transform topLeftPos;
        private Transform topRightPos;
        private Transform frontPos;
        private Transform middlePos;
        private Transform backPos;
        private Transform bottomRightPos;
        private Transform bottomLeftPos;
        private Transform bottomPos;
        
        private GameObject top;
        private GameObject topLeft;
        private GameObject topRight;
        private GameObject front;
        private GameObject middle;
        private GameObject back;
        private GameObject bottomRight;
        private GameObject bottomLeft;
        private GameObject bottom;
        
        Transform instantiateAt;
        private Vector3 instantiatePos;

        //-------------------------------------------------------------------------------------------//


        //-------------------------------------------------------------------------------------------//
        // START
        void Start()
        {
            topPos = transform.Find("Top");
            topLeftPos = transform.Find("Top Left");
            topRightPos = transform.Find("Top Right");
            frontPos = transform.Find("Front");
            middlePos = transform.Find("Middle");
            backPos = transform.Find("Back");
            bottomLeftPos = transform.Find("Bottom Left");
            bottomRightPos = transform.Find("Bottom Right");
            bottomPos = transform.Find("Bottom");

            rb = GetComponent<Rigidbody2D>();
            _uiManager = GameObject.Find("Main Canvas").GetComponent<UIManager>();
        }

        //-------------------------------------------------------------------------------------------//
        // UPDATE
        void Update()
        {
            
            if (!_isDead)
            {
                _horizontalInput = Input.GetAxisRaw("Horizontal") *  speed;
                
                if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) )
                {
                    Jump();
                }
            }
            else 
            {
                _horizontalInput =0f;
            }
        
        
        }

        void FixedUpdate()
        {
            // if (GameManager.playerControl)
            // {
                _characterController.Move(_horizontalInput* Time.fixedDeltaTime, false, _jump);
                _jump = false;
            // }
        

        }

        //-------------------------------------------------------------------------------------------//

        /*
    MOVEMENT FUNCTIONS
    */
        public void OnLanding()
        {
        }
        public bool isGrounded()
        {
            CircleCollider2D collider = GetComponent<CircleCollider2D>();
            bool grounded = collider.IsTouchingLayers(platformLayerMask);
            return grounded;
        }
        public void Jump()
        {
            _jump = true;
        }

        //-------------------------------------------------------------------------------------------//
        
        void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("DeathFloor"))
            {
                TakeDamage(100);
            }
        }
        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("DeathFloor"))
            {
                TakeDamage(100);
            }  
        }

        void OnTriggerExit2D(Collider2D other)
        {

        }
        

        //-------------------------------------------------------------------------------------------//
        
        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;
            if (_currentHealth <= 0)
            {
                Die();
            }
        }
        
        void Die()
        {
            Invoke("GameOverSequence",0.35f);
            FindObjectOfType<AudioManager>().Play("Death1");
            _isDead = true;
            GetComponent<CharacterController2D>().enabled = false;
            Destroy(this.gameObject, 4f);
        }
        //-------------------------------------------------------------------------------------------//
        // UI AND SCORE FUNCTIONS
        //-------------------------------------------------------------------------------------------//
        
        void GameOverSequence()
        {
            _uiManager.GameOverSequence();
        }
        public void LevelComplete()
        {
            _uiManager.LevelComplete();
        }

        public void SelectGadget(int number)
        {

            currentlySelectedGadget = gadgets[number];
            currentlySelectedGadgetNumber = number;
            Debug.Log("Selected "+currentlySelectedGadget);
            FindObjectOfType<AudioManager>().Play("SelectGadgetSound");

        }

        public void SetGadget(string positionName)
        {
            if (transform.Find(positionName).childCount > 0) {
                Destroy(transform.Find(positionName).GetChild(0).gameObject);
            }

            if (currentlySelectedGadget == null) {
                Debug.Log("il faut sélectionner un objet à mettre sur la position " + positionName);
                return;
            }

            switch (currentlySelectedGadgetNumber)
            {
                case 0:
                    FindObjectOfType<AudioManager>().Play("WoodSound");
                    break;
                case 1:
                    FindObjectOfType<AudioManager>().Play("MotorSound");
                    break;
                case 2:
                    FindObjectOfType<AudioManager>().Play("BalloonSound");
                    break;
                case 3:
                    FindObjectOfType<AudioManager>().Play("UmbrellaSound");
                    break;
                case 4:
                    FindObjectOfType<AudioManager>().Play("WheelSound");
                    break;
                case 5:
                    FindObjectOfType<AudioManager>().Play("WoodSound");
                    break;
                case 6:
                    FindObjectOfType<AudioManager>().Play("WoodSound");
                    break;
            }

            ScriptableGadget currentScriptableGadget =
                currentlySelectedGadget.GetComponent<Gadget>().gadgetScriptableObject;
            switch (positionName)
            {  
                case "Top Left":
                    if (topLeft != null)
                    {
                        Destroy(topLeft);
                    }
                    instantiateAt = topLeftPos;
                    instantiatePos = instantiateAt.transform.position;
                    if (currentScriptableGadget.avancedStats.isPlayer)
                    {
                        topLeft = Instantiate(currentlySelectedGadget,instantiateAt);
                        rb.mass += currentScriptableGadget.avancedStats.mass;
                    }
                    else
                    {
                        topLeft =Instantiate(currentlySelectedGadget,instantiatePos, Quaternion.identity);
                        Set_Linked(topLeft);
                        bottomRight.GetComponent<Rigidbody2D>().mass = currentScriptableGadget.avancedStats.mass;
                    }
                    break;
                case "Top":
                    if (top != null)
                    {
                        Destroy(top);
                    }
                    instantiateAt = topPos;
                    instantiatePos = instantiateAt.transform.position;
                    if (currentScriptableGadget.avancedStats.isPlayer)
                    {
                        top =Instantiate(currentlySelectedGadget,instantiateAt);
                        rb.mass += currentScriptableGadget.avancedStats.mass;
                    }
                    else
                    {
                        top =Instantiate(currentlySelectedGadget,instantiatePos, Quaternion.identity);
                        Set_Linked(top);
                        top.GetComponent<Rigidbody2D>().mass = currentScriptableGadget.avancedStats.mass;
                    }
                    break;
                case "Top Right":
                    if (topRight != null)
                    {
                        Destroy(topRight);
                    }
                    instantiateAt = topRightPos;
                    instantiatePos = instantiateAt.transform.position;
                    if (currentScriptableGadget.avancedStats.isPlayer)
                    {
                        topRight =Instantiate(currentlySelectedGadget,instantiateAt);
                        rb.mass += currentScriptableGadget.avancedStats.mass;
                    }
                    else
                    {
                        topRight =Instantiate(currentlySelectedGadget,instantiatePos, Quaternion.identity);
                        Set_Linked(topRight);
                        topRight.GetComponent<Rigidbody2D>().mass = currentScriptableGadget.avancedStats.mass;
                    }
                    break;
                case "Front":
                    if (front != null)
                    {
                        Destroy(front);
                    }
                    instantiateAt = frontPos;
                    instantiatePos = instantiateAt.transform.position;
                    if (currentScriptableGadget.avancedStats.isPlayer)
                    {
                        front =Instantiate(currentlySelectedGadget,instantiateAt);
                        rb.mass += currentScriptableGadget.avancedStats.mass;
                    }
                    else
                    {
                        front =Instantiate(currentlySelectedGadget,instantiatePos, Quaternion.identity);
                        Set_Linked(front);
                        front.GetComponent<Rigidbody2D>().mass = currentScriptableGadget.avancedStats.mass;
                    }
                    break;
                case "Middle":
                    if (middle != null)
                    {
                        Destroy(middle);
                    }
                    instantiateAt = middlePos;
                    instantiatePos = instantiateAt.transform.position;
                    if (currentScriptableGadget.avancedStats.isPlayer)
                    {
                        middle =Instantiate(currentlySelectedGadget,instantiateAt);
                        rb.mass += currentScriptableGadget.avancedStats.mass;
                    }
                    else
                    {
                        middle =Instantiate(currentlySelectedGadget,instantiatePos, Quaternion.identity);
                        Set_Linked(middle);
                        middle.GetComponent<Rigidbody2D>().mass = currentScriptableGadget.avancedStats.mass;
                    }
                    break;
                case "Back":
                    if (back != null)
                    {
                        Destroy(back);
                    }
                    instantiateAt = backPos;
                    instantiatePos = instantiateAt.transform.position;
                    if (currentScriptableGadget.avancedStats.isPlayer)
                    {
                        back =Instantiate(currentlySelectedGadget,instantiateAt);
                        rb.mass += currentScriptableGadget.avancedStats.mass;
                    }
                    else
                    {
                        back =Instantiate(currentlySelectedGadget,instantiatePos, Quaternion.identity);
                        Set_Linked(back);
                        back.GetComponent<Rigidbody2D>().mass = currentScriptableGadget.avancedStats.mass;
                    }
                    break;
                case "Bottom Left":
                    if (bottomLeft != null)
                    {
                        Destroy(bottomLeft);
                    }
                    instantiateAt = bottomLeftPos;
                    instantiatePos = instantiateAt.transform.position;
                    if (currentScriptableGadget.avancedStats.isPlayer)
                    {
                        bottomLeft =Instantiate(currentlySelectedGadget,instantiateAt);
                        rb.mass += currentScriptableGadget.avancedStats.mass;
                    }
                    else
                    {
                        bottomLeft =Instantiate(currentlySelectedGadget,instantiatePos, Quaternion.identity);
                        Set_Linked(bottomLeft);
                        bottomLeft.GetComponent<Rigidbody2D>().mass = currentScriptableGadget.avancedStats.mass;
                    }
                    break;
                case "Bottom":
                    if (bottom != null)
                    {
                        Destroy(bottom);
                    }
                    instantiateAt = bottomPos;
                    instantiatePos = instantiateAt.transform.position;
                    if (currentScriptableGadget.avancedStats.isPlayer)
                    {
                        bottom =Instantiate(currentlySelectedGadget,instantiateAt);
                        rb.mass += currentScriptableGadget.avancedStats.mass;
                    }
                    else
                    {
                        bottom =Instantiate(currentlySelectedGadget,instantiatePos, Quaternion.identity);
                        Set_Linked(bottom);
                        bottom.GetComponent<Rigidbody2D>().mass = currentScriptableGadget.avancedStats.mass;
                    }
                    break;
                case "Bottom Right":
                    if (bottomRight != null)
                    {
                        Destroy(bottomRight);
                    }
                    instantiateAt = bottomRightPos;
                    instantiatePos = instantiateAt.transform.position;
                    if (currentScriptableGadget.avancedStats.isPlayer)
                    {
                        bottomRight =Instantiate(currentlySelectedGadget,instantiateAt);
                        rb.mass += currentScriptableGadget.avancedStats.mass;
                    }
                    else
                    {
                        bottomRight = Instantiate(currentlySelectedGadget,instantiatePos, Quaternion.identity);
                        Set_Linked(bottomRight);
                        bottomRight.GetComponent<Rigidbody2D>().mass = currentScriptableGadget.avancedStats.mass;
                    }
                    break;
                
            }  
        } 

        void Set_Linked(GameObject object_created)
        {
            if (object_created == null) {
                Debug.LogError("Set_Linked : object_created shouldn't be null");
                return;
            }
            Rigidbody2D object_created_rb = object_created.GetComponent<Rigidbody2D>();
            if (object_created_rb == null) {
                Debug.LogError("Set_Linked : Rigidbody2D components not found on GameObjects!");
                return;
            }

            FixedJoint2D fixedJoint = object_created_rb.AddComponent<FixedJoint2D>();
            // Connect the FixedJoint2D to the second GameObject's Rigidbody2D
            fixedJoint.connectedBody = rb;
        } 
    }
}
