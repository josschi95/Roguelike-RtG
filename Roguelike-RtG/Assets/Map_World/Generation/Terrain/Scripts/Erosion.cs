using UnityEngine;

//Courtesy of Sebastian Lague
//https://www.youtube.com/watch?v=eaXk97ujbPQ

namespace JS.World.Map.Generation
{
    public class Erosion : MonoBehaviour
    {
        struct HeightAndGradient
        {
            public float height;
            public float gradientX;
            public float gradientY;
        }

        [SerializeField] private WorldData worldMap;

        [Range(2, 8)]
        [SerializeField] private int erosionRadius = 3;
        [Range(0, 1)]
        [SerializeField] private float inertia = .05f; // At zero, water will instantly change direction to flow downhill. At 1, water will never change direction. 
        [SerializeField] private float sedimentCapacityFactor = 3;// 4; // Multiplier for how much sediment a droplet can carry
        [SerializeField] private float minSedimentCapacity = .01f; // Used to prevent carry capacity getting too close to zero on flatter terrain
        [Range(0, 1)]
        [SerializeField] private float erodeSpeed = .3f;
        [Range(0, 1)]
        [SerializeField] private float depositSpeed = .3f;
        [Range(0, 1)]
        [SerializeField] private float evaporateSpeed = .01f;
        [SerializeField] private float gravity = 4;
        [SerializeField] private int maxDropletLifetime = 30;

        [SerializeField] private float initialWaterVolume = 1;
        [SerializeField] private float initialSpeed = 1;

        // Indices and weights of erosion brush precomputed for every node
        private int[][] erosionBrushIndices;
        private int[,] erosionBrushIndices_New;

        private float[][] erosionBrushWeights;
        private float[,] erosionBrushWeights_New;
        System.Random PRNG;

        private int currentSeed;
        private int currentErosionRadius;
        private int currentMapSize;

        // Initialization creates a System.Random object and precomputes indices and weights of erosion brush    
        private void Initialize(int mapSize, int newSeed)
        {
            if (PRNG == null || currentSeed != newSeed)
            {
                PRNG = new System.Random(newSeed);
                currentSeed = newSeed;
            }

            if (erosionBrushIndices == null || currentErosionRadius != erosionRadius || currentMapSize != mapSize)
            {
                InitializeBrushIndices(mapSize, erosionRadius);
                currentErosionRadius = erosionRadius;
                currentMapSize = mapSize;
            }
        }

        public float[,] Erode(float[,] heightMap, int numIterations, int seed)
        {
            var linearHeightMap = ArrayHelper.Convert2DFloatArrayTo1D(heightMap);
            Erode(linearHeightMap, heightMap.GetLength(0), numIterations, seed);
            heightMap = ArrayHelper.Convert1DFloatArrayTo2D(linearHeightMap, heightMap.GetLength(0), heightMap.GetLength (1));
            return heightMap;
        }

