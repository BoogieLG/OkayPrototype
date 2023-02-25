using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] GameObject _targetsPrefab;
        [SerializeField] InputController _inputController;
        [SerializeField] ObjController _objController;
        [SerializeField] LevelController _levelController;
        [SerializeField] SaveLoadController _saveload;

        [SerializeField] List<TargetController> _targets;

        int _currentLevel;

        void Start()
        {
            LoadData();

            _saveload.OnLoad += e => _levelController.GetLevelMap(e);
            _levelController.OnGetLevel += e => _saveload.LoadLevelMap(e);
            _levelController.OnSaveLevel += e => _saveload.Save(e);
            _inputController.OnBeginDragging += e => _objController.SetStartPosition(e);
            _inputController.OnEndDragging += e => _objController.SetEndPosition(e);
            _objController.OnResetGame += Restart;
            _objController.OnHit += e => GetHit(e);
            SetLevel(_currentLevel);
        }

        void Restart()
        {
            foreach (TargetController target in _targets)
            {
                target.Restart();
            }
        }

        void GetHit(TargetController target)
        {
            target.GetHit();
            CheckForGameOver();
        }

        void SetLevel(int level)
        {
            foreach(TargetController obj in _targets)
            {
                Destroy(obj.gameObject);
            }
            _targets.Clear();
            _levelController.SetLevelMap(_currentLevel);
            for (int i = 0; i < _levelController.TargetCounts; i++)
            {
                var obj = Instantiate(_targetsPrefab, transform).GetComponent<MeshFilter>();
                _levelController.GetData(i, out Vector3[] vertices, out int[] triangles);
                obj.mesh.vertices = vertices;
                obj.mesh.triangles = triangles;
                obj.mesh.RecalculateBounds();
                obj.mesh.RecalculateNormals();
                obj.GetComponent<MeshCollider>().sharedMesh = obj.mesh;
                _targets.Add(obj.gameObject.GetComponent<TargetController>());
            }
        }

        void UnSubscribeControllers()
        {
            _inputController.OnBeginDragging -= e => _objController.SetStartPosition(e);
            _inputController.OnEndDragging -= e => _objController.SetEndPosition(e);
            _objController.OnResetGame -= Restart;
            _objController.OnHit -= e => GetHit(e);
        }

        void CheckForGameOver()
        {
            bool gameOver = true;
            foreach (TargetController target in _targets)
            {
                if (!target.GetStatus())
                {
                    gameOver = false;
                }
            }
            if (gameOver)
            {
                _currentLevel++;
                SaveData();
                SetLevel(_currentLevel);
            }
        }

        void SaveData()
        {
            PlayerPrefs.SetInt("Level", _currentLevel);
        }
        void LoadData()
        {
            _currentLevel = 1; // As prototype, better start scene from first level;
        }
        void OnDestroy()
        {
            UnSubscribeControllers();
        }
    }
}
