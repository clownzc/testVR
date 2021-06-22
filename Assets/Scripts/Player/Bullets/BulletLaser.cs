using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 闪电是用射线标记
/// </summary>
public class BulletLaser : BulletBase
{
    enum State
    {
        None,
        Starting,
        Shooting,
    }
    Camera myCamera;//相机
    [SerializeField] GameObject startLight;//开始的光束
    [SerializeField] float startLightTime;//开始光束的时间
    [SerializeField] GameObject laser;//闪电
    [SerializeField] GameObject hitPointEffect;//打击点的特效
    [SerializeField] float damageInterval;//伤害间隔

    private State curState = State.None;
    private float playTime = 0;
    private Action loopFunc = null;

    public override void StartMove(MoableDesc desc)
    {
        if (curState != State.None) return;
        base.StartMove(desc);
        playTime = 0;
        startLight.SetActive(true);
        laser.SetActive(false);
        hitPointEffect.SetActive(false);
        ChangeState(State.Starting, LoopStart);
    }

    public override void UpdateMove()
    {
        if (loopFunc == null) return;
        loopFunc();
    }

    private void ChangeState(State newState, Action loop)
    {
        Debug.Log($"Bullet ChangeState :{newState}");
        curState = newState;
        loopFunc = loop;
    }

    private void LoopStart()
    {
        playTime += Time.deltaTime;
        startLight.transform.up = _desc._dir;        
        if (playTime > startLightTime)
        {
            playTime = 0;
            startLight.SetActive(false);
            laser.SetActive(true);
            hitPointEffect.SetActive(true);
            ChangeState(State.Shooting, LoopShooting);
        }
    }

    private void LoopShooting()
    {
        laser.transform.up = _desc._dir;
        hitPointEffect.transform.position = _desc._dest - _desc._dir;

        playTime += Time.deltaTime;
        if(playTime > damageInterval)
        {
            playTime = 0;
            _desc._damgeCallback(this);
        }

        if (HasLife == false)
        {
            laser.SetActive(false);
            ChangeState(State.None, null);
        }
    }

    public override void EndMove()
    {
        base.EndMove();
    }

}