        private void Erode(float[] map, int mapSize, int numIterations, int newSeed)
        {
            Initialize(mapSize, newSeed);
            //int ranToWater = 0, ranToEdge = 0;
            for (int iteration = 0; iteration < numIterations; iteration++)
            {
                // Create water droplet at random point on map
                float posX = PRNG.Next(0, mapSize - 1);
                float posY = PRNG.Next(0, mapSize - 1);
                
                float dirX = 0;
                float dirY = 0;
                float speed = initialSpeed;
                float water = initialWaterVolume;
                float sediment = 0;

                for (int lifetime = 0; lifetime < maxDropletLifetime; lifetime++)
                {
                    int nodeX = (int)posX;
                    int nodeY = (int)posY;

                    int dropletIndex = nodeY * mapSize + nodeX;

                    // Calculate droplet's offset inside the cell (0,0) = at NW node, (1,1) = at SE node
                    float cellOffsetX = posX - nodeX;
                    float cellOffsetY = posY - nodeY;

                    //If the droplet reaches sea level, drop all of its sediment
                    if (map[dropletIndex] < WorldParameters.SEA_LEVEL)
                    {
                        //ranToWater++;
                        //sediment /= 4;
                        //map[dropletIndex] += sediment * (1 - cellOffsetX) * (1 - cellOffsetY);
                        //map[dropletIndex + 1] += sediment * cellOffsetX * (1 - cellOffsetY);
                        //map[dropletIndex + mapSize] += sediment * (1 - cellOffsetX) * cellOffsetY;
                        //map[dropletIndex + mapSize + 1] += sediment * cellOffsetX * cellOffsetY;
                        break;
                    }

                    // Calculate droplet's height and direction of flow with bilinear interpolation of surrounding heights
                    HeightAndGradient heightAndGradient = CalculateHeightAndGradient(map, mapSize, posX, posY);

                    // Update the droplet's direction and position (move position 1 unit regardless of speed)
                    dirX = (dirX * inertia - heightAndGradient.gradientX * (1 - inertia));
                    dirY = (dirY * inertia - heightAndGradient.gradientY * (1 - inertia));
                    // Normalize direction
                    float len = Mathf.Sqrt(dirX * dirX + dirY * dirY);
                    if (len != 0)
                    {
                        dirX /= len;
                        dirY /= len;
                    }
                    posX += dirX;
                    posY += dirY;

                    //Don't check if it's ran to water because I haven't set those values yet
                    // Stop simulating droplet if it's not moving or has flowed over edge of map
                    if ((dirX == 0 && dirY == 0) || posX < 0 || posX >= mapSize - 1 || posY < 0 || posY >= mapSize - 1)
                    {
                        //ranToEdge++;
                        break;
                    }
                    // Stop simulating droplet if it has reach a body of water
                    /*if (!worldMap.GetNode(nodeX, nodeY).IsLand)
                    {
                        ranToWater++;
                        if (lifetime <= 1) earlyLoss++;
                        break;
                    }*/

                    // Find the droplet's new height and calculate the deltaHeight
                    float newHeight = CalculateHeightAndGradient(map, mapSize, posX, posY).height;
                    float deltaHeight = newHeight - heightAndGradient.height;

                    // Calculate the droplet's sediment capacity (higher when moving fast down a slope and contains lots of water)
                    float sedimentCapacity = Mathf.Max(-deltaHeight * speed * water * sedimentCapacityFactor, minSedimentCapacity);

                    // If carrying more sediment than capacity, or if flowing uphill:
                    if (sediment > sedimentCapacity || deltaHeight > 0)
                    {
                        // If moving uphill (deltaHeight > 0) try fill up to the current height, otherwise deposit a fraction of the excess sediment
                        float amountToDeposit = (deltaHeight > 0) ? Mathf.Min(deltaHeight, sediment) : (sediment - sedimentCapacity) * depositSpeed;
                        sediment -= amountToDeposit;

                        // Add the sediment to the four nodes of the current cell using bilinear interpolation
                        // Deposition is not distributed over a radius (like erosion) so that it can fill small pits
                        map[dropletIndex] += amountToDeposit * (1 - cellOffsetX) * (1 - cellOffsetY);
                        map[dropletIndex + 1] += amountToDeposit * cellOffsetX * (1 - cellOffsetY);
                        map[dropletIndex + mapSize] += amountToDeposit * (1 - cellOffsetX) * cellOffsetY;
                        map[dropletIndex + mapSize + 1] += amountToDeposit * cellOffsetX * cellOffsetY;

                    }
                    else
                    {
                        // Erode a fraction of the droplet's current carry capacity.
                        // Clamp the erosion to the change in height so that it doesn't dig a hole in the terrain behind the droplet
                        float amountToErode = Mathf.Min((sedimentCapacity - sediment) * erodeSpeed, -deltaHeight);

                        // Use erosion brush to erode from all nodes inside the droplet's erosion radius
                        for (int brushPointIndex = 0; brushPointIndex < erosionBrushIndices[dropletIndex].Length; brushPointIndex++)
                        {
                            int nodeIndex = erosionBrushIndices[dropletIndex][brushPointIndex];
                            float weighedErodeAmount = amountToErode * erosionBrushWeights[dropletIndex][brushPointIndex];
                            float deltaSediment = (map[nodeIndex] < weighedErodeAmount) ? map[nodeIndex] : weighedErodeAmount;
                            map[nodeIndex] -= deltaSediment;
                            sediment += deltaSediment;
                        }
                    }

                    // Update droplet's speed and water content
                    speed = Mathf.Sqrt(speed * speed + deltaHeight * gravity);
                    water *= (1 - evaporateSpeed);
                }
            }

            //Debug.Log("Ran to Water: " + ranToWater);
            //Debug.Log("Ran to edge: " + ranToEdge);
        }

