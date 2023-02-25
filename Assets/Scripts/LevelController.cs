using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class LevelController : MonoBehaviour
    {
        public int TargetCounts => _currentLevel.TargetsCount;

        public Action<int> OnGetLevel;
        public Action<LevelMap> OnSaveLevel;

        [SerializeField] LevelMap _currentLevel;


        public void SetLevelMap(int level)
        {
            OnGetLevel?.Invoke(level);
        }

        public void AddTarget(Vector3[] vertices, int[] triangles)
        {
            _currentLevel.AddVertices(vertices);
            _currentLevel.AddTriangles(triangles);
        }


        public void GetData(int index, out Vector3[] vertices, out int[] triangles)
        {
            if (index < _currentLevel.TargetsCount)
            {
                vertices = _currentLevel.GetVertices(index);
                triangles = _currentLevel.GetTriangles(index);
                return;
            }

            vertices = null;
            triangles = null;
            Debug.LogError($"Index: {index}  is wrong index for GetData() from LevelScriptableObject");
        }

        public void GetLevelMap(LevelMap levelMap)
        {
            _currentLevel = levelMap;
        }

        public void SaveLevelMap()
        {
            OnSaveLevel?.Invoke(_currentLevel);
        }

    }
}
