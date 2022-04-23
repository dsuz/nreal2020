using UnityEngine;
using NRKernal;

/// <summary>
/// Nreal �Ŗ{�𑀍삷��R���|�[�l���g�B�Ή����Ă��鑀��͈ȉ��̒ʂ�B
/// ���X���C�v: �{�����o��
/// �E�X���C�v: �{���߂���
/// ���X���C�v: �{���߂���
/// ���X���C�v: �{��ЂÂ���
/// �G�~�����[�^�[�ő��삷��ꍇ�́A�J�[�\���L�[�� 1 �b�㉟���Ă��痣�����ƁB�����ɗ����ƃX���C�v����Ƃ݂Ȃ���Ȃ��B
/// </summary>
public class BookController : MonoBehaviour
{
    /// <summary>�{�̃A�j���[�^�[���w�肷��</summary>
    [SerializeField] Animator _book = default;
    /// <summary>�X���C�v�Ƃ��Č��o���鋗�����w�肷��i�������l�ɂ���΃Z���V�e�B�u�ɂȂ�j</summary>
    [SerializeField] float _swipeDetectDistance = 0.5f;
    /// <summary>���O�̃t���[���Ń^�b�`���Ă�����</summary>
    bool _isTouchedOnLastFrame = false;
    /// <summary>�X���C�v�J�n���̃|�W�V����</summary>
    Vector2 _touchStartPosition = default;

    void Update()
    {
        // �X���C�v�����o���A���ꂼ��̃X���C�v�ɂ��K�؂ȏ������s���i�X���C�v�C�x���g���Ȃ��̂ŏ������ʓ|���j
        bool isTouched = NRInput.IsTouching();
        
        if (isTouched && !_isTouchedOnLastFrame)    // �^�b�`�J�n
        {
            _touchStartPosition = NRInput.GetTouch();   // NRInput.Get* �œ��͂��󂯎��
            Debug.Log($"Touch start. Start position: {_touchStartPosition.ToString("F2")}");
        }
        else if (!isTouched && _isTouchedOnLastFrame)   // �^�b�`�I��
        {
            // �X���C�v�����o����
            Vector2 touchEndPosition = NRInput.GetTouch();
            Debug.Log($"Touch end. End position: {touchEndPosition.ToString("F2")}");
            Vector2 diff = touchEndPosition - _touchStartPosition;  // �^�b�`�I���ƃ^�b�`�J�n�̍����Ƃ�

            if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
            {
                // �������X���C�v�Ƃ���
                if (diff.x > _swipeDetectDistance)
                {
                    // �E�����X���C�v���o
                    Debug.Log("Right Swipe detected.");
                    // �y�[�W���߂���
                    _book.SetTrigger("TurnPageForwardTrigger");
                }
                else if (diff.x < -1 * _swipeDetectDistance)
                {
                    // �������X���C�v���o
                    Debug.Log("Right Swipe detected.");
                    // �y�[�W���߂���
                    _book.SetTrigger("TurnPageBackTrigger");
                }
            }
            else
            {
                // �c�����X���C�v�Ƃ���
                if (diff.y > _swipeDetectDistance)
                {
                    // �������X���C�v���o�i�オ�}�C�i�X�Ȃ̂ł킩��ɂ����j
                    Debug.Log("Down Swipe detected.");
                    
                    // �{�����o��
                    if (!_book.gameObject.activeSelf)   // ����
                    {
                        _book.gameObject.SetActive(true);
                        _book.Play("SummonBook");
                    }
                    else if (_book.transform.localScale == Vector3.zero)    // 2��ڈȍ~
                    {
                        _book.Play("SummonBook");
                    }
                }
                else if (diff.y < -1 * _swipeDetectDistance)
                {
                    // ������X���C�v���o�i�オ�}�C�i�X�Ȃ̂ł킩��ɂ����j
                    Debug.Log("Up Swipe detected.");
                    // �{��Еt����
                    _book.SetTrigger("HideTrigger");
                }
            }
        }

        _isTouchedOnLastFrame = isTouched;
    }
}