        private HeightAndGradient CalculateHeightAndGradient(float[] nodes, int mapSize, float posX, float posY)
        {
            int coordX = (int)posX;
            int coordY = (int)posY;

            // Calculate droplet's offset inside the cell (0,0) = at NW node, (1,1) = at SE node
            float x = posX - coordX;
            float y = posY - coordY;

            // Calculate heights of the four nodes of the droplet's cell
            int nodeIndexNW = coordY * mapSize + coordX;
            float heightNW = nodes[nodeIndexNW];
            float heightNE = nodes[nodeIndexNW + 1];
            float heightSW = nodes[nodeIndexNW + mapSize];
            float heightSE = nodes[nodeIndexNW + mapSize + 1];

            // Calculate droplet's direction of flow with bilinear interpolation of height difference along the edges
            float gradientX = (heightNE - heightNW) * (1 - y) + (heightSE - heightSW) * y;
            float gradientY = (heightSW - heightNW) * (1 - x) + (heightSE - heightNE) * x;

            // Calculate height with bilinear interpolation of the heights of the nodes of the cell
            float height = heightNW * (1 - x) * (1 - y) + heightNE * x * (1 - y) + heightSW * (1 - x) * y + heightSE * x * y;

            return new HeightAndGradient() { height = height, gradientX = gradientX, gradientY = gradientY };
        }

        private void InitializeBrushIndices(int mapSize, int radius)
        {
            erosionBrushIndices = new int[mapSize * mapSize][];
            erosionBrushWeights = new float[mapSize * mapSize][];

            int[] xOffsets = new int[radius * radius * 4];
            int[] yOffsets = new int[radius * radius * 4];
            float[] weights = new float[radius * radius * 4];
            float weightSum = 0;
            int addIndex = 0;

            for (int i = 0; i < erosionBrushIndices.GetLength(0); i++)
            {
                int centreX = i % mapSize;
                int centreY = i / mapSize;

                if (centreY <= radius || centreY >= mapSize - radius || centreX <= radius + 1 || centreX >= mapSize - radius)
                {
                    weightSum = 0;
                    addIndex = 0;
                    for (int y = -radius; y <= radius; y++)
                    {
                        for (int x = -radius; x <= radius; x++)
                        {
                            float sqrDst = x * x + y * y;
                            if (sqrDst < radius * radius)
                            {
                                int coordX = centreX + x;
                                int coordY = centreY + y;

                                if (coordX >= 0 && coordX < mapSize && coordY >= 0 && coordY < mapSize)
                                {
                                    float weight = 1 - Mathf.Sqrt(sqrDst) / radius;
                                    weightSum += weight;
                                    weights[addIndex] = weight;
                                    xOffsets[addIndex] = x;
                                    yOffsets[addIndex] = y;
                                    addIndex++;
                                }
                            }
                        }
                    }
                }

                int numEntries = addIndex;
                erosionBrushIndices[i] = new int[numEntries];
                erosionBrushWeights[i] = new float[numEntries];

                for (int j = 0; j < numEntries; j++)
                {
                    erosionBrushIndices[i][j] = (yOffsets[j] + centreY) * mapSize + xOffsets[j] + centreX;
                    erosionBrushWeights[i][j] = weights[j] / weightSum;
                }
            }
        }

        #region - Attempted Rework - 
        
        //This is good to go
        private void Initialize_2D(int sideLength, int newSeed)
        {
            if (PRNG == null || currentSeed != newSeed)
            {
                PRNG = new System.Random(newSeed);
                currentSeed = newSeed;
            }

            if (erosionBrushIndices == null || currentErosionRadius != erosionRadius || currentMapSize != sideLength)
            {
                InitializeBrushIndices_2D(sideLength, erosionRadius);
                currentErosionRadius = erosionRadius;
                currentMapSize = sideLength;
            }
        }

