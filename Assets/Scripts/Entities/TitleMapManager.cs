using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TitleMapManager : MonoBehaviour
{
   public Tilemap tm_Ground;
   public Tilemap tm_Grass;
   public Tilemap tm_Forest;
   public Tilemap tm_Sprout;
   
   public TileBase tb_Forest;
   public TileBase tb_Sprout;

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
      
      for (int x = minX; x < midX; x++)
      {
         for (int y = tm_Ground.cellBounds.min.y; y < tm_Ground.cellBounds.max.y; y++)
         {
            // PCG cho nửa trái: Random giữa Grass và Sprout
            // Sử dụng UnityEngine.Random.value (trả về giá trị từ 0.0 đến 1.0)
            // Nếu giá trị < 0.2 (20% tỷ lệ), thì tạo Sprout. Ngược lại tạo Grass.
            State randomState = (UnityEngine.Random.value < 0.2f) ? State.Sprout : State.Grass;
            TileMapDetail tm_detail = new TileMapDetail(x, y, randomState, DateTime.Now);
            tileMapDetails.Add(tm_detail);
         }
      }
      
      for (int x = midX; x < maxX; x++)
      {
         for (int y = tm_Ground.cellBounds.min.y; y < tm_Ground.cellBounds.max.y; y++)
         {
            // Nửa trái tạo mới toàn bộ là Grass
            TileMapDetail tm_detail = new TileMapDetail(x, y, State.Grass, DateTime.Now);
            tileMapDetails.Add(tm_detail);
         }
      }
      //
      // for (int x = tm_Ground.cellBounds.min.x; x < tm_Ground.cellBounds.max.x; x++)
      // {
      //    for (int y = tm_Ground.cellBounds.min.y; y < tm_Ground.cellBounds.max.y; y++)
      //    {
      //       TileMapDetail tm_detail = new TileMapDetail(x,y,State.Grass,DateTime.Now);
      //       tileMapDetails.Add(tm_detail);
      //    }
      // }

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
      if (tileMapDetail.titlemapState == State.Ground)
      {
         tm_Grass.SetTile(cellPos,null);
         tm_Forest.SetTile(cellPos,null);
      }
      else if (tileMapDetail.titlemapState == State.Grass)
      {
         tm_Forest.SetTile(cellPos,null);
         
         
         // if(cellPos == new Vector3Int(1, 1, 0))
         // {
         //    tm_Grass.SetTile(cellPos,null);
         //    tm_Forest.SetTile(cellPos,tb_Forest);
         // }else
         // {
         //    
         //    tm_Forest.SetTile(cellPos,null);
         // };
         
      }
      else if (tileMapDetail.titlemapState == State.Forest)
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
      else if (tileMapDetail.titlemapState == State.Sprout)
      {
         tm_Sprout.SetTile(cellPos,tb_Sprout);
      }
      
   }

   public void MapToUI(Map map)
   {
      for (int i = 0; i < map.GetLength(); i++)
      {
         TilemapDetailToTileBase(map.lstmap[i]);
      }
   }

   public void SetStateForTilemapDetail(int x, int y, State state)
   {
      // Kiểm tra xem đối tượng map đã tồn tại chưa trước khi gọi GetLength()
      for(int i=0;i<LoadDataManager.userInGame.MapInGame.GetLength();i++)
      {
         if (LoadDataManager.userInGame.MapInGame.lstmap[i].x == x && LoadDataManager.userInGame.MapInGame.lstmap[i].y == y)
         {
            LoadDataManager.userInGame.MapInGame.lstmap[i].titlemapState = state;
            LoadDataManager.userInGame.MapInGame.lstmap[i].growTime = DateTime.Now;
            dataDatabaseManager.WriteDatabase("Users/" + LoadDataManager.firebaseUser.UserId,LoadDataManager.userInGame.ToString());
         }
      }
      
   }
}
