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
   
   public TileBase tb_Forest;
   
   private Map map;
   private FirebaseUser _user;

   private DatabaseReference _reference;
   
   private FirebaseDatabaseManager dataDatabaseManager;
   void Start()
   {
      // Tắt tính năng lưu cache offline để dữ liệu luôn được lấy từ server
      //FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(true);
      map = new Map();
      
      dataDatabaseManager = GameObject.Find("DatabaseManager").GetComponent<FirebaseDatabaseManager>();
      _user = FirebaseAuth.DefaultInstance.CurrentUser;
      
      //WriteAllTitleMapToFirebase();
      FirebaseApp app = FirebaseApp.DefaultInstance;
      _reference = FirebaseDatabase.DefaultInstance.RootReference;
      LoadMapForUser();
   }
   
   public void WriteAllTitleMapToFirebase()
   {
      List<TileMapDetail> tileMapDetails = new List<TileMapDetail>();
      for (int x = tm_Ground.cellBounds.min.x; x < tm_Ground.cellBounds.max.x; x++)
      {
         for (int y = tm_Ground.cellBounds.min.y; y < tm_Ground.cellBounds.max.y; y++)
         {
            TileMapDetail tm_detail = new TileMapDetail(x,y,State.Grass);
            tileMapDetails.Add(tm_detail);
         }
      }
      map = new Map(tileMapDetails);

      dataDatabaseManager.WriteDatabase(_user.UserId + "/Map",map.ToString());
   }

   public void LoadMapForUser()
   {
      _reference.Child("Users").Child(_user.UserId + "/Map").GetValueAsync().ContinueWithOnMainThread(task =>
      {
         if (task.IsCanceled) return;
         else if (task.IsFaulted) return;
         else if (task.IsCompleted)
         {
            DataSnapshot snapshot = task.Result;
            map = JsonConvert.DeserializeObject<Map>(snapshot.Value.ToString());
            Debug.Log("Load map:"+map.ToString());
            MapToUI(map);
         }
      });
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
         tm_Grass.SetTile(cellPos,null);
         tm_Forest.SetTile(cellPos,tb_Forest);
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
      if (map == null || map.lstmap == null) 
      {
         Debug.Log("Dữ liệu Map chưa được tải xong hoặc bị trống!");
         return; 
      }
      for(int i=0;i<map.GetLength();i++)
      {
         if (map.lstmap[i].x == x && map.lstmap[i].y == y)
         {
            map.lstmap[i].titlemapState = state;
            dataDatabaseManager.WriteDatabase(_user.UserId + "/Map",map.ToString());
            
         }
      }
      
   }
}
