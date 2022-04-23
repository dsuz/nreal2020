using UnityEngine;
using NRKernal;

/// <summary>
/// Nreal で本を操作するコンポーネント。対応している操作は以下の通り。
/// 下スワイプ: 本を取り出す
/// 右スワイプ: 本をめくる
/// 左スワイプ: 本をめくる
/// 下スワイプ: 本を片づける
/// エミュレーターで操作する場合は、カーソルキーを 1 秒弱押してから離すこと。すぐに離すとスワイプ操作とみなされない。
/// </summary>
public class BookController : MonoBehaviour
{
    /// <summary>本のアニメーターを指定する</summary>
    [SerializeField] Animator _book = default;
    /// <summary>スワイプとして検出する距離を指定する（小さい値にすればセンシティブになる）</summary>
    [SerializeField] float _swipeDetectDistance = 0.5f;
    /// <summary>直前のフレームでタッチしていたか</summary>
    bool _isTouchedOnLastFrame = false;
    /// <summary>スワイプ開始時のポジション</summary>
    Vector2 _touchStartPosition = default;

    void Update()
    {
        // スワイプを検出し、それぞれのスワイプにより適切な処理を行う（スワイプイベントがないので処理が面倒だ）
        bool isTouched = NRInput.IsTouching();
        
        if (isTouched && !_isTouchedOnLastFrame)    // タッチ開始
        {
            _touchStartPosition = NRInput.GetTouch();   // NRInput.Get* で入力を受け取る
            Debug.Log($"Touch start. Start position: {_touchStartPosition.ToString("F2")}");
        }
        else if (!isTouched && _isTouchedOnLastFrame)   // タッチ終了
        {
            // スワイプを検出する
            Vector2 touchEndPosition = NRInput.GetTouch();
            Debug.Log($"Touch end. End position: {touchEndPosition.ToString("F2")}");
            Vector2 diff = touchEndPosition - _touchStartPosition;  // タッチ終了とタッチ開始の差をとる

            if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
            {
                // 横方向スワイプとする
                if (diff.x > _swipeDetectDistance)
                {
                    // 右方向スワイプ検出
                    Debug.Log("Right Swipe detected.");
                    // ページをめくる
                    _book.SetTrigger("TurnPageForwardTrigger");
                }
                else if (diff.x < -1 * _swipeDetectDistance)
                {
                    // 左方向スワイプ検出
                    Debug.Log("Right Swipe detected.");
                    // ページをめくる
                    _book.SetTrigger("TurnPageBackTrigger");
                }
            }
            else
            {
                // 縦方向スワイプとする
                if (diff.y > _swipeDetectDistance)
                {
                    // 下方向スワイプ検出（上がマイナスなのでわかりにくい）
                    Debug.Log("Down Swipe detected.");
                    
                    // 本を取り出す
                    if (!_book.gameObject.activeSelf)   // 初回
                    {
                        _book.gameObject.SetActive(true);
                        _book.Play("SummonBook");
                    }
                    else if (_book.transform.localScale == Vector3.zero)    // 2回目以降
                    {
                        _book.Play("SummonBook");
                    }
                }
                else if (diff.y < -1 * _swipeDetectDistance)
                {
                    // 上方向スワイプ検出（上がマイナスなのでわかりにくい）
                    Debug.Log("Up Swipe detected.");
                    // 本を片付ける
                    _book.SetTrigger("HideTrigger");
                }
            }
        }

        _isTouchedOnLastFrame = isTouched;
    }
}