        public float[,] Erode_2D(float[,] heightMap, int numIterations, int seed)
        {
            int mapSize = heightMap.GetLength(0);
            Initialize_2D(mapSize, seed);

            for (int iteration = 0; iteration < numIterations; iteration++)
            {
                // Create water droplet at random point on map
                float posX = PRNG.Next(0, mapSize - 1);
                float posY = PRNG.Next(0, mapSize - 1);
                float dirX = 0;
                float dirY = 0;
                float speed = initialSpeed;
                float water = initialWaterVolume;
                float sediment = 0;

                for (int lifetime = 0; lifetime < maxDropletLifetime; lifetime++)
                {
                    int nodeX = (int)posX;
                    int nodeY = (int)posY;
                    //int dropletIndex = nodeY * mapSize + nodeX;
                    // Calculate droplet's offset inside the cell (0,0) = at NW node, (1,1) = at SE node
                    float cellOffsetX = posX - nodeX; //0 = West, 1 = East
                    float cellOffsetY = posY - nodeY; //0 = North, 1 = South

                    // Calculate droplet's height and direction of flow with bilinear interpolation of surrounding heights
                    HeightAndGradient heightAndGradient = CalculateHeightAndGradient(heightMap, posX, posY);

                    // Update the droplet's direction and position (move position 1 unit regardless of speed)
                    dirX = (dirX * inertia - heightAndGradient.gradientX * (1 - inertia));
                    dirY = (dirY * inertia - heightAndGradient.gradientY * (1 - inertia));

                    // Normalize direction
                    float len = Mathf.Sqrt(dirX * dirX + dirY * dirY);
                    if (len != 0)
                    {
                        dirX /= len;
                        dirY /= len;
                    }
                    posX += dirX;
                    posY += dirY;

                    // Stop simulating droplet if it's not moving or has flowed over edge of map
                    if ((dirX == 0 && dirY == 0) || posX < 0 || posX >= mapSize - 1 || posY < 0 || posY >= mapSize - 1)
                    {
                        break;
                    }

                    // Find the droplet's new height and calculate the deltaHeight
                    float newHeight = CalculateHeightAndGradient(heightMap, posX, posY).height;
                    float deltaHeight = newHeight - heightAndGradient.height;

                    // Calculate the droplet's sediment capacity (higher when moving fast down a slope and contains lots of water)
                    float sedimentCapacity = Mathf.Max(-deltaHeight * speed * water * sedimentCapacityFactor, minSedimentCapacity);

                    // If carrying more sediment than capacity, or if flowing uphill:
                    if (sediment > sedimentCapacity || deltaHeight > 0)
                    {
                        // If moving uphill (deltaHeight > 0) try fill up to the current height, otherwise deposit a fraction of the excess sediment
                        float amountToDeposit = (deltaHeight > 0) ? Mathf.Min(deltaHeight, sediment) : (sediment - sedimentCapacity) * depositSpeed;
                        sediment -= amountToDeposit;

                        // Add the sediment to the four nodes of the current cell using bilinear interpolation
                        // Deposition is not distributed over a radius (like erosion) so that it can fill small pits
                        heightMap[nodeX - 1, nodeY + 1] += amountToDeposit * (1 - cellOffsetX) * (1 - cellOffsetY); //NorthWest                                         //May need to review these orders
                        heightMap[nodeX + 1, nodeY + 1] += amountToDeposit * cellOffsetX * (1 - cellOffsetY);       //NorthEast
                        heightMap[nodeX - 1, nodeY - 1] += amountToDeposit * (1 - cellOffsetX) * cellOffsetY;       //SouthEast
                        heightMap[nodeX + 1, nodeY - 1] += amountToDeposit * cellOffsetX * cellOffsetY;             //SouthWest
                    }
                    else
                    {
                        // Erode a fraction of the droplet's current carry capacity.
                        // Clamp the erosion to the change in height so that it doesn't dig a hole in the terrain behind the droplet
                        float amountToErode = Mathf.Min((sedimentCapacity - sediment) * erodeSpeed, -deltaHeight);

                        // Use erosion brush to erode from all nodes inside the droplet's erosion radius
                        for (int brushPointIndex = 0; brushPointIndex < erosionBrushIndices_New[nodeX, nodeY]; brushPointIndex++)
                        {
                            float weighedErodeAmount = amountToErode * erosionBrushIndices_New[nodeX, nodeY];
                            float deltaSediment = (heightMap[nodeX, nodeY] < weighedErodeAmount) ? heightMap[nodeX, nodeY] : weighedErodeAmount;
                            heightMap[nodeX, nodeY] -= deltaSediment;
                            sediment += deltaSediment;
                        }
                    }

                    // Update droplet's speed and water content
                    speed = Mathf.Sqrt(speed * speed + deltaHeight * gravity);
                    water *= (1 - evaporateSpeed);
                }
            }

            return heightMap;
        }

