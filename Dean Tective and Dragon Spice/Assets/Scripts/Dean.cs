using System;
using UnityEngine;
using UnityEngine.UI;

public class Dean : MonoBehaviour
{

    //==================================================================================================================
    // Variables 
    //==================================================================================================================
    
    //====================================================== Misc  =====================================================
    private float _xInput;          //The Horizontal Movement Input 
    private float _yInput;          //The Vertical Movement Input 
    private readonly float _speed = 15;      //The Speed at which the player moves 
    
    private Rigidbody2D _rigidbody2D;   //Connects to the physics engine 
    private Animator _animator;         //Connects to the animation engine 
    
    public SpriteRenderer interactSprite;   //Shows the player that they can interact with an object they're near 

    //====================================================== Flags =====================================================

    private bool _isTouchingPerson;             //Tells us if we're near a person so that we can bring up dialogue canvas 
    private bool _isTalking;                    //Tells us if we're talking to the person, changes the input handling 
    private bool _gettingAnswer;                //Tells us if the current dialogue is a answer from an NPC 
    
    private int _currentChoice;         //Tell us where we are in the dialogue tree 
    private int _maxChoices;            //Tells us how many options we have in the tree

    //============================================ Dialogue Canvas =====================================================
   
    public SpriteRenderer canvasBody;                   //Used to update the Body displayed in the Dialogue Canvas 
    public SpriteRenderer canvasHead;                   //Used to update the Head displayed in the Dialogue Canvas 
    public SpriteRenderer highlight;                    //Used to show which option in the dialogue tree the user is looking at 
    
    public Text canvasName;                             //Used to update the name text of the talking NPC
    public Text canvasDialogue;                         //Used to update the dialogue text of the talking NPC
    
    private Vector2 _highlightPosition;                 //Tell us the initial spot that the highlight is located at 
    public GameObject dialogueCanvas;                   //Used to turn the Dialogue Canvas on and off
    public Person person;                               //The person that we're grabbing the data from 
    

    //==================================================================================================================
    // Set Up
    //==================================================================================================================

    // Start is called before the first frame update
    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();                 //Sets up with physic engine 
        _animator = GetComponent<Animator>();                       //Sets up animations 
        interactSprite.color =  new Color(1, 1, 1, 0);       //Makes interacted indicator sprite invisible 
        
