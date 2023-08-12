using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [Header("------- Component -------")]
    public Block blockPrefab;
    public Transform tetromino;
    public Combo comboComponent;
    public Map map;
    public GameUI gameUI;

    public Border topBorder;
    public Border leftBorder;
    public Border rightBorder;
    public Border bottomBorder;

    public RotationBorder topRotationBorder;
    public RotationBorder leftRotationBorder;
    public RotationBorder rightRotationBorder;
    public RotationBorder bottomRotationBorder;

    public Block[] spawnTetromino;
    public Block[] backgroundBlock;

    [Header("------- Board Info -------")]
    int[] tetIndex;
    public int boardWidth = 10;
    public int boardHeight = 20;

    public bool isDropping;
    public bool isLanding;

    float fallingTimer;
    float landingTimer;

    void Awake()
    {
        // 0: 현재 나올 블럭, 1: 넥스트 블럭
        tetIndex = new int[2];
        tetIndex[0] = Random.Range(0, 7);
        tetIndex[1] = Random.Range(0, 7);
    }

    void Start()
    {
        // 배경 블럭 세팅
        CreateBoardBackground();
    }

    void FixedUpdate()
    {
        // 낙하 버튼 누르면 낙하 로직 수행
        if (isDropping)
        {
            DropTetromino();
        }
    }

    void Update()
    {
        if (!GameManager.instance.isLoading && GameManager.instance.isGamePlaying)
        {
            // 바닥에 떨어지거나, 다른 블럭 위에 착지중
            if (isLanding || bottomBorder.isCollide)
            {
                fallingTimer = 0;
                LandingTetromino();
            }
            else
            {
                landingTimer = 0;
                FallingTetromino();
            }
        }
    }

    public void CreateBoardBackground()
    {
        int halfWidth = boardWidth / 2;
        int halfHeight = boardHeight / 2;
        Vector2[,] backgroundVector = new Vector2[boardHeight, boardWidth];

        // x좌표는 -5 ~ 5, y좌표는 -10 ~ 10)
        for (int i = 0; i < boardHeight; i++)
        {
            for (int j = 0; j < boardWidth; j++)
            {
                backgroundVector[i, j] = new Vector2(j- halfWidth, i- halfHeight);
                blockPrefab.transform.localPosition = backgroundVector[i,j] + new Vector2(0.5f, 0.5f);
                blockPrefab.transform.localScale = new Vector3(1, 1, 1);

                // 첫 시작 시 배경이 없으면 생성
                // 재시작 시 배경이 이미 존재하면 덮어씌우고 초기화
                if (backgroundBlock.Length == 0) Instantiate(blockPrefab, transform);
                else
                {
                    backgroundBlock[i * 10 + j].isLanded = false;
                }
            }
        }
        backgroundBlock = GetComponentsInChildren<Block>();
    }

    void ResetTetromino()
    {
        // 위치와 회전 초기화
        tetromino.localPosition = new Vector3(0, 9, 0);
        tetromino.rotation = Quaternion.identity;

        // 활성화되었던 스폰 블럭 비활성화
        spawnTetromino = tetromino.GetComponentsInChildren<Block>(true);
        foreach (Block block in spawnTetromino)
        {
            if (block.gameObject.activeSelf) block.gameObject.SetActive(false);
        }
    }

    public void InitTetromino()
    {
        ResetTetromino();

        // 스폰 블럭 생성
        CreateTetromino(tetIndex[0]);
    }

    public void CreateTetromino(int tetId)
    {
        Vector2[] positionVector = new Vector2[4];

        // 0: 하늘색, ㅣ
        // 1: 파란색, ㄴ
        // 2: 주황색, ㄴ(반대)
        // 3: 노란색, ㅁ
        // 4: 초록색, ㄴ+ㄱ
        // 5: 보라색, ㅗ
        // 6: 빨간색, ㄱ+ㄴ
        switch (tetId)
        {
            case 0:
                positionVector[0] = new Vector2(-2f, 0f);
                positionVector[1] = new Vector2(-1f, 0f);
                positionVector[2] = new Vector2(0f, 0f);
                positionVector[3] = new Vector2(1f, 0f);
                break;
            case 1:
                positionVector[0] = new Vector2(-1f, 0f);
                positionVector[1] = new Vector2(-1f, -1f);
                positionVector[2] = new Vector2(0f, -1f);
                positionVector[3] = new Vector2(1f, -1f);
                break;
            case 2:
                positionVector[0] = new Vector2(-1f, -1f);
                positionVector[1] = new Vector2(0f, -1f);
                positionVector[2] = new Vector2(1f, -1f);
                positionVector[3] = new Vector2(1f, 0f);
                break;
            case 3:
                positionVector[0] = new Vector2(-1f, -1f);
                positionVector[1] = new Vector2(0f, -1f);
                positionVector[2] = new Vector2(-1f, 0f);
                positionVector[3] = new Vector2(0f, 0f);
                break;
            case 4:
                positionVector[0] = new Vector2(-1f, -1f);
                positionVector[1] = new Vector2(0f, -1f);
                positionVector[2] = new Vector2(0f, 0f);
                positionVector[3] = new Vector2(1f, 0f);
                break;
            case 5:
                positionVector[0] = new Vector2(-1f, -1f);
                positionVector[1] = new Vector2(0f, -1f);
                positionVector[2] = new Vector2(1f, -1f);
                positionVector[3] = new Vector2(0f, 0f);
                break;
            case 6:
                positionVector[0] = new Vector2(-1f, 0f);
                positionVector[1] = new Vector2(0f, 0f);
                positionVector[2] = new Vector2(0f, -1f);
                positionVector[3] = new Vector2(1f, -1f);
                break;
        }
        CreateBlock(tetId, positionVector);
    }

    public void CreateBlock(int blockId, Vector2[] position)
    {
        // 스폰 블럭 세팅
        for (int i = 0; i < spawnTetromino.Length; i++)
        {
            spawnTetromino[i].gameObject.SetActive(true);

            spawnTetromino[i].blockId = blockId;
            spawnTetromino[i].transform.localPosition = position[i] + new Vector2(0.5f, 0.5f);

            // 스폰된 테트로미노에 이미 쌓여있는게 있다면 게임오버
            if (CheckSpecialBlockIsCollide(spawnTetromino[i], Vector3.down, 0.01f))
            {
                GameManager.instance.GameOver();
                break;
            }
        }

        // 넥스트 블럭 세팅
        gameUI.SetNextBlock(tetIndex[1]);

        // 블럭 만든 후에는 넥스트 블럭을 스폰 블럭으로 변경
        tetIndex[0] = tetIndex[1];
        tetIndex[1] = Random.Range(0, 7);
    }

    public void CreateMap(int[] mapArray)
    {
        for (int i=0; i<backgroundBlock.Length; i++)
        {
            backgroundBlock[i].blockId = mapArray[i];
            if (mapArray[i] != -1)  backgroundBlock[i].isLanded = true;
            else backgroundBlock[i].isLanded = false;
        }
    }

    public void ResetBoard()
    {
        CreateBoardBackground();
        CreateMap(GameManager.instance.mapInfo[map.mapIndex]);
        ResetTetromino();
    }

    // 테트로미노 옆에 있을때 좌우로 움직이면 뚫고 들어가는 버그 고치기
    // 착지 로직을 블럭의 충돌이 아닌 레이캐스트 거리로 수정
    public void MoveLeftTetromino()
    {
        if (leftBorder.isCollide || CheckIsCollide(Vector3.left)) return;
        tetromino.localPosition += Vector3.left;
        GameManager.instance.EffectPlay(GameManager.Effect.MOVE);
        isLanding = false;
    }

    public void MoveRightTetromino()
    {
        if (rightBorder.isCollide || CheckIsCollide(Vector3.right)) return;
        tetromino.localPosition += Vector3.right;
        GameManager.instance.EffectPlay(GameManager.Effect.MOVE);
        isLanding = false;
    }

    public void MoveDownTetromino()
    {
        if (bottomBorder.isCollide || isLanding) return;
        GameManager.instance.score += 10;
        tetromino.localPosition += Vector3.down;
        isLanding = false;
    }

    public void RotateTetromino()
    {
        // 회전 시 고려대상: 회전 후 상하좌우의 장애물, 테트로미노 모양
        // 현재 모양에서 회전하면? 근데 그 모양이 이거라면? 회전했을때 그 주위에 무언가 있다면?
        float rotateAngle = tetromino.rotation.eulerAngles.z;
        if (rotateAngle == 0)   // 정방향이므로 다음 회전하면 90도가 된다
        {
            if (spawnTetromino[0].blockId == 0)
            {
                if (CheckSpecialBlockIsCollide(spawnTetromino[1], Vector3.up, 0.6f) || CheckSpecialBlockIsCollide(spawnTetromino[1], Vector3.down, 1.6f)) return;
            }
            else if (spawnTetromino[0].blockId == 1)
            {
                if (CheckSpecialBlockIsCollide(spawnTetromino[2], Vector3.up, 1.6f))    return;
            }
            else if (spawnTetromino[0].blockId == 2)
            {
                if (CheckSpecialBlockIsCollide(spawnTetromino[0], Vector3.up, 1.6f) || CheckSpecialBlockIsCollide(spawnTetromino[1], Vector3.up, 1.6f)) return;
            }
            else if (spawnTetromino[0].blockId == 4)
            {
                if (CheckSpecialBlockIsCollide(spawnTetromino[0], Vector3.up, 1.6f))    return;
            }
            else if (spawnTetromino[0].blockId == 5)
            {
                if (CheckSpecialBlockIsCollide(spawnTetromino[0], Vector3.up, 0.6f) || CheckSpecialBlockIsCollide(spawnTetromino[3], Vector3.up, 0.6f)) return;
            }
            else if (spawnTetromino[0].blockId == 6)
            {
                if (CheckSpecialBlockIsCollide(spawnTetromino[0], Vector3.down, 0.6f) || CheckSpecialBlockIsCollide(spawnTetromino[1], Vector3.up, 0.6f))   return;
            }
        }
        else if (rotateAngle == 90)
        {
            if (spawnTetromino[0].blockId == 0)
            {
                if (CheckSpecialBlockIsCollide(spawnTetromino[1], Vector3.left, 0.6f) || CheckSpecialBlockIsCollide(spawnTetromino[1], Vector3.right, 1.6f))    return;
            }
            else if (spawnTetromino[0].blockId == 1)
            {
                if (CheckSpecialBlockIsCollide(spawnTetromino[2], Vector3.left, 1.6f))  return;
            }
            else if (spawnTetromino[0].blockId == 2)
            {
                if (CheckSpecialBlockIsCollide(spawnTetromino[0], Vector3.left, 1.6f) || CheckSpecialBlockIsCollide(spawnTetromino[1], Vector3.left, 1.6f)) return;
            }
            else if (spawnTetromino[0].blockId == 4)
            {
                if (CheckSpecialBlockIsCollide(spawnTetromino[0], Vector3.left, 1.6f))  return;
            }
            else if (spawnTetromino[0].blockId == 5)
            {
                if (CheckSpecialBlockIsCollide(spawnTetromino[0], Vector3.left, 0.6f) || CheckSpecialBlockIsCollide(spawnTetromino[3], Vector3.left, 0.6f)) return;
            }
            else if (spawnTetromino[0].blockId == 6)
            {
                if (CheckSpecialBlockIsCollide(spawnTetromino[0], Vector3.right, 0.6f) || CheckSpecialBlockIsCollide(spawnTetromino[1], Vector3.left, 0.6f))    return;
            }
        }
        else if (rotateAngle == 180)
        {
            if (spawnTetromino[0].blockId == 0)
            {
                if (CheckSpecialBlockIsCollide(spawnTetromino[1], Vector3.down, 0.6f) || CheckSpecialBlockIsCollide(spawnTetromino[1], Vector3.up, 1.6f))   return;
            }
            else if (spawnTetromino[0].blockId == 1)
            {
                if (CheckSpecialBlockIsCollide(spawnTetromino[2], Vector3.down, 1.6f))  return;
            }
            else if (spawnTetromino[0].blockId == 2)
            {
                if (CheckSpecialBlockIsCollide(spawnTetromino[0], Vector3.down, 1.6f) || CheckSpecialBlockIsCollide(spawnTetromino[1], Vector3.down, 1.6f)) return;
            }
            else if (spawnTetromino[0].blockId == 4)
            {
                if (CheckSpecialBlockIsCollide(spawnTetromino[0], Vector3.down, 1.6f))  return;
            }
            else if (spawnTetromino[0].blockId == 5)
            {
                if (CheckSpecialBlockIsCollide(spawnTetromino[0], Vector3.down, 0.6f) || CheckSpecialBlockIsCollide(spawnTetromino[3], Vector3.down, 0.6f)) return;
            }
            else if (spawnTetromino[0].blockId == 6)
            {
                if (CheckSpecialBlockIsCollide(spawnTetromino[0], Vector3.up, 0.6f) || CheckSpecialBlockIsCollide(spawnTetromino[1], Vector3.down, 0.6f))   return;
            }
        }
        else if (rotateAngle == 270)
        {
            if (spawnTetromino[0].blockId == 0)
            {
                if (CheckSpecialBlockIsCollide(spawnTetromino[1], Vector3.right, 0.6f) || CheckSpecialBlockIsCollide(spawnTetromino[1], Vector3.left, 1.6f))    return;
            }
            else if (spawnTetromino[0].blockId == 1)
            {
                if (CheckSpecialBlockIsCollide(spawnTetromino[2], Vector3.right, 1.6f)) return;
            }
            else if (spawnTetromino[0].blockId == 2)
            {
                if (CheckSpecialBlockIsCollide(spawnTetromino[0], Vector3.right, 1.6f) || CheckSpecialBlockIsCollide(spawnTetromino[1], Vector3.right, 1.6f))   return;
            }
            else if (spawnTetromino[0].blockId == 4)
            {
                if (CheckSpecialBlockIsCollide(spawnTetromino[0], Vector3.right, 1.6f)) return;
            }
            else if (spawnTetromino[0].blockId == 5)
            {
                if (CheckSpecialBlockIsCollide(spawnTetromino[0], Vector3.right, 0.6f) || CheckSpecialBlockIsCollide(spawnTetromino[3], Vector3.right, 0.6f))   return;
            }
            else if (spawnTetromino[0].blockId == 6)
            {
                if (CheckSpecialBlockIsCollide(spawnTetromino[0], Vector3.left, 0.6f) || CheckSpecialBlockIsCollide(spawnTetromino[1], Vector3.right, 0.6f))    return;
            }
        }

        tetromino.Rotate(Vector3.forward * 90f);
        GameManager.instance.EffectPlay(GameManager.Effect.ROTATE);
        isLanding = false;
    }

    void FallingTetromino()
    {
        fallingTimer += Time.deltaTime;

        // 낙하 중에 착지를 하면 바로 착지 로직 수행하도록 세팅
        foreach (Block block in spawnTetromino)
        {
            float distance = CheckLandingPosition(block);

            if (distance < 0.5f)
            {
                isLanding = true;
                GameManager.instance.EffectPlay(GameManager.Effect.DROP);
                fallingTimer = 0f;
                break;
            }
        }

        // 낙하 시간 간격마다 한칸씩 낙하
        if (fallingTimer > GameManager.instance.fallingInterval)
        {
            GameManager.instance.score += 10;
            tetromino.localPosition += Vector3.down;
            fallingTimer = 0f;
        }
    }

    void LandingTetromino()
    {
        landingTimer += Time.deltaTime;

        // 착지 시간이 되면 착지 로직 수행
        if (landingTimer > GameManager.instance.landingInterval)
        {
            foreach (Block block in spawnTetromino)
            {
                // 메인 보드 위치가 (0, -0.7) 이고, 블럭의 중심 위치가 (0.5, 0.5) 단위 -> 그래서 실질적 위치가 (0.5, -0.2)가 되어있음
                // -> 위치 차이만큼 보정해주고, 배경블럭의 첫 시작은 좌하단부터 시작이기 때문에 가로와 세로의 길이 반만큼 더해줌으로서 보정
                Vector3 mapPosition = block.transform.position + new Vector3(-0.5f, 0.2f, 0) + new Vector3(5f, 10f, 0);

                // 블럭 쌓기 완료 했다는 표시
                block.isLanded = true;

                // 배경 블럭을 현재 블럭으로 세팅하여 쌓인것 처럼 보이도록 설정
                backgroundBlock[Mathf.RoundToInt(mapPosition.y) * 10 + Mathf.RoundToInt(mapPosition.x)].blockId = block.blockId;
                backgroundBlock[Mathf.RoundToInt(mapPosition.y) * 10 + Mathf.RoundToInt(mapPosition.x)].isLanded = true;
            }

            // 착지한 테트로미노 수 추가
            GameManager.instance.blockCount++;

            // 착지 후 지워져야 할 라인이 있으면 수행
            LineClear();

            // 노말모드에서 레벨이 10 초과이거나, 하드모드이면 블럭 추가 기믹 활성화
            if((GameManager.instance.mode == 0 && GameManager.instance.level > 10) || GameManager.instance.mode == 1)
            {
                if(GameManager.instance.blockCount % 5 == 0)    AddOneBlock();
                if(GameManager.instance.blockCount % 10 == 0)   AddOneLine();
            }

            // 스테이지모드에서 특정 스테이지일 경우 블럭 추가 기믹 활성화
            if (GameManager.instance.mode == 2)
            {
                if(GameManager.instance.stage == 7)
                {
                    if (GameManager.instance.blockCount % 7 == 0) AddOneBlock();
                }
                else if (GameManager.instance.stage == 8)
                {
                    if (GameManager.instance.blockCount % 5 == 0) AddOneBlock();
                }
                else if (GameManager.instance.stage == 9)
                {
                    if (GameManager.instance.blockCount % 3 == 0) AddOneBlock();
                }
                else if (GameManager.instance.stage == 10)
                {
                    if (GameManager.instance.blockCount % 7 == 0) AddOneLine();
                }
                else if (GameManager.instance.stage == 11)
                {
                    if (GameManager.instance.blockCount % 5 == 0) AddOneLine();
                }
                else if (GameManager.instance.stage == 12)
                {
                    if (GameManager.instance.blockCount % 3 == 0) AddOneLine();
                }
            }

            // 랜딩 타이머 초기화
            isLanding = false;
            landingTimer = 0f;

            // 다음 블럭 리스폰
            InitTetromino();
        }
    }

    void DropTetromino()
    {
        if (spawnTetromino[0].isLanded) return;

        // 현재 블럭에서 착지하는 거리 계산 후 가장 작은 값으로 해서 블럭 이동
        float distance = 20f;
        foreach (Block block in spawnTetromino)
        {
            float currentBlockDistance = CheckLandingPosition(block);
            if (currentBlockDistance < distance)
            {
                distance = currentBlockDistance;
            }
        }
        distance = Mathf.RoundToInt(distance);

        // 이동 후 효과음 출력, 점수 증가
        tetromino.position = new Vector3(tetromino.position.x, tetromino.position.y - distance, tetromino.position.z);
        GameManager.instance.EffectPlay(GameManager.Effect.DROP);
        GameManager.instance.score += Mathf.RoundToInt(distance) * 10;

        isDropping = false;
        isLanding = true;
    }

    float CheckLandingPosition(Block block)
    {
        Vector3 start = block.transform.position;

        // RayCast: 오브젝트 검색을 위해 Ray를 쏘는 방식
        // Debug.DrawRay(): 에디터 상에서만 Ray를 그려주는 함수
        // RayCastHit: Ray에 닿은 오브젝트
        // collider: ray가 목표 지점에 충돌한 오브젝트
        // distance: ray가 목표지점에 닿았을 때의 거리

        //Debug.DrawRay(start, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(start, Vector3.down, boardHeight, LayerMask.GetMask("Border"));

        // 역시나 중심이 0.5이기 때문에 최종 거리에서 0.5를 뺀 값을 리턴
        return rayHit.distance - 0.5f;
    }

    public void OnDrop()
    {
        isDropping = true;
    }

    bool CheckIsCollide(Vector3 direction)
    {
        foreach (Block block in spawnTetromino)
        {
            Vector3 start = block.transform.position;
            RaycastHit2D rayHit = Physics2D.Raycast(start, direction, boardWidth, LayerMask.GetMask("Border"));

            if (rayHit.distance != 0 && rayHit.distance - 0.5f < 0.6f)
            {
                return true;
            }
        }
        return false;
    }

    bool CheckSpecialBlockIsCollide(Block startBlock, Vector3 direction, float checkDistance)
    {
        Vector3 start = startBlock.transform.position;
        RaycastHit2D rayHit = Physics2D.Raycast(start, direction, boardHeight, LayerMask.GetMask("Border"));

        if (rayHit.distance != 0 && rayHit.distance - 0.5f < checkDistance)
        {
            return true;
        }

        return false;
    }

    int findFullLine()
    {
        for (int i = 0; i < boardHeight; i++)
        {
            int calculateLine = 0;  // 한 라인에서 블럭이 채워졌는지 아닌지 계산용 변수

            // 한 라인에서 블럭이 채워졌으면 변수에 1 추가
            for (int j = 0; j < boardWidth; j++)
            {
                if (backgroundBlock[i * 10 + j].blockId == -1) break;
                else calculateLine++;
            }
            
            // 한줄이 다 채워졌으면 해당 번째 줄 리턴
            if(calculateLine == 10)
            {
                return i;
            }
        }
        return -1;
    }

    void LineClear()
    {
        // 여기서 라인 한줄이 완성되었으면 한줄 지우고 위에있는걸 아래로 끌어내리고 등등 로직 수행
        int fullLineIndex = findFullLine();

        if (fullLineIndex > -1)
        {
            List<int> clearLines = new List<int>();

            // 삭제 라인 수 별 점수 추가
            clearLines = CheckFullLineCount(fullLineIndex);

            while(clearLines.Count > 0)
            {
                // 모두 차있는 라인 삭제
                //for (int i = 0; i < boardWidth; i++)
                //{
                //    Destroy(backgroundBlock[fullLineIndex * 10 + i].gameObject);
                //}

                // 나머지 라인 밑으로 내리기
                for (int j = fullLineIndex + 1; j < boardHeight; j++)
                {
                    for (int k = 0; k < boardWidth; k++)
                    {
                        //Vector3 position = backgroundBlock[j * 10 + k].gameObject.transform.localPosition;
                        //backgroundBlock[j * 10 + k].gameObject.transform.localPosition = new Vector3(position.x, position.y - 1f, position.z);
                        //backgroundBlock[(j - 1) * 10 + k] = backgroundBlock[j * 10 + k];

                        
                        backgroundBlock[(j - 1) * 10 + k].blockId = backgroundBlock[j * 10 + k].blockId;
                        backgroundBlock[(j - 1) * 10 + k].isLanded = backgroundBlock[j * 10 + k].isLanded;
                    }
                }

                // 맨 위에 배경블럭 생성
                //int halfWidth = boardWidth / 2;
                //int halfHeight = boardHeight / 2;
                //Vector2[] backgroundVector = new Vector2[boardWidth];
                //for (int l = 0; l < boardWidth; l++)
                //{
                //    backgroundVector[l] = new Vector2(l - halfWidth, boardHeight - 1 - halfHeight);
                //    blockPrefab.transform.localPosition = backgroundVector[l] + new Vector2(0.5f, 0.5f);
                //    blockPrefab.transform.localScale = new Vector3(1, 1, 1);
                //    Block newBackgroundBlock = Instantiate(blockPrefab, transform);
                //    backgroundBlock[(boardHeight - 1) * 10 + l] = newBackgroundBlock;
                //}

                for (int i = 0; i < clearLines.Count; i++)
                {
                    clearLines[i] -= 1;
                }
                clearLines.RemoveAt(0);
            }

            // 사운드 출력
            GameManager.instance.EffectPlay(GameManager.Effect.LINECLEAR);

            // 콤보 카운트 추가
            GameManager.instance.combo++;

            // 3콤보 이상이면 점수 추가 및 화면에 콤보 애니메이션 출력
            if (GameManager.instance.combo > 2)
            {
                GameManager.instance.score += ((int)Mathf.Pow(2, GameManager.instance.combo - 3) * 100);
                comboComponent.SetComboAnimation();
            }

            // 최대 콤보 저장
            GameManager.instance.highestCombo = Mathf.Max(GameManager.instance.highestCombo, GameManager.instance.combo);

            // 스테이지 모드일 경우 남은 라인 수가 0 이하면 다음 스테이지 진행
            if (GameManager.instance.mode == 2 && GameManager.instance.remainLines <= 0)
            {
                StartCoroutine(GameManager.instance.SetNextStage());
            }
        }
        else
        {
            // 콤보 카운트 초기화
            GameManager.instance.combo = 0;
        }
    }

    List<int> CheckFullLineCount(int fullLineIndex)
    {
        int count = 0;
        int checkLine = Mathf.Max(fullLineIndex + 4, boardHeight);
        List<int> clearLines = new List<int>();

        // 싱글, 더블, 트리플, 테트리스 확인 위해 지워진 라인부터 위 4줄 검사
        for (int i = fullLineIndex; i < checkLine; i++)
        {
            int calculateLine = 0;  // 한 라인에서 블럭이 채워졌는지 아닌지 계산용 변수

            // 한 라인에서 블럭이 채워졌으면 변수에 1 추가
            for (int j = 0; j < boardWidth; j++)
            {
                if (backgroundBlock[i * 10 + j].blockId == -1) break;
                else calculateLine++;
            }

            // 한줄이 다 채워졌으면 해당 번째 줄 리턴
            if (calculateLine == 10)
            {
                clearLines.Add(i);
                count++;
            }
        }

        // 스테이지 모드일 경우 남은 라인 수에서 지금 제거한 라인 수 차감
        if (GameManager.instance.mode == 2)
        {
            GameManager.instance.remainLines -= count;
            if (GameManager.instance.remainLines < 0) GameManager.instance.remainLines = 0;
        }

        // 일반 모드일 경우 제거한 라인 수 증가
        // 제거한 라인 수가 레벨업 기준을 넘었다면 레벨업 로직 수행
        else
        {
            GameManager.instance.line += count;
            if (GameManager.instance.level <= 10 && GameManager.instance.line >= GameManager.instance.levelStandard[GameManager.instance.level - 1])
            {
                GameManager.instance.level++;
                GameManager.instance.EffectPlay(GameManager.Effect.LEVELUP);
            }
            else if (GameManager.instance.level > 10)
            {
                if (GameManager.instance.line % 50 == 0) GameManager.instance.level++;
                GameManager.instance.EffectPlay(GameManager.Effect.LEVELUP);
            }
        }

        // 한번에 제거한 라인 수에 따른 점수 증가
        switch (count)
        {
            case 1:
                GameManager.instance.score += 100;
                break;
            case 2:
                GameManager.instance.score += 200;
                break;
            case 3:
                GameManager.instance.score += 400;
                break;
            case 4:
                GameManager.instance.score += 800;
                break;
        }

        return clearLines;
    }

    public void AddOneBlock()
    {
        // X축의 랜덤한 곳에 블럭을 올리는데
        // 가장 위에있는 곳에 올림
        bool isAddBlock = false;
        int randomX = Random.Range(0, 10);
        for(int i= boardHeight - 2; i>=0; i--)
        {
            if(backgroundBlock[i * 10 + randomX].blockId != -1)
            {
                backgroundBlock[10 + (i * 10) + randomX].blockId = 99;
                backgroundBlock[10 + (i * 10) + randomX].isLanded = true;
                isAddBlock = true;
                break;
            }
        }

        if(!isAddBlock)
        {
            backgroundBlock[randomX].blockId = 99;
            backgroundBlock[randomX].isLanded = true;
        }

        GameManager.instance.EffectPlay(GameManager.Effect.ADDBLOCK);
    }

    public void AddOneLine()
    {
        // 맨 윗줄 초기화
        for (int i = 0; i < boardWidth; i++)
        {
            backgroundBlock[190 + i].blockId = -1;
            backgroundBlock[190 + i].isLanded = false;
        }

        // 맨 윗줄을 제외한 라인들을 한줄 상승
        for (int j = boardHeight - 2; j >= 0; j--)
        {
            for (int k = 0; k < boardWidth; k++)
            {
                backgroundBlock[(j + 1) * 10 + k].blockId = backgroundBlock[j * 10 + k].blockId;
                backgroundBlock[(j + 1) * 10 + k].isLanded = backgroundBlock[j * 10 + k].isLanded;
            }
        }

        // 맨 아랫줄 초기화 및 라인 채우기
        for (int i = 0; i < boardWidth; i++)
        {
            backgroundBlock[i].blockId = -1;
            backgroundBlock[i].isLanded = false;
        }

        int randomX = Random.Range(0, 10);
        for (int i = 0; i < boardWidth; i++)
        {
            if(i != randomX)
            {
                backgroundBlock[i].blockId = 99;
                backgroundBlock[i].isLanded = true;
            }
        }

        GameManager.instance.EffectPlay(GameManager.Effect.ADDLINE);
    }
}
