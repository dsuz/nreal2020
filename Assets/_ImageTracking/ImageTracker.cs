using System.Collections.Generic;
using UnityEngine;
using NRKernal;

public class ImageTracker : MonoBehaviour
{
    /// <summary>�摜�����������ɒu���v���n�u</summary>
    [SerializeField] GameObject _prefab;
    /// <summary>�C���X�^���X������đ��݂��Ă���v���n�u�̃��X�g�B�C���f�b�N�X�̓f�[�^�x�[�X���̉摜�I�u�W�F�N�g�B</summary>
    Dictionary<NRTrackableImage, GameObject> _prefabInstanceDictionary = new Dictionary<NRTrackableImage, GameObject>();
    /// <summary>�摜�̃C���f�b�N�X�Ɩ��O��R�Â��邽�߂̃f�[�^�x�[�X</summary>
    NRTrackingImageDatabase _db = default;

    void Start()
    {
        // �摜�f�[�^�x�[�X���擾����
        var config = NRSessionManager.Instance.NRSessionBehaviour.SessionConfig;
        _db = config.TrackingImageDatabase;
    }

    void Update()
    {
        if (NRFrame.SessionStatus != SessionState.Running) return;
        // �B�e�͈͂ɑ��݂��Ă��錟�o�\�ȉ摜���擾����
        List<NRTrackableImage> trackedImageList = new List<NRTrackableImage>();
        NRFrame.GetTrackables<NRTrackableImage>(trackedImageList, NRTrackableQueryFilter.New);  // ����̂݌��o����
        
        // �摜�����o������v���n�u�𐶐�����i������ʂ�̂͏���̂݁j
        foreach (var image in trackedImageList)
        {
            if (image.GetTrackingState() == TrackingState.Tracking)
            {
                Debug.Log($"Detected {_db[image.GetDataBaseIndex()].Name}.");
                // �v���n�u���C���X�^���X������
                var go = Instantiate(_prefab);
                _prefabInstanceDictionary.Add(image, go);
            }
        }

        // ���ɃC���X�^���X�����ꂽ�v���n�u����������o������ړ�����
        foreach (var e in _prefabInstanceDictionary)
        {
            if (e.Key.GetTrackingState() == TrackingState.Tracking)
            {
                if (!e.Value.activeSelf)
                {
                    Debug.Log($"Re-detected {_db[e.Key.GetDataBaseIndex()].Name}");
                    // ��A�N�e�B�u�ɂ��Ă����摜���J���������o������ēx�A�N�e�B�u������
                    e.Value.SetActive(true);
                }

                // �摜�����o�����ꏊ�Ƀv���n�u�C���X�^���X���ړ�����
                Pose center = e.Key.GetCenterPose();
                e.Value.transform.position = center.position;
            }
            else if (e.Value.activeSelf)
            {
                Debug.Log($"Lost {_db[e.Key.GetDataBaseIndex()].Name}");
                // �摜���J�����͈̔͂���O�ꂽ��v���n�u���A�N�e�B�u������
                e.Value.SetActive(false);
            }
        }
    }
}
