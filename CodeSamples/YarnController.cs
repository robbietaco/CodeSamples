using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YarnController : MonoBehaviour
{

    [Header("Level Objects")]
    public GameObject spool;
    public GameObject player;
    public GameObject levelCompletePanel;

    [Header("Yarn Stats")]
    public float yarnLength;
    private float remainingLength;
    
    LineRenderer yarnLine;
    bool levelComplete = false;

    void Start()
    {
        //raycast collider config
        Physics2D.queriesStartInColliders = false;

        //assign yarnObject and its line renderer
        if(!player.GetComponent<LineRenderer>()){
            player.AddComponent<LineRenderer>();
        }
        yarnLine = player.GetComponent<LineRender>();

        //yarn linerenderer config
        yarnLine.SetPosition(0, spool.transform.position);
        yarnLine.startWidth = .5f;
        yarnLine.endWidth = .5f;
    }

    void Update()
    {
        //cast yarn line
        RaycastHit2D hit;
        float rayDirX = yarnLine.GetPosition(yarnLine.positionCount -1).x -  yarnLine.GetPosition(yarnLine.positionCount-2).x;
        float rayDirY = yarnLine.GetPosition(yarnLine.positionCount -1).y -  yarnLine.GetPosition(yarnLine.positionCount-2).y;
        Vector2 direction = new Vector2(rayDirX, rayDirY);
        Ray2D ray = new Ray2D(yarnLine.GetPosition(yarnLine.positionCount -2), direction);
        float distance = Vector2.Distance(yarnLine.GetPosition(yarnLine.positionCount -2), yarnLine.GetPosition(yarnLine.positionCount-1));
        float hitAngle;

        if(yarnLine.positionCount > 2){
            Vector2 dirOne = yarnLine.GetPosition(yarnLine.positionCount - 2) - yarnLine.GetPosition(yarnLine.positionCount -3);
            Vector2 dirTwo = yarnLine.GetPosition(yarnLine.positionCount - 1) - yarnLine.GetPosition(yarnLine.positionCount -2);
            hitAngle = Vector2.Angle(dirOne, dirTwo);
        }else{
            hitAngle = Mathf.Abs(Vector2.Angle(yarnLine.GetPosition(yarnLine.positionCount -2), yarnLine.GetPosition(yarnLine.positionCount-1)));
        }

        if(hit = Physics2D.Raycast(ray.origin, ray.direction, distance)){
            //when our yarn cast collides with a collider
            if(hit.collider != null){
                    //get direction and distance from collision object's center to collision point
                    Vector2 hitPointDir = new Vector2(hit.point.x - hit.collider.transform.position.x, hit.point.y - hit.collider.transform.position.y);
                    float hitPointDist = Vector2.Distance(hit.point, hit.collider.transform.position);
                    //set new point of collision
                    Vector2 hitPoint = hit.point + hitPointDir * .05f;

                    //if we hit a friendly or enemy object, kill it
                    if(hit.collider.gameObject.CompareTag("enemy")){
                        hit.collider.gameObject.GetComponent<enemyDeath>().doDeath();
                    }else if(hit.collider.gameObject.CompareTag("friendly")){
                        hit.collider.gameObject.GetComponent<friendlyDeath>().doDeath();
                    }
                    
                    //if we hit an object in the level, add a point to the yarnLine, and cast a new ray
                    else if(hitAngle > 5f && hit.collider.gameObject != player){
                        yarnLine.SetPosition(yarnLine.positionCount - 1, hitPoint);
                        yarnLine.positionCount += 1;
                        
                        //new ray
                        Vector2 newDirection = new Vector2(player.transform.position.x - hitPoint.x, player.transform.position.y - hitPoint.y);
                        ray = new Ray2D(hitPoint, newDirection);
                    }else if(hit.collider.gameObject == player && yarnLine.positionCount > 2){
                        yarnLine.positionCount -= 1;
                    }
            }
        }

        yarnLine.SetPosition(yarnLine.positionCount -1, player.transform.position);


        float distanceMoved = 0;
        for(int i = 0; i < yarnLine.positionCount -1; i++){
            float pointDistance = Vector2.Distance(yarnLine.GetPosition(i), yarnLine.GetPosition(i+1));
            distanceMoved = distanceMoved + pointDistance;

            if(pointDistance > .1f){
                RaycastHit2D lazer;
                Vector2 lazerDir = new Vector2(yarnLine.GetPosition(i).x - yarnLine.GetPosition(i+1).x, yarnLine.GetPosition(i).y - yarnLine.GetPosition(i+1).y);
                float lazerDist = Vector2.Distance(yarnLine.GetPosition(i), yarnLine.GetPosition(i+1));
                Ray2D lazerRay = new Ray2D(yarnLine.GetPosition(i+1), lazerDir);
                if(lazer = Physics2D.Raycast(lazerRay.origin, lazerRay.direction, lazerDist)){
                    if(lazer.collider.gameObject.CompareTag("bullet")){
                        lazer.collider.gameObject.GetComponent<bullet>().KillObject();
                    }
                }
            }
        }
        remainingLength = yarnLength - distanceMoved;


        if(distanceMoved < 1f){
            if(!levelComplete){
                GameObject.Find("LevelCompleteManager").GetComponent<LevelCompleteManager>().CompleteLevel();
                levelComplete = true;
            }
        }
    }
}
