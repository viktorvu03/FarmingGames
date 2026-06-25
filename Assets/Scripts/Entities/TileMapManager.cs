using System;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapManager : MonoBehaviour
{
   public Tilemap tm_Ground;
   public Tilemap tm_Grass;
   public Tilemap tm_Forest;
   public Tilemap tm_Sprout;
   public Tilemap tm_House;
   
   public TileBase tb_Forest;
   public TileBase tb_Sprout;
   public TileBase tb_House;

   public List<TileBase> tb_Rice;
   
   private DatabaseReference _reference;
   
   public PlayerFarmController playerFarmController;
   
   private FirebaseDatabaseManager dataDatabaseManager;
   void Start()
   {
      // Tắt tính năng lưu cache offline để dữ liệu luôn được lấy từ server
      //FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(true);

      
      dataDatabaseManager = GameObject.Find("DatabaseManager").GetComponent<FirebaseDatabaseManager>();
      if (LoadDataManager.userInGame.MapInGame.lstmap == null)
      {
         WriteAllTitleMapToFirebase();
      }
      else
      {
         LoadMapForUser();
      }
      FirebaseApp app = FirebaseApp.DefaultInstance;
      _reference = FirebaseDatabase.DefaultInstance.RootReference;
      
   }
   
   public void WriteAllTitleMapToFirebase()
   {
      List<TileMapDetail> tileMapDetails = new List<TileMapDetail>();
      int minX = tm_Ground.cellBounds.min.x;
      int maxX = tm_Ground.cellBounds.max.x;
      int midX = minX + (maxX - minX) / 2;
      int minY = tm_Ground.cellBounds.min.x;
      int maxY = tm_Ground.cellBounds.max.x;
      int midy = minY + (maxY - minY) / 2;
      
      
      //Scale (Độ thu phóng): Càng nhỏ thì các "cụm" tài nguyên càng to và mượt.
      float scale = 0.15f; 
    
      // Offset (Độ lệch / Seed): Perlin Noise luôn trả về cùng một giá trị cho cùng một tọa độ.
      // Việc cộng thêm offset ngẫu nhiên giúp mỗi lần tạo map sẽ ra một hình thù khác nhau (Random Seed) nên mỗi người chơi là một map khác nhau.
      float offsetX = UnityEngine.Random.Range(0f, 100000f);
      float offsetY = UnityEngine.Random.Range(0f, 100000f);
    
      // Threshold (Ngưỡng): Giới hạn để quyết định tạo Sprout hay Grass.
      // Ví dụ: < 0.3f (khoảng 30% diện tích) sẽ là Sprout.
      float threshold = 0.3f; 

      for (int x = minX; x < midy; x++)
      {
         for (int y = tm_Ground.cellBounds.min.y; y < tm_Ground.cellBounds.max.y; y++)
         {
            // Tính toán tọa độ đầu vào cho hàm Perlin Noise
            float xCoord = (x + offsetX) * scale;
            float yCoord = (y + offsetY) * scale;

            // Lấy giá trị nhiễu (luôn nằm trong khoảng từ 0.0 đến 1.0)
            float perlinValue = Mathf.PerlinNoise(xCoord, yCoord);

            // Sinh trạng thái dựa trên ngưỡng (Threshold)
            State generatedState = (perlinValue < threshold) ? State.Sprout : State.Grass;
            
            if (generatedState == State.Sprout)
            {
               TileMapDetail tm_detail = new TileMapDetail(x, y, generatedState, DateTime.Now);
               tileMapDetails.Add(tm_detail);
            }
         }
      }
      LoadDataManager.userInGame.MapInGame = new Map(tileMapDetails);
      dataDatabaseManager.WriteDatabase("Users/" + LoadDataManager.firebaseUser.UserId,LoadDataManager.userInGame.ToString());
   }

   public void LoadMapForUser()
   {
      MapToUI(LoadDataManager.userInGame.MapInGame);
   }

   public void TilemapDetailToTileBase(TileMapDetail tileMapDetail)
   {
      Vector3Int cellPos = new Vector3Int(tileMapDetail.x,tileMapDetail.y,0);
      if (tileMapDetail.tilemapState == State.Ground)
      {
         tm_Grass.SetTile(cellPos,null);
         tm_Forest.SetTile(cellPos,null);
      }
      else if (tileMapDetail.tilemapState == State.Grass)
      {
         tm_Forest.SetTile(cellPos,null);
      }
      else if (tileMapDetail.tilemapState == State.Plants)
      {
         double elapsedTime = DateTime.Now.Subtract(tileMapDetail.growTime).TotalSeconds;
         tm_Grass.SetTile(cellPos,null);
         
         if (elapsedTime > 20)
         {
            // tm_Forest.SetTile(cellPos, tb_Rice[4]);
            playerFarmController.StartCoroutine(playerFarmController.GrowPlant(cellPos,tm_Forest,tb_Rice.GetRange(4,1),1));

         }
         else if (elapsedTime > 15)
         {
            // tm_Forest.SetTile(cellPos, tb_Rice[3]);
            playerFarmController.StartCoroutine(playerFarmController.GrowPlant(cellPos,tm_Forest,tb_Rice.GetRange(3,2),1));

         }
         else if (elapsedTime > 10)
         {
            // tm_Forest.SetTile(cellPos, tb_Rice[2]);
            playerFarmController.StartCoroutine(playerFarmController.GrowPlant(cellPos,tm_Forest,tb_Rice.GetRange(2,3),1));

         }
         else if (elapsedTime > 5)
         {
            // tm_Forest.SetTile(cellPos, tb_Rice[1]);
            playerFarmController.StartCoroutine(playerFarmController.GrowPlant(cellPos,tm_Forest,tb_Rice.GetRange(1,4),1));
         }
         else
         {
            // tm_Forest.SetTile(cellPos,tb_Rice[0]);
            playerFarmController.StartCoroutine(playerFarmController.GrowPlant(cellPos,tm_Forest,tb_Rice,1));

         }
      }
      else if (tileMapDetail.tilemapState == State.Sprout)
      {
         tm_Sprout.SetTile(cellPos,tb_Sprout);
      }
      else if (tileMapDetail.tilemapState == State.House)
      {
         tm_House.SetTile(cellPos,tb_House);
      }
      
   }

   public void MapToUI(Map map)
   {
      for (int i = 0; i < map.GetLength(); i++)
      {
         TilemapDetailToTileBase(map.lstmap[i]);
      }
   }

   public void RemoveTilemapDetail(TileMapDetail tileMapDetail)
   {
      LoadDataManager.userInGame.MapInGame.lstmap.Remove(tileMapDetail);
      dataDatabaseManager.WriteDatabase("Users/" + LoadDataManager.firebaseUser.UserId,LoadDataManager.userInGame.ToString());
   }

   public void SetStateForTilemapDetail(int x, int y, State state)
   {
      // Kiểm tra xem đối tượng map đã tồn tại chưa trước khi gọi GetLength()
      for(int i=0;i<LoadDataManager.userInGame.MapInGame.GetLength();i++)
      {
         if (LoadDataManager.userInGame.MapInGame.lstmap[i].x == x && LoadDataManager.userInGame.MapInGame.lstmap[i].y == y)
         {
            LoadDataManager.userInGame.MapInGame.lstmap[i].tilemapState = state;
            LoadDataManager.userInGame.MapInGame.lstmap[i].growTime = DateTime.Now;
            dataDatabaseManager.WriteDatabase("Users/" + LoadDataManager.firebaseUser.UserId,LoadDataManager.userInGame.ToString());
         }
      }
   }

   public void AddStateForTilemapDetail(int x, int y, State state)
   {
      TileMapDetail tm_detail = new TileMapDetail(x, y, state, DateTime.Now);
      LoadDataManager.userInGame.MapInGame.lstmap.Add(tm_detail);
      dataDatabaseManager.WriteDatabase("Users/" + LoadDataManager.firebaseUser.UserId,LoadDataManager.userInGame.ToString());
   }
}
