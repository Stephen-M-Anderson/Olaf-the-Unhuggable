using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DotweenTest : MonoBehaviour
{
    private float cycle = 2;                //Both Cycle and fastcycle are values plugged into the dofunctions that control various attributes such as howfast the animations moves.
    private float fastcycle = 1;
    public List<Transform> waypoints;       //Waypoints are a list of objects that can be inserted to control the points where positional data is grabbed for the Dofunctions.
    public int target;                      //Target is inteded to call and control which waypoints is used in the list.
    public int control;                     //Control is a variable that determines what dofunctions is being followed by the script. If we end up doing complicated platforms, I recommend not doing it this way.
                                            //... This is purely to demonstrate and is intended to be refined later.


    // Start is called before the first frame update
    void Start()
    {
        //Initilize the do functions that will be used.
        var sequence = DOTween.Sequence();
        //Setup the control functions that makes the shape do things. Each function can be broken down thusly, A transform using the DOfunctions that just needs positional data(provided here by Waypoints.)
        //..., cycle which controls how fast a cycle is completed, and finally the Ease portion of the function. Eases can be used to add animational effects to the overall DO animatics.
        //... I used the eases here either to add effects IE. the InoutSine/Inoutbounce or inmplement looping functionality through the SetLoops.
        if (control == 0) { transform.DOMove(waypoints[target].position, cycle).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo); } 
        else if (control == 1) { transform.DOMove(waypoints[target].position, cycle).SetEase(Ease.InOutBounce).SetLoops(-1, LoopType.Yoyo); }
        else if (control == 2) { transform.DOMove(waypoints[target].position, cycle).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Restart); }
        else if (control == 3) { transform.DOMove(waypoints[target].position, fastcycle).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo); }
        else if (control == 4){
            sequence.Append(transform.DOMove(waypoints[target].position, cycle).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo));
            sequence.Append(transform.DOMove(waypoints[1].position, cycle).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo));
            sequence.OnComplete(() =>
            {
                transform.DOMove(waypoints[2].position, cycle).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
            });
        }
        //Because the DOFunctions are actually adding animational effects, simply deleting or deinstantinating objects will not kill the effects. In such cases a DOKill function must be used to terminate
        //... animation effects, otherwise they will remain and still take up processing space.
        else
        {
            transform.DOKill();
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
