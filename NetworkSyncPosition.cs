using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(channel = 1, sendInterval = 0.1f)]
public class NetworkSyncPosition : NetworkBehaviour {

    [SyncVar(hook = "SyncPositionValues")]
    private Vector3 syncPos;

    [SerializeField]
    Transform myTransform;
    private float lerpRate;
    private float normalLerpRate = 15;
    private float fasterLerpRate = 25;
    private float evenFasterLerpRate = 45;

    private Vector3 lastPos;
    [SerializeField]
    private float threshold = 0.1f;
    
    private List<Vector3> syncPosList = new List<Vector3>();
    [SerializeField]
    private bool useHistoricalLerping = false;
    private float closeEnough = 0.1f;

    void Start()
    {
        //lerpRate = normalLerpRate;
        lerpRate = fasterLerpRate;
        //lerpRate = evenFasterLerpRate;
    }

    void Update()
    {
        LerpPosition();
    }

    void FixedUpdate()
    {
        TransmitPosition();

    }

    void LerpPosition()
    {
        if (!isLocalPlayer)
        {
            if (useHistoricalLerping)
            {
                HistoricalLerping();
            }
            else
            {
                OrdinaryLerping();
            }

            //Debug.Log(Time.deltaTime.ToString());
        }
    }

    [Command]
    void CmdProvidePositionToServer(Vector3 pos)
    {
        syncPos = pos;
        Debug.Log("Player Position Transmitted");
    }

    [ClientCallback]
    void TransmitPosition()
    {
        if (isLocalPlayer && Vector3.Distance(myTransform.position, lastPos) > threshold)
        {
            CmdProvidePositionToServer(myTransform.position);
            lastPos = myTransform.position;
        }
    }

    [Client]
    void SyncPositionValues(Vector3 latestPos)
    {
        syncPos = latestPos;
        syncPosList.Add(syncPos);
    }
    
    void OrdinaryLerping()
    {
        myTransform.position = Vector3.Lerp(myTransform.position, syncPos, Time.deltaTime * lerpRate);
    }

    void HistoricalLerping()
    {
        if (syncPosList.Count > 0)
        {
            myTransform.position = Vector3.Lerp(myTransform.position, syncPosList[0], Time.deltaTime * lerpRate);

            if (Vector3.Distance(myTransform.position, syncPosList[0]) < closeEnough)
            {
                syncPosList.RemoveAt(0);
            }

            if (syncPosList.Count > 10)
            {
                lerpRate = fasterLerpRate;
            }
            else
            {
                lerpRate = normalLerpRate;
            }

            //Debug.Log(syncPosList.Count.ToString());
        }
    }

}