        public void ErodeTest(float[,] heightMap, int numIterations, int seed)
        {
            Initialize_2D(heightMap.GetLength(0), seed);

            for (int iterations = 0; iterations < numIterations; iterations++)
            {
                //Create a water droplet at random point on map
                float posX = PRNG.Next(0, heightMap.GetLength(0));
                float posY = PRNG.Next(1, heightMap.GetLength(1));
                float dirX = 0; //lateral movement of the droplet
                float dirY = 0; //longitudinal movement of the droplet
                float speed = initialSpeed;
                float water = initialWaterVolume;
                float sediment = 0;


                for (int lifetime = 0; lifetime < maxDropletLifetime; lifetime++)
                {
                    int nodeX = (int)posX; //get the current nodal position of the droplet
                    int nodeY = (int)posY;

                    // Calculate droplet's offset inside the cell (0,0) = at NW node, (1,1) = at SE node
                    float cellOffsetX = posX - nodeX;
                    float cellOffsetY = posY - nodeY;

                    // Calculate droplet's height and direction of flow with bilinear interpolation of surrounding heights
                    //Evaluates the heights of the surrounding nodes and pushes droplet towards the lowest node
                    HeightAndGradient heightAndGradient = CalculateHeightAndGradient(heightMap, posX, posY);

                    //Update the droplet's direction and position (move position 1 unit regardless of speed)
                    dirX = (dirX * inertia - heightAndGradient.gradientX * (1 - inertia));
                    dirY = (dirY * inertia - heightAndGradient.gradientY * (1 - inertia));

                    //Normalize direction
                    float len = Mathf.Sqrt(dirX * dirX + dirY * dirY);
                    if (len != 0)
                    {
                        dirX /= len;
                        dirY /= len;
                    }
                    posX += dirX;
                    posY += dirY;

                    if (dirX == 0 && dirY == 0) break; // Stop simulating droplet if it's not moving

                    //The droplet has ran off the edge of the map
                    if (posX < 0 || posX >= heightMap.GetLength(0) - 1 || posY < 0 || posY >= heightMap.GetLength(1) - 1) break;

                    // Stop simulating droplet if it has reach a body of water
                    if (!worldMap.GetNode(nodeX, nodeY).IsLand) break;

                    // Find the droplet's new height and calculate the deltaHeight
                    float newHeight = CalculateHeightAndGradient(heightMap, posX, posY).height;
                    float deltaHeight = newHeight - heightAndGradient.height;

                    // Calculate the droplet's sediment capacity (higher when moving fast down a slope and contains lots of water)
                    float sedimentCapacity = Mathf.Max(-deltaHeight * speed * water * sedimentCapacityFactor, minSedimentCapacity);

                    // If carrying more sediment than capacity, or if flowing uphill:
                    if (sediment > sedimentCapacity || deltaHeight > 0)
                    {
                        // If moving uphill (deltaHeight > 0) try fill up to the current height, otherwise deposit a fraction of the excess sediment
                        float amountToDeposit = (deltaHeight > 0) ? Mathf.Min(deltaHeight, sediment) : (sediment - sedimentCapacity) * depositSpeed;
                        sediment -= amountToDeposit;

                        // Add the sediment to the four nodes of the current cell using bilinear interpolation
                        // Deposition is not distributed over a radius (like erosion) so that it can fill small pits
                        heightMap[nodeX - 1, nodeY + 1] += amountToDeposit * (1 - cellOffsetX) * (1 - cellOffsetY); //NW
                        heightMap[nodeX + 1, nodeY + 1] += amountToDeposit * cellOffsetX * (1 - cellOffsetY); //NE
                        heightMap[nodeX - 1, nodeY - 1] += amountToDeposit * (1 - cellOffsetX) * cellOffsetY; //SW
                        heightMap[nodeX + 1, nodeY - 1] += amountToDeposit * cellOffsetX * cellOffsetY; //SE
                    }
                    else
                    {
                        // Erode a fraction of the droplet's current carry capacity.
                        // Clamp the erosion to the change in height so that it doesn't dig a hole in the terrain behind the droplet
                        float amountToErode = Mathf.Min((sedimentCapacity - sediment) * erodeSpeed, -deltaHeight);

                        // Use erosion brush to erode from all nodes inside the droplet's erosion radius
                        for (int x = nodeX - erosionRadius; x < nodeX + erosionRadius; x++)
                        {
                            for (int y = nodeY - erosionRadius; y < nodeY + erosionRadius; y++)
                            {
                                if (x < 0 || x > heightMap.GetLength(0) - 1) continue;
                                if (y < 0 || y > heightMap.GetLength(1) - 1) continue;

                                //weight decreases with distance from raindrop center
                                float weight = 1 - (GridMath.GetStraightDist(nodeX, nodeY, x, y) / erosionRadius);
                                float weightedErodeAmount = amountToErode * weight;
                                float deltaSediment = (heightMap[nodeX, nodeY] < weightedErodeAmount) ? heightMap[x, y] : weightedErodeAmount;
                                heightMap[x, y] -= deltaSediment;
                                sediment += deltaSediment;
                            }
                        }
                    }

                    // Update droplet's speed and water content
                    speed = Mathf.Sqrt(speed * speed + deltaHeight * gravity);
                    water *= (1 - evaporateSpeed);
                }
            }
        }

