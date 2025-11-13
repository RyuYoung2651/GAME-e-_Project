using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    //각 큐브별 스프라이트
    public Sprite GrassPrefab;
    public Sprite DirtPrefab;
    public Sprite WaterPrefab;

    public List<Transform> Slot;  //내 UI의 각 슬롯들의 리스트
    public GameObject SlotItem;     //슬롯 내붕 들어가는 아이템
    List<GameObject> items;          //아이템 삭제용 전체 리스트
    //인벤토리 업데이트 시 호출

    public void UpdateInventory(Inventory myinven)
    {
        //1 슬롯 초기화
        foreach(var slotItems in items)
        {
            Destroy(slotItems);     //시작할때 슬롯 아이템의 게임오브젝트 삭제
        }
        items.Clear();      //시작할때 아이템 리스트 클리어
        //2 내 인벤토리 데이터 전체 탐색
        int idx = 0;        // 접근할 슬롯의 인덱스
        foreach(var item in myinven.items)
        {
           // 슬롯아이템 생성로직(게임오브젝트 인스턴스 생성, 위치 조정, 슬롯아이템 프리팹 컴퍼먼트 가져오기, 그 후 아이템 세팅)
           var go = Instantiate(SlotItem, Slot[idx].transform);
            go.transform.localPosition = Vector3.zero;
            SlotItemPrefab sitem = go.GetComponent<SlotItemPrefab>();
            items.Add(go);   //아이템 리스트에 하나 추가

            switch(item.Key) //각 케이스별로 아이템 추가
            {
                case BlockType.Dirt:
                   // sitem.ItemSetting(Color.black,)
                    break;
                case BlockType.Grass:
                    break;
                case BlockType.Water:
                    break;
            }
            idx++;      //인덱스 한 칸 추가
        }
    }
}
