using UnityEngine;

namespace JS.World.Map.Climate
{
    public static class AirPressureData
    {
        //So for air pressure, I also need to take into account sea level, so if max altitude is 1 and min is 0.4
            //then the only range needs to be between those two values
            //But that also still only takes into account altitude and nothing else
        public static float[,] GetAirPressureMap(float[,] heightMap)
        {
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);
            float[,] pressureMap = new float[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    pressureMap[x, y] = 1 - Mathf.Sqrt(heightMap[x, y]);
                }
            }
            return pressureMap;
        }

        public static Compass[,] GetWindMap(float[,] map)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);
            var equator = height / 2f;
            float windZoneLength = equator / 3f;
            Compass[,] windMap = new Compass[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float distanceFromEquator = Mathf.Abs(equator - y);
                    int windZone = Mathf.Clamp(Mathf.FloorToInt(distanceFromEquator / windZoneLength), 0, 2);

                    if (windZone == 1) //Secondary Direction is East
                    {
                        if (y > equator) windMap[x, y] = Compass.NorthEast;
                        else windMap[x, y] = Compass.SouthEast;
                    }
                    else //Secondary direction is West
                    {
                        if (y > equator) windMap[x, y] = Compass.SouthWest;
                        else windMap[x, y] = Compass.NorthWest;
                    }
                }
            }

            return windMap;
        }
    }
}