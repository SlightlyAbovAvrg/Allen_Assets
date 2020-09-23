using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MindJack : MonoBehaviour
{
    #region Cam Vars

    #region These are for the "animation"/transition of the cam

    // If you want to set these in unity, make sure to remove the assignments in Start()

    public Vector3[] EXTENDED_POS_ROT; // POS_ROT means, [0] is position, [1] is rotation
    public Vector3 ANCHOR_POS;
    public Vector3[] CLOSE_POS_ROT;
    public float TRANSITION_MAX;
    private float transitionStatus;

    #endregion

    public Camera playerView;
    private Camera enemyView;
    public RenderTexture display;

    // Whether you are "taking a picture", or viewing from an enemy
    private bool pictureMode;

    #endregion



    void Start()
    {
        #region These values are in proportion with Robot Kyle

        EXTENDED_POS_ROT = new[]{ new Vector3(-0.3f, 1.33f, 0.54f), new Vector3(100f, -26f, 0f)};
        ANCHOR_POS = new Vector3(-0.1f, 1.33f, 0.5f);
        CLOSE_POS_ROT = new[] { new Vector3(0f, 1.44f, 0.4f), new Vector3(100f, -26f, 0f) };

        TRANSITION_MAX = 1;
        transitionStatus = 0;

        #endregion

        pictureMode = true;
        enemyView = null;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && enemyView != null) // swapping modes
        {
            pictureMode = !pictureMode;
        }


        if (pictureMode) // The stuff of each mode
        {
            playerView.targetTexture = display;
            if (enemyView!=null)
                enemyView.targetTexture = null;
            
            if (Input.GetKeyDown(KeyCode.Mouse0))
                ShootBaby();
        }
        else
        {
            enemyView.targetTexture = display;
            playerView.targetTexture = null;
        }

        Aiming(); //Determines inside whether you're "aiming" or not.
    }

    /// <summary>
    /// The act of taking a picture, or "shooting"
    /// </summary>
    private void ShootBaby()
    {
        Ray rsy = new Ray(playerView.transform.position, playerView.transform.forward);

        RaycastHit hit;

        if (Physics.SphereCast(rsy, 1, out hit, 10))
        {
            if (hit.collider.tag == "Enemy")
            {
                //Enem enem = hit.collider.GetComponent<Enem>();

                //UpdateDisplay(enem.enemyCam);
            }
        }
    }

    /// <summary>
    /// Called on a successful picture taken.
    /// Swaps picture mode and updates the enemy being jacked.
    /// </summary>
    /// <param name="enemCam"></param>
    private void UpdateDisplay(Camera enemCam)
    {
        enemyView = enemCam;

        pictureMode = !pictureMode;
    }

    private void Aiming()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            if (transitionStatus < TRANSITION_MAX)
                transitionStatus +=  2.5f * Time.deltaTime;
            CamMove();
        }
        else
        {
            if (transitionStatus > 0)
                transitionStatus -= 2.5f * Time.deltaTime;
            CamMove();
        }

    }


    /// <summary>
    /// Andrew Whitfield's Bezier Curve
    /// Handles the Cam's movement from aiming.
    /// </summary>
    private void CamMove()
    {//a(1 - t)^2 + 2b(1 - t)t + ct^2
        Vector3 newPos = 
            Mathf.Pow(1 - transitionStatus, 2) * EXTENDED_POS_ROT[0] + 
            (1 - transitionStatus) * 2 * ANCHOR_POS * transitionStatus + 
            Mathf.Pow(transitionStatus, 2) * CLOSE_POS_ROT[0];

        this.transform.localPosition = newPos;

        Vector3 newRot =
            EXTENDED_POS_ROT[1] +
            ((CLOSE_POS_ROT[1] - EXTENDED_POS_ROT[1]) *
            transitionStatus);

        this.transform.localEulerAngles = newRot;
    }

}
