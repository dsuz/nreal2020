using System.Collections.Generic;
using UnityEngine;
using NRKernal;

public class ImageTracker : MonoBehaviour
{
    /// <summary>画像を見つけた時に置くプレハブ</summary>
    [SerializeField] GameObject _prefab;
    /// <summary>インスタンス化されて存在しているプレハブのリスト。インデックスはデータベース内の画像オブジェクト。</summary>
    Dictionary<NRTrackableImage, GameObject> _prefabInstanceDictionary = new Dictionary<NRTrackableImage, GameObject>();
    /// <summary>画像のインデックスと名前を紐づけるためのデータベース</summary>
    NRTrackingImageDatabase _db = default;

    void Start()
    {
        // 画像データベースを取得する
        var config = NRSessionManager.Instance.NRSessionBehaviour.SessionConfig;
        _db = config.TrackingImageDatabase;
    }

    void Update()
    {
        if (NRFrame.SessionStatus != SessionState.Running) return;
        // 撮影範囲に存在している検出可能な画像を取得する
        List<NRTrackableImage> trackedImageList = new List<NRTrackableImage>();
        NRFrame.GetTrackables<NRTrackableImage>(trackedImageList, NRTrackableQueryFilter.New);  // 初回のみ検出する
        
        // 画像を検出したらプレハブを生成する（ここを通るのは初回のみ）
        foreach (var image in trackedImageList)
        {
            if (image.GetTrackingState() == TrackingState.Tracking)
            {
                Debug.Log($"Detected {_db[image.GetDataBaseIndex()].Name}.");
                // プレハブをインスタンス化する
                var go = Instantiate(_prefab);
                _prefabInstanceDictionary.Add(image, go);
            }
        }

        // 既にインスタンス化されたプレハブを消したり出したり移動する
        foreach (var e in _prefabInstanceDictionary)
        {
            if (e.Key.GetTrackingState() == TrackingState.Tracking)
            {
                if (!e.Value.activeSelf)
                {
                    Debug.Log($"Re-detected {_db[e.Key.GetDataBaseIndex()].Name}");
                    // 非アクティブにしていた画像をカメラが検出したら再度アクティブ化する
                    e.Value.SetActive(true);
                }

                // 画像を検出した場所にプレハブインスタンスを移動する
                Pose center = e.Key.GetCenterPose();
                e.Value.transform.position = center.position;
            }
            else if (e.Value.activeSelf)
            {
                Debug.Log($"Lost {_db[e.Key.GetDataBaseIndex()].Name}");
                // 画像がカメラの範囲から外れたらプレハブを非アクティブ化する
                e.Value.SetActive(false);
            }
        }
    }
}
