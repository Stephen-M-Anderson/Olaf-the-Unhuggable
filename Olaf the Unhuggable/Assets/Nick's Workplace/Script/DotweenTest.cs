using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DotweenTest : MonoBehaviour
{
    private float cycle = 2;
    private float fastcycle = 1;
    public List<Transform> waypoints;
    public int target;
    public int control;


    // Start is called before the first frame update
    void Start()
    {
        var sequence = DOTween.Sequence();

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
