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
        // 0: ���� ���� ��, 1: �ؽ�Ʈ ��
        tetIndex = new int[2];
        tetIndex[0] = Random.Range(0, 7);
        tetIndex[1] = Random.Range(0, 7);
    }

    void Start()
    {
        // ��� �� ����
        CreateBoardBackground();
    }

    void FixedUpdate()
    {
        // ���� ��ư ������ ���� ���� ����
        if (isDropping)
        {
            DropTetromino();
        }
    }

    void Update()
    {
        if (!GameManager.instance.isLoading && GameManager.instance.isGamePlaying)
        {
            // �ٴڿ� �������ų�, �ٸ� �� ���� ������
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

        // x��ǥ�� -5 ~ 5, y��ǥ�� -10 ~ 10)
        for (int i = 0; i < boardHeight; i++)
        {
            for (int j = 0; j < boardWidth; j++)
            {
                backgroundVector[i, j] = new Vector2(j- halfWidth, i- halfHeight);
                blockPrefab.transform.localPosition = backgroundVector[i,j] + new Vector2(0.5f, 0.5f);
                blockPrefab.transform.localScale = new Vector3(1, 1, 1);

                // ù ���� �� ����� ������ ����
                // ����� �� ����� �̹� �����ϸ� ������ �ʱ�ȭ
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
        // ��ġ�� ȸ�� �ʱ�ȭ
        tetromino.localPosition = new Vector3(0, 9, 0);
        tetromino.rotation = Quaternion.identity;

        // Ȱ��ȭ�Ǿ��� ���� �� ��Ȱ��ȭ
        spawnTetromino = tetromino.GetComponentsInChildren<Block>(true);
        foreach (Block block in spawnTetromino)
        {
            if (block.gameObject.activeSelf) block.gameObject.SetActive(false);
        }
    }

    public void InitTetromino()
    {
        ResetTetromino();

        // ���� �� ����
        CreateTetromino(tetIndex[0]);
    }

    public void CreateTetromino(int tetId)
    {
        Vector2[] positionVector = new Vector2[4];

        // 0: �ϴû�, ��
        // 1: �Ķ���, ��
        // 2: ��Ȳ��, ��(�ݴ�)
        // 3: �����, ��
        // 4: �ʷϻ�, ��+��
        // 5: �����, ��
        // 6: ������, ��+��
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
        // ���� �� ����
        for (int i = 0; i < spawnTetromino.Length; i++)
        {
            spawnTetromino[i].gameObject.SetActive(true);

            spawnTetromino[i].blockId = blockId;
            spawnTetromino[i].transform.localPosition = position[i] + new Vector2(0.5f, 0.5f);

            // ������ ��Ʈ�ι̳뿡 �̹� �׿��ִ°� �ִٸ� ���ӿ���
            if (CheckSpecialBlockIsCollide(spawnTetromino[i], Vector3.down, 0.01f))
            {
                GameManager.instance.GameOver();
                break;
            }
        }

        // �ؽ�Ʈ �� ����
        gameUI.SetNextBlock(tetIndex[1]);

        // �� ���� �Ŀ��� �ؽ�Ʈ ���� ���� ������ ����
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

    // ��Ʈ�ι̳� ���� ������ �¿�� �����̸� �հ� ���� ���� ��ġ��
    // ���� ������ ���� �浹�� �ƴ� ����ĳ��Ʈ �Ÿ��� ����
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
        // ȸ�� �� ������: ȸ�� �� �����¿��� ��ֹ�, ��Ʈ�ι̳� ���
        // ���� ��翡�� ȸ���ϸ�? �ٵ� �� ����� �̰Ŷ��? ȸ�������� �� ������ ���� �ִٸ�?
        float rotateAngle = tetromino.rotation.eulerAngles.z;
        if (rotateAngle == 0)   // �������̹Ƿ� ���� ȸ���ϸ� 90���� �ȴ�
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

        // ���� �߿� ������ �ϸ� �ٷ� ���� ���� �����ϵ��� ����
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

        // ���� �ð� ���ݸ��� ��ĭ�� ����
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

        // ���� �ð��� �Ǹ� ���� ���� ����
        if (landingTimer > GameManager.instance.landingInterval)
        {
            foreach (Block block in spawnTetromino)
            {
                // ���� ���� ��ġ�� (0, -0.7) �̰�, ���� �߽� ��ġ�� (0.5, 0.5) ���� -> �׷��� ������ ��ġ�� (0.5, -0.2)�� �Ǿ�����
                // -> ��ġ ���̸�ŭ �������ְ�, ������ ù ������ ���ϴܺ��� �����̱� ������ ���ο� ������ ���� �ݸ�ŭ ���������μ� ����
                Vector3 mapPosition = block.transform.position + new Vector3(-0.5f, 0.2f, 0) + new Vector3(5f, 10f, 0);

                // �� �ױ� �Ϸ� �ߴٴ� ǥ��
                block.isLanded = true;

                // ��� ���� ���� ������ �����Ͽ� ���ΰ� ó�� ���̵��� ����
                backgroundBlock[Mathf.RoundToInt(mapPosition.y) * 10 + Mathf.RoundToInt(mapPosition.x)].blockId = block.blockId;
                backgroundBlock[Mathf.RoundToInt(mapPosition.y) * 10 + Mathf.RoundToInt(mapPosition.x)].isLanded = true;
            }

            // ������ ��Ʈ�ι̳� �� �߰�
            GameManager.instance.blockCount++;

            // ���� �� �������� �� ������ ������ ����
            LineClear();

            // �븻��忡�� ������ 10 �ʰ��̰ų�, �ϵ����̸� �� �߰� ��� Ȱ��ȭ
            if((GameManager.instance.mode == 0 && GameManager.instance.level > 10) || GameManager.instance.mode == 1)
            {
                if(GameManager.instance.blockCount % 5 == 0)    AddOneBlock();
                if(GameManager.instance.blockCount % 10 == 0)   AddOneLine();
            }

            // ����������忡�� Ư�� ���������� ��� �� �߰� ��� Ȱ��ȭ
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

            // ���� Ÿ�̸� �ʱ�ȭ
            isLanding = false;
            landingTimer = 0f;

            // ���� �� ������
            InitTetromino();
        }
    }

    void DropTetromino()
    {
        if (spawnTetromino[0].isLanded) return;

        // ���� ������ �����ϴ� �Ÿ� ��� �� ���� ���� ������ �ؼ� �� �̵�
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

        // �̵� �� ȿ���� ���, ���� ����
        tetromino.position = new Vector3(tetromino.position.x, tetromino.position.y - distance, tetromino.position.z);
        GameManager.instance.EffectPlay(GameManager.Effect.DROP);
        GameManager.instance.score += Mathf.RoundToInt(distance) * 10;

        isDropping = false;
        isLanding = true;
    }

    float CheckLandingPosition(Block block)
    {
        Vector3 start = block.transform.position;

        // RayCast: ������Ʈ �˻��� ���� Ray�� ��� ���
        // Debug.DrawRay(): ������ �󿡼��� Ray�� �׷��ִ� �Լ�
        // RayCastHit: Ray�� ���� ������Ʈ
        // collider: ray�� ��ǥ ������ �浹�� ������Ʈ
        // distance: ray�� ��ǥ������ ����� ���� �Ÿ�

        //Debug.DrawRay(start, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(start, Vector3.down, boardHeight, LayerMask.GetMask("Border"));

        // ���ó� �߽��� 0.5�̱� ������ ���� �Ÿ����� 0.5�� �� ���� ����
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
            int calculateLine = 0;  // �� ���ο��� ���� ä�������� �ƴ��� ���� ����

            // �� ���ο��� ���� ä�������� ������ 1 �߰�
            for (int j = 0; j < boardWidth; j++)
            {
                if (backgroundBlock[i * 10 + j].blockId == -1) break;
                else calculateLine++;
            }
            
            // ������ �� ä�������� �ش� ��° �� ����
            if(calculateLine == 10)
            {
                return i;
            }
        }
        return -1;
    }

    void LineClear()
    {
        // ���⼭ ���� ������ �ϼ��Ǿ����� ���� ����� �����ִ°� �Ʒ��� ������� ��� ���� ����
        int fullLineIndex = findFullLine();

        if (fullLineIndex > -1)
        {
            List<int> clearLines = new List<int>();

            // ���� ���� �� �� ���� �߰�
            clearLines = CheckFullLineCount(fullLineIndex);

            while(clearLines.Count > 0)
            {
                // ��� ���ִ� ���� ����
                //for (int i = 0; i < boardWidth; i++)
                //{
                //    Destroy(backgroundBlock[fullLineIndex * 10 + i].gameObject);
                //}

                // ������ ���� ������ ������
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

                // �� ���� ���� ����
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

            // ���� ���
            GameManager.instance.EffectPlay(GameManager.Effect.LINECLEAR);

            // �޺� ī��Ʈ �߰�
            GameManager.instance.combo++;

            // 3�޺� �̻��̸� ���� �߰� �� ȭ�鿡 �޺� �ִϸ��̼� ���
            if (GameManager.instance.combo > 2)
            {
                GameManager.instance.score += ((int)Mathf.Pow(2, GameManager.instance.combo - 3) * 100);
                comboComponent.SetComboAnimation();
            }

            // �ִ� �޺� ����
            GameManager.instance.highestCombo = Mathf.Max(GameManager.instance.highestCombo, GameManager.instance.combo);

            // �������� ����� ��� ���� ���� ���� 0 ���ϸ� ���� �������� ����
            if (GameManager.instance.mode == 2 && GameManager.instance.remainLines <= 0)
            {
                StartCoroutine(GameManager.instance.SetNextStage());
            }
        }
        else
        {
            // �޺� ī��Ʈ �ʱ�ȭ
            GameManager.instance.combo = 0;
        }
    }

    List<int> CheckFullLineCount(int fullLineIndex)
    {
        int count = 0;
        int checkLine = Mathf.Max(fullLineIndex + 4, boardHeight);
        List<int> clearLines = new List<int>();

        // �̱�, ����, Ʈ����, ��Ʈ���� Ȯ�� ���� ������ ���κ��� �� 4�� �˻�
        for (int i = fullLineIndex; i < checkLine; i++)
        {
            int calculateLine = 0;  // �� ���ο��� ���� ä�������� �ƴ��� ���� ����

            // �� ���ο��� ���� ä�������� ������ 1 �߰�
            for (int j = 0; j < boardWidth; j++)
            {
                if (backgroundBlock[i * 10 + j].blockId == -1) break;
                else calculateLine++;
            }

            // ������ �� ä�������� �ش� ��° �� ����
            if (calculateLine == 10)
            {
                clearLines.Add(i);
                count++;
            }
        }

        // �������� ����� ��� ���� ���� ������ ���� ������ ���� �� ����
        if (GameManager.instance.mode == 2)
        {
            GameManager.instance.remainLines -= count;
            if (GameManager.instance.remainLines < 0) GameManager.instance.remainLines = 0;
        }

        // �Ϲ� ����� ��� ������ ���� �� ����
        // ������ ���� ���� ������ ������ �Ѿ��ٸ� ������ ���� ����
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

        // �ѹ��� ������ ���� ���� ���� ���� ����
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
        // X���� ������ ���� ���� �ø��µ�
        // ���� �����ִ� ���� �ø�
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
        // �� ���� �ʱ�ȭ
        for (int i = 0; i < boardWidth; i++)
        {
            backgroundBlock[190 + i].blockId = -1;
            backgroundBlock[190 + i].isLanded = false;
        }

        // �� ������ ������ ���ε��� ���� ���
        for (int j = boardHeight - 2; j >= 0; j--)
        {
            for (int k = 0; k < boardWidth; k++)
            {
                backgroundBlock[(j + 1) * 10 + k].blockId = backgroundBlock[j * 10 + k].blockId;
                backgroundBlock[(j + 1) * 10 + k].isLanded = backgroundBlock[j * 10 + k].isLanded;
            }
        }

        // �� �Ʒ��� �ʱ�ȭ �� ���� ä���
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
