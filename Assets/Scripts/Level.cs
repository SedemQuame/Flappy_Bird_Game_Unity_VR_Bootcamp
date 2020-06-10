using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;

public class Level : MonoBehaviour
{
    private const float CAMERA_ORTHO_SIZE = 50f;
    private const float PIPE_WIDTH = 10f;
    private const float PIPE_HEAD_HEIGHT = 1.54f;
    private const float PIPE_MOVE_SPEED = 15f;
    private const float PIPE_DESTROY_X_POSITION = -140f;
    private const float PIPE_SPAWN_X_POSITION = 140f;
    private const float BIRD_X_POSITION = 0f;

    private float pipeSpawnTimer;
    private float pipeSpawnTimerMax;
    private List<Pipe> pipeList;
    private float gapSize;
    private int pipeSpawned;
    private int pipesPassedCount;
    private State state;

    private static Level instance;
    public static Level GetInstance()
    {
        return instance;
    }

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard,
        Impossible
    }

    private enum State
    {
        WaitingToStart,
        Playing,
        BirdDead,
    }

    private void Awake()
    {
        instance = this;
        pipeList = new List<Pipe>();
        pipeSpawnTimerMax = 2.4f;
        SetDifficulty(Difficulty.Easy);
        state = State.WaitingToStart;
    }

    public void Start()
    {
        Bird.GetInstance().OnDied += Bird_OnDied;
        Bird.GetInstance().OnStartedPlaying += Level_OnStartedPlaying;
    }

    private void Level_OnStartedPlaying(object sender, System.EventArgs e)
    {
        state = State.Playing;
    }

    private void Bird_OnDied(object sender, System.EventArgs e)
    {
        state = State.BirdDead;
    }

    private void Update()
    {
        if(state == State.Playing)
        {
            HandlePipeMovement();
            HandlePipeSpawning();
        }
    }

    private void HandlePipeSpawning()
    {
        pipeSpawnTimer -= Time.deltaTime;
        if (pipeSpawnTimer < 0)
        {
            //Time to spawn another pipe
            pipeSpawnTimer += pipeSpawnTimerMax;
            float heightEdgeLimit = 10f;
            float totalHeight = CAMERA_ORTHO_SIZE * 2f;
            float minHeight = gapSize * .5f + heightEdgeLimit;
            float maxHeight = totalHeight - gapSize * .5f - heightEdgeLimit;
            float height = UnityEngine.Random.Range(minHeight, maxHeight);
            CreateGapPipes(height, gapSize, PIPE_SPAWN_X_POSITION);
        }
    }

    private void HandlePipeMovement()
    {
        for (int i = 0; i < pipeList.Count; i++)
        {
            Pipe pipe = pipeList[i];

            bool isToTheRightOfBird = pipe.GetXPosition() > BIRD_X_POSITION;
            pipe.Move();
            if(isToTheRightOfBird && pipe.GetXPosition() <= BIRD_X_POSITION)
            {
                pipesPassedCount++;
            }
            if (pipe.GetXPosition() < PIPE_DESTROY_X_POSITION)
            {
                pipe.DestroySelf();
                pipeList.Remove(pipe);
                i--;
            }
        }
    }

    private void SetDifficulty(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                gapSize = 50f;
                pipeSpawnTimerMax = 2.4f;
                break;
            case Difficulty.Medium:
                gapSize = 40f;
                pipeSpawnTimerMax = 2.2f;
                break;
            case Difficulty.Hard:
                gapSize = 30f;
                pipeSpawnTimerMax = 2.0f;
                break;
            case Difficulty.Impossible:
                gapSize = 20f;
                pipeSpawnTimerMax = 1.8f;
                break;
        }
    }

    private Difficulty GetDifficulty()
    {
        if (pipeSpawned >= 50) return Difficulty.Impossible;
        if (pipeSpawned >= 40) return Difficulty.Hard;
        if (pipeSpawned >= 30) return Difficulty.Medium;
        return Difficulty.Easy;
    }

    private void CreateGapPipes(float gapY, float gapSize, float xPosition)
    {
        CreatePipe(gapY - gapSize * .5f, xPosition, true);
        CreatePipe(CAMERA_ORTHO_SIZE * 2f - gapY - gapSize * .5f, xPosition, false);
        pipeSpawned++;
        SetDifficulty(GetDifficulty());
    }

    private void CreatePipe(float height, float xPosition, bool createBottom)
    {
        //Set Up Head
        Transform pipeHead = Instantiate(GameAssets.GetInstance().pfPipeHead);
        float pipeHeadYPosition;
        if (createBottom)
        {
            pipeHeadYPosition = -CAMERA_ORTHO_SIZE + height - PIPE_HEAD_HEIGHT * .5f;
        } else
        {
            pipeHeadYPosition = CAMERA_ORTHO_SIZE - height + PIPE_HEAD_HEIGHT * .5f;
        }
        pipeHead.position = new Vector3(xPosition, pipeHeadYPosition);
        


        //Set Up Body
        Transform pipeBody = Instantiate(GameAssets.GetInstance().pfPipeBody);
        float pipeBodyYPosition;
        if (createBottom)
        {
            pipeBodyYPosition = -CAMERA_ORTHO_SIZE;
        }
        else
        {
            pipeBodyYPosition = +CAMERA_ORTHO_SIZE;
            pipeBody.localScale = new Vector3(1, -1, 1);
        }
        pipeBody.position = new Vector3(xPosition, pipeBodyYPosition);

        SpriteRenderer pipeBodySpriteRenderer = pipeBody.GetComponent<SpriteRenderer>();
        pipeBodySpriteRenderer.size = new Vector2(PIPE_WIDTH, height);

        BoxCollider2D pipeBodyBoxCollider = pipeBody.GetComponent<BoxCollider2D>();
        pipeBodyBoxCollider.size = new Vector2(PIPE_WIDTH, height);
        pipeBodyBoxCollider.offset = new Vector2(0f, height * .5f);

        Pipe pipe = new Pipe(pipeHead, pipeBody);
        pipeList.Add(pipe);
    }

    private class Pipe
    {
        private Transform pipeHeadTransform;
        private Transform pipeBodyTransform;

        public Pipe(Transform pipeHeadTransform, Transform pipeBodyTransform)
        {
            this.pipeBodyTransform = pipeBodyTransform;
            this.pipeHeadTransform = pipeHeadTransform;
        }

        public void Move()
        {
            pipeHeadTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;
            pipeBodyTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;
        }

        public float GetXPosition()
        {
            return pipeBodyTransform.position.x;
        }

        public void DestroySelf()
        {
            Destroy(pipeBodyTransform.gameObject);
            Destroy(pipeHeadTransform.gameObject);
        }
    }

    public int GetPipeSpawned()
    {
        return pipeSpawned;
    }

    public int GetPipesPassedBird()
    {
        return pipesPassedCount;
    }



}
