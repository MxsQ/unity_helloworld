using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyAnimationConfig : ScriptableObject
{
    [SerializeField] AnimationClip move = default;

    [SerializeField] AnimationClip intro = default;

    [SerializeField] AnimationClip outro = default;

    public AnimationClip Move => move;

    public AnimationClip Intro => intro;

    public AnimationClip Outro => outro;

}
