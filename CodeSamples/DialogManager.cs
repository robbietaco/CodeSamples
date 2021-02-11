using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [Header("UI Objects")]
    public Text nameText;
    public Text dialogText;
    public Image characterImage;
    public Animator dialogAnimator;
    public Animator characterImageAnimator;

    AudioSource speakAudio;

    private Queue<DialogPoints> points = new Queue<DialogPoints>();
    Choice choiceSet;

    bool dialogActive, isTyping, choicePresent, choiceInitiated;

    string sceneName;

    bool nextDialog;
    GameObject SetNextDialog;
    public GameObject ChoiceOptions;
    public GameObject OptionA;
    public GameObject OptionB;
    

    void Start()
    {
        dialogActive = true;
        speakAudio = GetComponent<AudioSource>();
    }

    void Update(){
        if(dialogActive){
            if(Input.GetMouseButtonDown(0) || Input.anyKey){
                if(!choiceInitiated){
                    DisplayNextSentence();
                }
            }
        }
    }

    public void StartDialog(Dialog dialog, bool isChoice, Choice choice, bool isEndTrigger, GameObject EndTrigger, string SceneName){
        //initiate new dialog from queue
        sceneName = SceneName;
        choiceInitiated = false;
        choicePresent = isChoice;
        choiceSet = choice;
        nextDialog = isEndTrigger;
        SetNextDialog = EndTrigger;
        dialogAnimator.SetBool("isOpen", true);
        dialogActive = true;

        //clear old dialog points
        points.Clear();
        //enqueue new dialog points
        foreach(DialogPoints dialogpoint in dialog.dialogPoints){
            points.Enqueue(dialogpoint);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence(){
        if(points.Count == 0){
            if(choicePresent){
                DisplayChoice();
                return;
            }
            if(nextDialog){
                Invoke("DoNextDialog", 1);
            }
            EndDialog();
            return;
        }

        DialogPoints dialogPoint = points.Peek();
        if(isTyping == false){
            StopAllCoroutines();
            StartCoroutine(TypeSentence(dialogPoint));
            return;
        }
        StopAllCoroutines();
        dialogText.text = "";
        dialogText.text = dialogPoint.sentence;
        isTyping = false;
        points.Dequeue();
    }

    IEnumerator TypeSentence (DialogPoints dialogPoint){
        isTyping = true;
        characterImage.sprite = dialogPoint.characterImage;
        characterImage.preserveAspect = true;
        characterImageAnimator.SetInteger("sideInt", dialogPoint.sideInt);
        nameText.text = dialogPoint.name;
        dialogText.text = "";
        foreach (char letter in dialogPoint.sentence.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(.02f);
            speakAudio.Play();
        }
        isTyping = false;
        if(points.Count > 0 || !choicePresent){
            points.Dequeue();
        }
    }

    void DisplayChoice(){
        choiceInitiated = true;
        StopAllCoroutines();
        StartCoroutine(TypeSentence(choiceSet.dialogPoints));
        ChoiceOptions.SetActive(true);
        OptionA.GetComponent<Text>().text = choiceSet.OptionA;
        OptionA.GetComponent<Button>().onClick.AddListener(ChooseOptionA);
        OptionB.GetComponent<Text>().text = choiceSet.OptionB;
        OptionB.GetComponent<Button>().onClick.AddListener(ChooseOptionB);
    }

    void ChooseOptionA(){
        choiceSet.TriggerA.GetComponent<DialogTrigger>().TriggerDialog();
        ChoiceOptions.SetActive(false);
    }

    void ChooseOptionB(){
        choiceSet.TriggerB.GetComponent<DialogTrigger>().TriggerDialog();
        ChoiceOptions.SetActive(false);
    }

    public void DoNextDialog(){
        SetNextDialog.SendMessage("TriggerDialog", SendMessageOptions.DontRequireReceiver);
    }

    void EndDialog(){
        dialogAnimator.SetBool("isOpen", false);
        dialogActive = false;
        characterImageAnimator.SetInteger("sideInt", 0);
    }
}
