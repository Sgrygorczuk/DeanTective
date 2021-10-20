using System;
using UnityEngine;

public class Person : MonoBehaviour
{
    //==================================================================================================================
    // Variables 
    //==================================================================================================================
    
    //====================================================== Misc  =====================================================
    public SpriteRenderer head;                             //Keeps track of the head sprite 
    public SpriteRenderer body;                             //Keeps track of the body sprite 
    public new string name = "Person";                      //The name of the unity 
    public string[] questions = new string[] { };           //Array of questions that Dean can ask this unit    
    public string[] answers = new string[] { };             //Array of answers that the NPC can give back 
    private string _questionsFull = "";                     //A string of all the question squished to one string 

    //==================================================================================================================
    // Set Up  
    //==================================================================================================================
    
    // Start is called before the first frame update
    private void Start()
    {
        //Squishes all the questions into one string 
        if (questions == null) return;
        foreach (var question  in questions) { _questionsFull += question  + "\n"; }
    }
    
    //==================================================================================================================
    // GET Methods  
    //==================================================================================================================
    
    //Return: Head Sprite
    //Purpose: Send over the head to update the canvas image 
    public Sprite GETHead() { return head.sprite; }
    
    //Return: Body Sprite
    //Purpose: Send over the body to update the canvas image 
    public Sprite GETBody() { return body.sprite; }
    
    //Return: Unit's Name
    //Purpose: Send over the name to update the canvas text
    public string GETName() { return name; }
    
    //Return: Questions Dean can ask
    //Purpose: Send over the questions to update the canvas text
    public string GETDialogue() { return _questionsFull; }

    //Return: How many questions there are 
    //Purpose: Send over the lenght of the question array 
    public int GETQuestionLenght() { return questions.Length; }
    
    //Return: All the answers the NPC has 
    //Purpose:  Send over the answers to update the canvas text
    public string GETAnswer(int index) { return answers[index]; }
}
