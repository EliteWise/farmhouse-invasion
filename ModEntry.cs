using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley;
using StardewValley.Monsters;

namespace FarmhouseInvasion
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            this.SpawnMorningMonster();
        }

        private void SpawnMorningMonster()
        {
            if (!Context.IsMainPlayer)
            {
                this.Monitor.Log("Only the host can spawn the morning monster.", LogLevel.Debug);
                return;
            }

            Farm farm = Game1.getFarm();
            Building farmhouse = farm?.GetMainFarmHouse();
            if (farm == null || farmhouse == null)
            {
                this.Monitor.Log("Farmhouse not found.", LogLevel.Warn);
                return;
            }

            List<Vector2> spawnTiles = this.GetOpenTilesAroundBuilding(farm, farmhouse);
            if (spawnTiles.Count == 0)
            {
                this.Monitor.Log("There are no free tiles around the farmhouse.", LogLevel.Warn);
                return;
            }

            int monsterType = Game1.random.Next(6);
            int monstersToSpawn = Game1.random.Next(5, 11);
            int spawned = 0;

            for (int i = 0; i < monstersToSpawn && spawnTiles.Count > 0; i++)
            {
                int tileIndex = Game1.random.Next(spawnTiles.Count);
                Vector2 tile = spawnTiles[tileIndex];
                spawnTiles.RemoveAt(tileIndex);

                Monster monster = this.CreateSimpleMonster(monsterType, tile * Game1.tileSize);
                farm.characters.Add(monster);
                spawned++;
            }

        }

        private Monster CreateSimpleMonster(int monsterType, Vector2 position)
        {
            switch (monsterType)
            {
                case 0:
                    return new Bug(position, 0);
                case 1:
                    return new Fly(position);
                case 2:
                    return new Duggy(position);
                case 3:
                    return new Grub(position, false);
                case 4:
                    return new GreenSlime(position);
                case 5:
                    return new RockCrab(position);
                default:
                    return new RockCrab(position);
            }
        }

        private List<Vector2> GetOpenTilesAroundBuilding(GameLocation location, Building building)
        {
            List<Vector2> tiles = new List<Vector2>();
            int left = building.tileX.Value - 1;
            int right = building.tileX.Value + building.tilesWide.Value;
            int top = building.tileY.Value - 1;
            int bottom = building.tileY.Value + building.tilesHigh.Value;

            for (int x = left; x <= right; x++)
            {
                for (int y = top; y <= bottom; y++)
                {
                    bool touchesBuilding =
                        x == left ||
                        x == right ||
                        y == top ||
                        y == bottom;
                    if (!touchesBuilding)
                        continue;

                    Vector2 tile = new Vector2(x, y);
                    if (location.isTileLocationOpen(tile))
                        tiles.Add(tile);
                }
            }

            return tiles;
        }
    }
}