        var localPosition = highlight.transform.localPosition;  //Grabs the position of the highlight 
        _highlightPosition = new Vector2(localPosition.x, localPosition.y); //Saves that position 
    }

    
    //==================================================================================================================
    // Updates 
    //==================================================================================================================

    // Purpose: Update is called once per frame,
    private void Update()
    {
        //Updates depending on isTalking flag
        switch (_isTalking)
        {
            //If we aren't talking we can turn the canvas on 
            case false when _isTouchingPerson && Input.GetKeyDown(KeyCode.Space):
                CanvasSetUp(true);
                UpdateCanvasData();
                break;
            //If we're done talking turn off the canvas 
            case true when Input.GetKeyDown(KeyCode.Space) && (_currentChoice == _maxChoices -1 && _gettingAnswer) || _maxChoices == 0:
                CanvasSetUp(false);
                break;
        }
    }

    //Input: @visible - tells us if the canvas is on or off
    //Purpose: Sets up the Canvas to be turned on and off and reset and flags 
    private void CanvasSetUp(bool visible)
    {
        dialogueCanvas.SetActive(visible);  //Turns canvas on/off
        _isTalking = visible;               //Tells us if we're talking or not 
        //Makes the interact sprite on if near but not talking, or off if talking 
        interactSprite.color = visible ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0) ;
        
        //Resets Vars 
        _currentChoice = 0;             //Resets current choice to be the first one 
        _gettingAnswer = false;         //Tell us in dialogue tree 
        highlight.color = new Color(1, 1, 1, 1) ;   //Turns the highlight to be visible 
    }
    
    //Purpose: Updates the data displayed by the canvas 
    private void UpdateCanvasData()
    {
        canvasBody.sprite = person.GETBody();       //Update Body Sprite 
        canvasHead.sprite = person.GETHead();       //Update Head Sprite 
        canvasDialogue.text = person.GETDialogue(); //Update text 
        canvasName.text = person.GETName();         //Update name 
        _maxChoices = person.GETQuestionLenght();   //Update number of question that can be accessed 
    }
    
    //==================================================================================================================
    // Fixed Updates 
    //==================================================================================================================

    //Purpose: Updates inputs 
    private void FixedUpdate()
    {
        if (!_isTalking) { UpdateMovement(); }
        else { UpdateTalking(); }
    }

    //Purpose: Updates player movement 
    private void UpdateMovement()
    {
        //Updates Horizontal Movement 
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
            _xInput = -_speed;
        }
        else if(Input.GetKey(KeyCode.D) ||Input.GetKey(KeyCode.RightArrow)){
            _xInput = _speed;
        }
        else{
            _xInput = 0;
        }
        
        //Updates Vertical Movement 
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)){
            _yInput = _speed;
        }
        else if(Input.GetKey(KeyCode.S) ||Input.GetKey(KeyCode.DownArrow)){
            _yInput = -_speed;
        }
        else{
            _yInput = 0;
        }

        //Updates the Animation State 
        if (_xInput != 0 || _yInput != 0) { _animator.SetBool("IsRunning", true); }
        else { _animator.SetBool("IsRunning", false); }
        
        //Pass all the info into the rigidbody 
        _rigidbody2D.velocity = new Vector2(_xInput, _yInput); //Updates the ridge body 
    }

    // Purpose: Updates talking  
    private void UpdateTalking()
    {
        //If the player is selecting the choice they want to pick move the highlight to hover over options 
        if (!_gettingAnswer)
        {
            if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && _currentChoice > 0)
            {
                _currentChoice--;
            }
            else if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) &&
                     _currentChoice < _maxChoices - 1)
            {
                _currentChoice++;
            }
        }

        //Updates the y postion of the highlight 
        highlight.transform.localPosition = new Vector3(_highlightPosition.x, _highlightPosition.y - 43 * _currentChoice, 0);

        //Updates choice 
        //Enters us to the NPC's answer 
        if(Input.GetKeyDown(KeyCode.Space) && !_gettingAnswer)
        {
            canvasDialogue.text = person.GETAnswer(_currentChoice);
            _gettingAnswer = true;
            highlight.color = new Color(1, 1, 1, 0) ;
        }
        //Exits us from the NPC's answer 
        else if(Input.GetKeyDown(KeyCode.Space) && _gettingAnswer)
        {
            canvasDialogue.text = person.GETDialogue();
            _gettingAnswer = false;
            highlight.color = new Color(1, 1, 1, 1) ;
        }
        
    }
    
    //==================================================================================================================
    // Trigger Events 
    //==================================================================================================================

    
    /**
    * Input: hitBox
    * Purpose: Check if the player walked into any triggering hitBoxes   
    */
    private void OnTriggerEnter2D(Collider2D hitBox)
    {
        //Make the interact sprite pop up 
        if (hitBox.CompareTag($"Person"))
        {
            interactSprite.color = new Color(1, 1, 1, 1);
            //Saves the data from the person to be loaded into the dialogue canvas 
            person = hitBox.gameObject.GetComponent<Person>();
        }
        _isTouchingPerson = true;

    }
    
    /**
    * Input: hitBox
    * Purpose: Check if the player exits into any triggering hitBoxes   
    */
    private void OnTriggerExit2D(Collider2D hitBox)
    {
        //Make the interact sprite disappear  
        if(hitBox.CompareTag($"Person")){interactSprite.color = new Color(1, 1, 1, 0);}
        _isTouchingPerson = false;
    }
}
