using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : GameBehavior
{
    [SerializeField] Transform model = default;

    [SerializeField] EnemyAnimationConfig animationConfig = default;

    EnemyFactory originFactory;

    EnemyAnimator animator;

    public EnemyFactory OriginFactory
    {
        get => originFactory;
        set
        {
            Debug.Assert(originFactory == null, "Redefined origin factory");
            originFactory = value;
        }
    }

    public float Scale { get; private set; }

    GameTile tileFrom, tileTo;
    Vector3 positionFrom, positionTo;
    float progress, progressFactory;
    Direction direction;
    DirectionChange directionChange;
    float directionAngleFrom, directionAngleTo;
    float pathOffset;
    float speed;
    float Health { get; set; }

    private void Awake()
    {
        animator = new EnemyAnimator();
        animator.Configue(model.GetChild(0).gameObject.AddComponent<Animator>(), animationConfig);
    }

    private void OnDestroy()
    {
        animator.Destroy();
    }

    public void Initilize(float scale, float speed, float pathOffset, float health)
    {
        Health = health;
        Scale = scale;
        model.localScale = new Vector3(scale, scale, scale);
        this.speed = speed;
        this.pathOffset = pathOffset;
        animator.PlayIntro();
    }

    public void ApplyDamage(float damage)
    {
        Debug.Assert(damage >= 0f, "Nagative damage applied.");
        Health -= damage;
    }

    public void SpawnOn(GameTile tile)
    {
        Debug.Assert(tile.NextTileOnPath != null, "Nowhere to go!", this);
        tileFrom = tile;
        tileTo = tile.NextTileOnPath;
        progress = 0f;
        PrepareIntro();
    }

    void PrepareIntro()
    {
        positionFrom = tileFrom.transform.localPosition;
        transform.localPosition = positionFrom;
        positionTo = tileFrom.ExitPoint;
        direction = tileFrom.PathDirection;
        directionChange = DirectionChange.None;
        directionAngleFrom = directionAngleTo = direction.GetAngle();
        transform.localRotation = direction.GetRotation();
        progressFactory = speed;
    }

    void PrepareOutro()
    {
        positionTo = tileFrom.transform.localPosition;
        directionChange = DirectionChange.None;
        directionAngleTo = direction.GetAngle();
        model.localPosition = new Vector3(pathOffset, -0f);
        transform.localRotation = direction.GetRotation();
        progressFactory = speed;
    }

    public override bool GameUpdate()
    {
        if (animator.CurrentClip == EnemyAnimator.Clip.Intro)
        {
            if (!animator.IsDone)
            {
                return true;
            }
            animator.PlayMove(speed / Scale);
        }

        if (Health <= 0f)
        {
            //OriginFactory.Reclaim(this);
            Recycle();
            return false;
        }

        progress += Time.deltaTime * progressFactory;
        while (progress >= 1f)
        {
            //tileFrom = tileTo;
            //tileTo = tileTo.NextTileOnPath;

            if (tileTo == null)
            {
                TowerGame.EnemyReachedDestination();
                Recycle();
                return false;
            }

            progress = (progress - 1f) / progressFactory;
            PrepareNextState();
            progress *= progressFactory;
        }

        if (directionChange == DirectionChange.None)
        {
            transform.localPosition = Vector3.LerpUnclamped(positionFrom, positionTo, progress);
        }
        else
        {
            float angle = Mathf.LerpUnclamped(directionAngleFrom, directionAngleTo, progress);
            transform.localRotation = Quaternion.Euler(0f, angle, 0f);
        }
        return true;
    }

    void PrepareNextState()
    {
        tileFrom = tileTo;
        tileTo = tileTo.NextTileOnPath;
        positionFrom = positionTo;
        if (tileTo == null)
        {
            PrepareOutro();
            return;
        }
        positionTo = tileFrom.ExitPoint;
        directionChange = direction.GetDirectionChangeTo(tileFrom.PathDirection);
        direction = tileFrom.PathDirection;
        directionAngleFrom = directionAngleTo;
        switch (directionChange)
        {
            case DirectionChange.None: PrepareFoward(); break;
            case DirectionChange.TurnRight: PrepareTurnRight(); break;
            case DirectionChange.TurnLeft: PrepareTurnLeft(); break;
            default: PrepareTurnAround(); break;
        }
    }

    void PrepareFoward()
    {
        transform.localRotation = direction.GetRotation();
        directionAngleTo = direction.GetAngle();
        model.localPosition = new Vector3(pathOffset, -0f);
        progressFactory = speed;
    }

    void PrepareTurnRight()
    {
        directionAngleTo = directionAngleFrom + 90f;
        model.localPosition = new Vector3(pathOffset - 0.5f, 0f);
        transform.localPosition = positionFrom + direction.GetHolfVector();
        progressFactory = speed / (Mathf.PI * 0.5f * (0.5f - pathOffset));
    }

    void PrepareTurnLeft()
    {
        directionAngleTo = directionAngleFrom - 90f;
        model.localPosition = new Vector3(pathOffset + 0.5f, 0f);
        transform.localPosition = positionFrom + direction.GetHolfVector();
        progressFactory = speed / (Mathf.PI * 0.5f * (0.5f + pathOffset));
    }

    void PrepareTurnAround()
    {
        directionAngleTo = directionAngleFrom + (pathOffset < -0f ? 180f : -180f);
        model.localPosition = new Vector3(pathOffset, -0f);
        transform.localPosition = positionFrom;
        progressFactory = speed / (Mathf.PI * (Mathf.Max(Mathf.Abs(pathOffset), 0.2f)));
    }

    public override void Recycle()
    {
        originFactory.Reclaim(this);
        animator.Stop();
    }

    public enum TYPE
    {
        Small, Medium, Large
    }
}
