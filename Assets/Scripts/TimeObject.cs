using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TimeObject : MonoBehaviour
{

    bool isRewinding = false;

    public float recordTime = 5f;

    public List<PointInTime> pointsInTime;

    Rigidbody2D rb;

    public float stopDelay = 0.2f;
    Transform player;
    public bool clonesOnRewind = false;
    bool isAClone = false;

    Movement movement;

    List<PointInTime> clonedPointsInTime;

    List<TimeObject> clonedObjects;

    int indexInTime;
    int indexFromTop;

    bool playingActions = false;
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

    }


    void FixedUpdate()
    {
        if (playingActions)
        {
            PlayPastActions();
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
                rb.isKinematic = true;
            }
        }
        else if (!isAClone)
        {
            // can't record if you're a clone.
            Record();
            rb.isKinematic = false;
            rb.simulated = true;
        }
        else if (isAClone)
        {
            playingActions = true;
        }
    }


    void PlayPastActions()
    {

        if (clonedPointsInTime.Count > 0)
        {
            if (indexFromTop > -1)
            {
                PointInTime pointInTime = clonedPointsInTime[indexFromTop];
                rb.MovePosition(pointInTime.position);
                transform.rotation = pointInTime.rotation;
                indexFromTop--;
            }
            else
            {
                rb.isKinematic = true;
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
        } else
        {
            Invoke("StopRewindNow", stopDelay);
            PlayPastActions();
        }

    }

    void CreateClone()
    {
        rb.simulated = true;
        GameObject clone = Instantiate(gameObject, transform.position, transform.rotation);
        TimeObject timeObject = clone.GetComponent<TimeObject>();
        clonedObjects.Add(timeObject);

        timeObject.isAClone = true;
        timeObject.isRewinding = true;
        clone.GetComponent<Movement>().enabled = false;
        //timeObject.indexInTime = indexInTime;
        
        timeObject.pointsInTime = new List<PointInTime>(clonedPointsInTime);
        timeObject.clonedPointsInTime = new List<PointInTime>(clonedPointsInTime);

        clonedPointsInTime.Clear();
        pointsInTime.Clear();
        clone.tag = "Untagged";
        clone.layer = LayerMask.NameToLayer("Default");
    }

    public void Rewind()
    {
        if (isAClone)
        {
            // if there is data about past
            if (pointsInTime.Count > 0)
            {
                PointInTime pointInTime = pointsInTime[0];
                rb.MovePosition(pointInTime.position);
                transform.rotation = pointInTime.rotation;
                pointsInTime.RemoveAt(0);
                indexInTime++;
            }
            else
            {
                StopRewindNow();
            }
        }      
    }

    public void Record()
    {
        if (pointsInTime.Count > Mathf.Round(recordTime / Time.fixedDeltaTime))
        {
            pointsInTime.RemoveAt(pointsInTime.Count - 1);
            clonedPointsInTime.RemoveAt(clonedPointsInTime.Count - 1);
        }

        clonedPointsInTime.Insert(0, new PointInTime(transform.position, transform.rotation));
        pointsInTime.Insert(0, new PointInTime(transform.position, transform.rotation));
    }

    public void StartRewind()
    {
        isRewinding = true;
        rb.isKinematic = true;
        CreateClone();
    }

    public void StopRewindNow()
    {
        isRewinding = false;
        rb.isKinematic = false;
        indexFromTop = indexInTime - 1;
        if (transform.GetComponent<IMoveable>() != null)
        {
            transform.GetComponent<IMoveable>().StartMoving();
        }
    }
}