        //This one is good to go
        private HeightAndGradient CalculateHeightAndGradient(float[,] heightMap, float posX, float posY)
        {
            int coordX = (int)posX;
            int coordY = (int)posY;

            // Calculate droplet's offset inside the cell (0,0) = at NW node, (1,1) = at SE node
            float x = posX - coordX;
            float y = posY - coordY;

            // Calculate heights of the four nodes adjacent to the droplet's cell
            float heightNW = heightMap[coordX - 1, coordY + 1];
            float heightNE = heightMap[coordX + 1, coordY + 1];
            float heightSW = heightMap[coordX - 1, coordY - 1];
            float heightSE = heightMap[coordX + 1, coordY - 1];

            // Calculate droplet's direction of flow with bilinear interpolation of height difference along the edges
            float gradientX = (heightNE - heightNW) * (1 - y) + (heightSE - heightSW) * y;
            float gradientY = (heightSW - heightNW) * (1 - x) + (heightSE - heightNE) * x;

            // Calculate height with bilinear interpolation of the heights of the nodes of the cell
            float height = heightNW * (1 - x) * (1 - y) + heightNE * x * (1 - y) + heightSW * (1 - x) * y + heightSE * x * y;

            return new HeightAndGradient() { height = height, gradientX = gradientX, gradientY = gradientY };
        }

        //This one definitely needs to be reevaluated
        private void InitializeBrushIndices_2D(int sideLength, int radius)
        {
            erosionBrushIndices_New = new int[sideLength, sideLength];
            erosionBrushWeights_New = new float[sideLength, sideLength];

            int[] xOffsets = new int[radius * radius * 4];
            int[] yOffsets = new int[radius * radius * 4];
            float[] weights = new float[radius * radius * 4];
            float weightSum = 0;
            int addIndex = 0;

            for (int i = 0; i < erosionBrushIndices_New.GetLength(0); i++)
            {
                int centreX = i % sideLength;
                int centreY = i / sideLength;

                if (centreY <= radius || centreY >= sideLength - radius || centreX <= radius + 1 || centreX >= sideLength - radius)
                {
                    weightSum = 0;
                    addIndex = 0;
                    for (int y = -radius; y <= radius; y++)
                    {
                        for (int x = -radius; x <= radius; x++)
                        {
                            float sqrDst = x * x + y * y;
                            if (sqrDst < radius * radius)
                            {
                                int coordX = centreX + x;
                                int coordY = centreY + y;

                                if (coordX >= 0 && coordX < sideLength && coordY >= 0 && coordY < sideLength)
                                {
                                    float weight = 1 - Mathf.Sqrt(sqrDst) / radius;
                                    weightSum += weight;
                                    weights[addIndex] = weight;
                                    xOffsets[addIndex] = x;
                                    yOffsets[addIndex] = y;
                                    addIndex++;
                                }
                            }
                        }
                    }
                }

                int numEntries = addIndex;
                //erosionBrushIndices[i] = new int[numEntries];
                //erosionBrushWeights[i] = new float[numEntries];

                for (int j = 0; j < numEntries; j++)
                {
                    erosionBrushIndices_New[i, j] = (yOffsets[j] + centreY) * sideLength + xOffsets[j] + centreX;
                    erosionBrushWeights_New[i, j] = weights[j] / weightSum;
                }
            }

        }
        
        #endregion
    }
}