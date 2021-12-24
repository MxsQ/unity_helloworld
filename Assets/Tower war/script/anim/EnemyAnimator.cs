using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

[System.Serializable]
public class EnemyAnimator
{
    EnemyAnimator animator;

    PlayableGraph graph;

    AnimationMixerPlayable mixer;

    public Clip CurrentClip { get; private set; }

    public bool IsDone => GetPlayable(CurrentClip).IsDone();

    public void Configue(Animator animator, EnemyAnimationConfig config)
    {
        graph = PlayableGraph.Create();
        graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        mixer = AnimationMixerPlayable.Create(graph, 3);

        var clip = AnimationClipPlayable.Create(graph, config.Move);
        mixer.ConnectInput((int)Clip.Move, clip, 0);

        clip = AnimationClipPlayable.Create(graph, config.Intro);
        clip.SetDuration(config.Intro.length);
        mixer.ConnectInput((int)Clip.Intro, clip, 0);

        clip = AnimationClipPlayable.Create(graph, config.Outro);
        mixer.ConnectInput((int)Clip.Outro, clip, 0);

        var output = AnimationPlayableOutput.Create(graph, "Enemy", animator);
        output.SetSourcePlayable(mixer);
    }

    public void PlayIntro()
    {
        SetWeight(Clip.Intro, 1f);
        CurrentClip = Clip.Intro;
        graph.Play();
    }

    public void PlayMove(float speed)
    {
        SetWeight(CurrentClip, 0f);
        SetWeight(Clip.Move, 1f);
        GetPlayable(Clip.Move).SetSpeed(speed);
        CurrentClip = Clip.Move;
    }

    public void PlayOutro()
    {
        SetWeight(CurrentClip, 0f);
        SetWeight(Clip.Outro, 1f);
        CurrentClip = Clip.Outro;
    }

    void SetWeight(Clip clip, float weight)
    {
        mixer.SetInputWeight((int)clip, weight);
    }

    Playable GetPlayable(Clip clip)
    {
        return mixer.GetInput((int)clip);
    }

    public void Stop()
    {
        graph.Stop();
    }

    public void Destroy()
    {
        graph.Destroy();
    }

    public enum Clip { Move, Intro, Outro }
}
