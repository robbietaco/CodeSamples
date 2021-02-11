using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Handles video display
//Starts and ends video w/ replay func
//Handles UI for call UIBar and Correct/Incorrect calls
//Handles Animation for LessonObjects
public class videoPlayerScript : MonoBehaviour
{
    //VideoObjects
    UnityEngine.Video.VideoPlayer videoPlayer;
    UnityEngine.Video.VideoClip videoClip;

    //LessonManagers
    LessonConstructorScript LessonConstructor;

    [Header("UI Objects")]
    public GameObject SeeAnalysis;
    public GameObject TransitionPanel;
    public GameObject UIBar;


    //Animations
    Animator MakeTheCallAnim, SeeAnalysisAnim, NextLessonAnim, EndLessonAnim, TransitionAnim;

    public bool onScreenCallMade;

    void Start()
    {
        //LessonManagers
        LessonConstructor = GameObject.FindWithTag("Lesson").GetComponent<LessonConstructorScript>();

        //VideoObjects
        videoPlayer = GetComponent<UnityEngine.Video.VideoPlayer>();
        DetermineClipSource(LessonConstructor.isURL);


        //UI Animation Config
        if(UIBar == null){
            UIBar = GameObject.Find("UIBar");
        }
        UIBar.GetComponent<Animator>().SetBool("isOn", false);
        NextLessonAnim = GameObject.Find("NextLessonButton").GetComponent<Animator>();
        EndLessonAnim = GameObject.Find("EndLessonButton").GetComponent<Animator>();

        SeeAnalysisAnim = SeeAnalysis.GetComponent<Animator>();

        TransitionPanel = GameObject.Find("TransitionScreenPanel");
        TransitionAnim = TransitionPanel.GetComponent<Animator>();
        TransitionPanel.SetActive(false);

        //StartVideoLesson
        if(videoPlayer.isPrepared && !(videoPlayer.clip == null && videoPlayer.url == null)){
            videoPlayer.Play();
        }else{
            Debug.Log("Lesson Disrupted" + videoPlayer.source.ToString());
        }
    }

    void DetermineClipSource(bool isURL){
        if(isURL){
            videoPlayer.source = UnityEngine.Video.VideoSource.Url;
            videoPlayer.url = LessonConstructor.gameplayURL;
        }else{
            videoPlayer.source = UnityEngine.Video.VideoSource.VideoClip;
            videoClip = LessonConstructor.playVideo;
            videoPlayer.clip = videoClip;
        }
    }



    void FixedUpdate()
    {
        //if an unedited video has reached the calltime mark, or if an edited video has ended
        if((videoPlayer.time >= LessonConstructor.callTime || videoPlayer.time >= videoPlayer.clip.length)) && videoPlayer.isPrepared)
        {
            //if the user is watching the lesson video
            if(videoPlayer.url == LessonConstructor.gameplayURL){
                //prompt with signal options
                CallMadeUI();
                onScreenCallMade = true;
            }
            //if the user is watching the analysis video
            else if(videoPlayer.url == LessonConstructor.analysisURL){
                //prompt with transition UI
                StartTransitionAnims();
            }
            videoPlayer.Pause();
        }
    }

    //Transition UI Animation Handler
    void StartTransitionAnims(){
        UIBar.GetComponent<Animator>().SetBool("isOn", false);
        TransitionPanel.SetActive(true);
        TransitionAnim.SetBool("isOn", true);
        TransitionPanel.transform.parent.gameObject.SetActive(true);
    }


    //UI Handler
    void CallMadeUI(){
        if(!onScreenCallMade){
            UIBar.GetComponent<Animator>().SetBool("isOn", true);
            MakeTheCallAnim.SetBool("isActive", true);
        }
    }

    //called when a signal has been made by the user, gets the name of the call and UIObject associated with it
    public void CheckCall(string callName, GameObject callUIObject){
        if(onScreenCallMade){

            SeeAnalysisAnim.SetBool("isActive", true);
            NextLessonAnim.SetBool("isActive",true);
            EndLessonAnim.SetBool("isActive", true);

            //CallUI Shows user correct or incorrect
            if(callName == LessonConstructor.correctCall){
                callUIObject.transform.Find("Correct").gameObject.SetActive(true);
            }else{
                callUIObject.transform.Find("Incorrect").gameObject.SetActive(true);
            }

            //reset call
            onScreenCallMade = false;
        }
    }

    //Replay Functionality, called from UICornerHub
    //*also called from BodyManager on HeadTap()
    //*bug with halfspeed replay, changing playback to .5f and then back to 1 crashes unityeditor & builds
    public void ReplayClip(){
        videoPlayer.Stop();
        videoPlayer.playbackSpeed = 1;
        videoPlayer.Play()
        MakeTheCallAnim.SetBool("isActive", false);
        onScreenCallMade = false;
    }

}
