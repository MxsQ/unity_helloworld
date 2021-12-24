using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameScenario : ScriptableObject
{
    [SerializeField] EnemyWave[] waves = { };

    [SerializeField, Range(0, 10)] int cycles = 1;

    [SerializeField, Range(0f, 1f)] float cycleSpeedUp = 0.5f;

    public State Begin() => new State(this);

    [System.Serializable]
    public class State
    {
        GameScenario scenario;

        int cycle, index;

        EnemyWave.State wave;

        float timeScale;

        public State(GameScenario scenario)
        {
            this.scenario = scenario;
            cycle = 0;
            index = 0;
            timeScale = 1;
            Debug.Assert(scenario.waves.Length > 0, "Empty senario!");
            wave = scenario.waves[0].Begin();
        }

        public bool Progress()
        {
            float delatTime = wave.Progress(timeScale * Time.deltaTime);
            while (delatTime >= 0f)
            {
                if (++index >= scenario.waves.Length)
                {
                    if (++cycle >= scenario.cycles && scenario.cycles > 0)
                    {
                        return false;
                    }
                    index = 0;
                    timeScale += scenario.cycleSpeedUp;
                }

                wave = scenario.waves[index].Begin();
                delatTime = wave.Progress(Time.deltaTime);
            }
            return true;
        }
    }
}
