using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
public enum DungeonType
{
    DUNGEON,
    FOREST
}
public class DungeonGenerator : MonoBehaviour
{
    public bool isDay = false;
    
    public DungeonType dungeonType = DungeonType.DUNGEON;
    public GameObject dayDirectionalLight;
    public GameObject dungeonDirectionalLight;
    public class Cell
    {
        public bool visited = false;
        public bool[] status = new bool[4];
    }

    [System.Serializable]
    public class Rule
    {
        public GameObject room;
        public Vector2Int minPosition;
        public Vector2Int maxPosition;

        public bool obligatory;
        public int spawnChance = 100;

        public int ProbabilityOfSpawning(int x, int y)
        {
            // 0 - cannot spawn 1 - can spawn 2 - HAS to spawn

            if (x >= minPosition.x && x <= maxPosition.x && y >= minPosition.y && y <= maxPosition.y)
            {
                return obligatory ? 2 : 1;
            }

            return 0;
        }

    }

    public Vector2Int size;
    public int startPos = 0;
    public Rule[] roomsDungeon;
    public Rule[]roomsForest;
    public Vector2 offset;

    public int numberOfTreasureChests = 1;
    [Header("Special rooms")]
    public GameObject lastRoomPrefab;
    public GameObject treasureRoomPrefab;


    List<Cell> board;
    public static DungeonGenerator Instance { get; private set; }


    void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject); 
                return;
            }
            Instance = this;
        }
    // Start is called before the first frame update
    void Start()
    {
        if(GameManager.Instance != null)
        {
            if (GameManager.Instance.GetDungeonSize()!= Vector2Int.zero)
            {
                this.size = GameManager.Instance.GetDungeonSize();
            }
        }
        if(GameManager.Instance != null)
        {
            this.numberOfTreasureChests = GameManager.Instance.selectedNumberOfTreasureChests;
        }
        MazeGenerator();
         NavMeshSurface surface = GetComponent<NavMeshSurface>();
        if (surface != null)
        {
            surface.BuildNavMesh();
        }
        
    }

    void GenerateDungeon()
    {
        Rule[] rooms ;
        dungeonType = GameManager.Instance.dungeonType;
        if (dungeonType == DungeonType.FOREST)
        {
            rooms = roomsForest;
            dayDirectionalLight.SetActive(true);
            dungeonDirectionalLight.SetActive(false);
        }
        else
        {
            rooms = roomsDungeon;
            dayDirectionalLight.SetActive(false);
            dungeonDirectionalLight.SetActive(true);
        }
    int numberOfTreasureChestsSpawned=0;
    for (int i = 0; i < size.x; i++)
    {
        for (int j = 0; j < size.y; j++)
        {
            int index = i + j * size.x;
            Cell currentCell = board[index];

            if (!currentCell.visited)
                continue;

            GameObject roomToSpawn = null;

            // 🔥 OSTATNI POKÓJ
            if (index == board.Count - 1 && lastRoomPrefab != null)
            {
                roomToSpawn = lastRoomPrefab;
            }
            //  PRZEDOSTATNI POKÓJ
            else if (((i == size.x - 1 && j == size.y-2 )||((i == size.x-2 && j == size.y-1 )))&&treasureRoomPrefab != null && numberOfTreasureChestsSpawned==0)
            {
                roomToSpawn = treasureRoomPrefab;
                numberOfTreasureChestsSpawned++;
            }
            else
            {
                int randomRoom = -1;
                List<int> availableRooms = new List<int>();

                for (int k = 0; k < rooms.Length; k++)
                {
                    int p = rooms[k].ProbabilityOfSpawning(i, j);

                    if (p == 2)
                    {
                        randomRoom = k;
                        break;
                    }
                    else if (p == 1)
                    {
                        availableRooms.Add(k);
                    }
                }

                if (randomRoom == -1)
                {
                    if (availableRooms.Count > 0)
                    {
                        int totalWeight = 0;
                        foreach (int idx in availableRooms)
                            totalWeight += rooms[idx].spawnChance;

                        int rand = Random.Range(0, totalWeight);
                        int cumulative = 0;

                        foreach (int idx in availableRooms)
                        {
                            cumulative += rooms[idx].spawnChance;
                            if (rand < cumulative)
                            {
                                randomRoom = idx;
                                break;
                            }
                        }
                    }
                    else
                    {
                        randomRoom = 0;
                    }
                }

                roomToSpawn = rooms[randomRoom].room;
            }

            var newRoom = Instantiate(
                roomToSpawn,
                new Vector3(i * offset.x, 0, -j * offset.y),
                Quaternion.identity,
                transform
            ).GetComponent<RoomBehaviour>();

            newRoom.UpdateRoom(currentCell.status);
            newRoom.name += $" {i}-{j}";
        }
    }
}

    

    void MazeGenerator()
    {
        board = new List<Cell>();

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                board.Add(new Cell());
            }
        }

        int currentCell = startPos;

        Stack<int> path = new Stack<int>();

        int k = 0;

        while (k<1000)
        {
            k++;

            board[currentCell].visited = true;

            if(currentCell == board.Count - 1)
            {
                break;
            }

            //Check the cell's neighbors
            List<int> neighbors = CheckNeighbors(currentCell);

            if (neighbors.Count == 0)
            {
                if (path.Count == 0)
                {
                    break;
                }
                else
                {
                    currentCell = path.Pop();
                }
            }
            else
            {
                path.Push(currentCell);

                int newCell = neighbors[Random.Range(0, neighbors.Count)];

                if (newCell > currentCell)
                {
                    //down or right
                    if (newCell - 1 == currentCell)
                    {
                        board[currentCell].status[2] = true;
                        currentCell = newCell;
                        board[currentCell].status[3] = true;
                    }
                    else
                    {
                        board[currentCell].status[1] = true;
                        currentCell = newCell;
                        board[currentCell].status[0] = true;
                    }
                }
                else
                {
                    //up or left
                    if (newCell + 1 == currentCell)
                    {
                        board[currentCell].status[3] = true;
                        currentCell = newCell;
                        board[currentCell].status[2] = true;
                    }
                    else
                    {
                        board[currentCell].status[0] = true;
                        currentCell = newCell;
                        board[currentCell].status[1] = true;
                    }
                }

            }

        }
        GenerateDungeon();
    }

    List<int> CheckNeighbors(int cell)
    {
        List<int> neighbors = new List<int>();

        //check up neighbor
        if (cell - size.x >= 0 && !board[(cell-size.x)].visited)
        {
            neighbors.Add((cell - size.x));
        }

        //check down neighbor
        if (cell + size.x < board.Count && !board[(cell + size.x)].visited)
        {
            neighbors.Add((cell + size.x));
        }

        //check right neighbor
        if ((cell+1) % size.x != 0 && !board[(cell +1)].visited)
        {
            neighbors.Add((cell +1));
        }

        //check left neighbor
        if (cell % size.x != 0 && !board[(cell - 1)].visited)
        {
            neighbors.Add((cell -1));
        }

        return neighbors;
    }
}
