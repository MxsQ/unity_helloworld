using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyWave : ScriptableObject
{
    [SerializeField]
    EnemySpawnSequence[] spawnSequences = { new EnemySpawnSequence() };

    public State Begin() => new State(this);

    public class State
    {
        EnemyWave wave;

        int index;

        EnemySpawnSequence.State sequence;

        public State(EnemyWave wave)
        {
            this.wave = wave;
            index = 0;
            Debug.Assert(wave.spawnSequences.Length > 0, "Empty wave!");
            sequence = wave.spawnSequences[0].Begin();

            //Debug.Log(wave.spawnSequences.Length);
        }

        public float Progress(float deltaTime)
        {
            deltaTime = sequence.Progress(deltaTime);
            while (deltaTime >= 0f)
            {
                if (++index >= wave.spawnSequences.Length)
                {
                    return deltaTime;
                }
                //Debug.Log(index);
                sequence = wave.spawnSequences[index].Begin();
                deltaTime = sequence.Progress(deltaTime);
            }
            return -1f;
        }

    }
}
