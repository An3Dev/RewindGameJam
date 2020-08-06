using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TimeObject : MonoBehaviour
{

    bool isRewinding = false;

    float recordTime = 60f;

    public List<PointInTime> pointsInTime;

    Rigidbody2D rb;
    public Collider2D collider;
    public float stopDelay = 0.2f;
    Transform player;
    public bool clonesOnRewind = false;
    public bool isAClone = false;

    Movement movement;

    List<PointInTime> clonedPointsInTime;

    List<TimeObject> clonedObjects;

    int indexInTime;
    int indexFromTop;

    bool playingActions = false;

    int cloneIndex = 0;

    int rewindLimit = 100;
    int currentRewinds = 0;

    public PhysicsMaterial2D frictionMat;
    // Use this for initialization
    void Start()
    {
        if (pointsInTime == null)
        {
            pointsInTime = new List<PointInTime>();
        }
        clonedObjects = new List<TimeObject>();

        if (clonedPointsInTime == null)
        {
            clonedPointsInTime = new List<PointInTime>();
        }
        rb = GetComponent<Rigidbody2D>();
        player = MyGameManager.player;
        movement = player.GetComponent<Movement>();
        collider = movement.collider;
    }


    void FixedUpdate()
    {
        if (playingActions)
        {
            PlayPastActions();
        } else if (!isRewinding && isAClone)
        {
            Record();
        }

        if (isRewinding)
        {
            playingActions = false;
            if (isAClone)
            {
                Rewind();
            }
            else
            {
                //rb.isKinematic = true;
            }
        }
        else if (!isAClone)
        {
            
            Record();
            rb.isKinematic = false;
            rb.simulated = true;
        }
        else if (isAClone)
        {
            //playingActions = true;
            //Record();
        }
    }


    void PlayPastActions()
    {
        if (clonedPointsInTime.Count > 0)
        {
            if (indexFromTop > -1)
            {
                PointInTime pointInTime = clonedPointsInTime[indexFromTop];
                transform.position = pointInTime.position;
                //rb.MovePosition(pointInTime.position);
                //transform.right = new Vector2(pointInTime.direction, 0);
                indexFromTop--;
            }
            else
            {
                //rb.isKinematic = true;
                //rb.useFullKinematicContacts = true;
                playingActions = false;
            }

        }
    }
    public void StopRewind()
    {
        if (!isAClone)
        {
            foreach (TimeObject timeObject in clonedObjects)
            {
                timeObject.StopRewind();
            }
            Invoke("StopRewindNow", stopDelay);
            StopRewindNow();
        }
        else
        {
            //Invoke("StopRewindNow", stopDelay);
            StopRewindNow();
            PlayPastActions();
        }
    }

    void CreateClone()
    {
        rb.simulated = true;
        GameObject clone = Instantiate(gameObject, transform.position, transform.rotation);
        cloneIndex++;

        clone.name = cloneIndex.ToString();

        TimeObject timeObject = clone.GetComponent<TimeObject>();
        clonedObjects.Add(timeObject);

        timeObject.isAClone = true;
        //timeObject.isRewinding = true;
        clone.GetComponent<Movement>().enabled = false;
        //clone.GetComponent<BoxCollider2D>().isTrigger = false;
        clone.GetComponent<Collider2D>().sharedMaterial = frictionMat;
        clone.GetComponent<Rigidbody2D>().sharedMaterial = frictionMat;
        clone.GetComponent<Rigidbody2D>().mass *= 15;
        //clone.GetComponent
        timeObject.pointsInTime = new List<PointInTime>(clonedPointsInTime);
        timeObject.clonedPointsInTime = new List<PointInTime>(clonedPointsInTime);

        clonedPointsInTime.Clear();
        pointsInTime.Clear();
        indexInTime = 0;

        clone.tag = "Untagged";
        clone.layer = LayerMask.NameToLayer("Default");
    }

    public void Rewind()
    {
        if (isAClone)
        {
            pointsInTime = clonedPointsInTime.ToList<PointInTime>();

            // if there is data about past
            if (pointsInTime.Count > 0 && indexInTime < pointsInTime.Count)
            {
                PointInTime pointInTime = pointsInTime[indexInTime];
                //rb.MovePosition(pointInTime.position);
                transform.position = pointInTime.position;
                //transform.rotation = pointInTime.rotation;
                //pointsInTime.RemoveAt(0);
                indexInTime++;
            }
        }      
    }

    public void Record()
    {

        if (!isAClone)
        {
            if (pointsInTime.Count > Mathf.Round(recordTime / Time.fixedDeltaTime))
            {
                pointsInTime.RemoveAt(pointsInTime.Count - 1);
                clonedPointsInTime.RemoveAt(clonedPointsInTime.Count - 1);
            }
            clonedPointsInTime.Insert(0, new PointInTime(transform.position, transform.rotation));
            pointsInTime.Insert(0, new PointInTime(transform.position, transform.rotation));
        } else
        {
            if (clonedPointsInTime.Count > Mathf.Round(recordTime / Time.fixedDeltaTime))
            {
                pointsInTime.RemoveAt(pointsInTime.Count - 1);
            }

            pointsInTime.Insert(0, new PointInTime(transform.position, transform.rotation));
            //Debug.Log(transform.name + " Record: " + pointsInTime.Count);
        }
    }
    
    public void StartRewind()
    {
        if (currentRewinds >= rewindLimit)
        {
            return;
        }

        if (!isAClone)
        {
            currentRewinds++;
            isRewinding = true;
            rb.isKinematic = true;
            rb.useFullKinematicContacts = true;
            transform.position += Vector3.forward * 0.1f;
            CreateClone();

            foreach (TimeObject timeObject in clonedObjects)
            {
                timeObject.StartRewind();
            }
        } else
        {
            isRewinding = true;

            clonedPointsInTime = pointsInTime.ToList<PointInTime>();
            //collider.enabled = false;
        }
    }

    public void StopRewindNow()
    {
        collider.enabled = true;
        transform.position -= Vector3.forward * 0.1f;

        isRewinding = false;
        rb.isKinematic = false;
        indexFromTop = indexInTime - 1;
        //clonedPointsInTime.RemoveRange(indexInTime, pointsInTime.Count - 1);


        indexInTime = 0;
        playingActions = true;

        if (transform.GetComponent<IMoveable>() != null)
        {
            transform.GetComponent<IMoveable>().StartMoving();
        }
    }
}