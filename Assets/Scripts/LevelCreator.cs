using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    public class LevelCreator : MonoBehaviour
    {
        public int Height = 19;
        public int Width = 11;

        [SerializeField] GameObject _voxelPrefab;
        [SerializeField] GameObject _emptyPrefab;
        [SerializeField] SaveLoadController _saveLoadController;
        [SerializeField] InputController _inputController;
        [SerializeField] TMP_Dropdown _dropdown;

        List<GameObject> _createdObjs;
        LineRenderer[] _map;
        LineRenderer _currentLineRenderer;
        List<Vector3> _vertices;
        List<int> _triangles;
        int _currentIndex = 0;
        bool _CounterClockWise = false;
        LevelMap _currentLevelMap;

        public void LoadLevel(int level)
        {

            foreach (GameObject obj in _createdObjs)
            {
                Destroy(obj);
            }
            if (level == 0)
            {
                _currentLevelMap = new LevelMap();
                _currentLevelMap.Level = _saveLoadController.LevelMapsCount + 1;
                return;
            }

            _saveLoadController.LoadLevelMap(level);
            _createdObjs.Clear();
            CreateMeshFromMap();
        }

        public void SetLevel(LevelMap map)
        {
            _currentLevelMap = map;
        }

        public void SaveLevel()
        {
            _saveLoadController.Save(_currentLevelMap);
            foreach (GameObject obj in _createdObjs)
            {
                Destroy(obj);
            }
            _createdObjs.Clear();
            ResetDots();
            SetOptionsForDropDown();

            _currentLevelMap = new LevelMap();
            _currentLevelMap.Level = _saveLoadController.LevelMapsCount + 1;
        }

        public void ClearLevelMap()
        {
            foreach (GameObject obj in _createdObjs)
            {
                Destroy(obj);
            }
            _currentLevelMap.ClearData();
        }

        void Start()
        {
            _saveLoadController.OnLoad += e => SetLevel(e);
            _inputController.OnClick += e => Clicked(e);
            _createdObjs = new List<GameObject>();
            _map = new LineRenderer[Width * Height];
            CreateVoxelMap();
            _vertices = new List<Vector3>();
            _triangles = new List<int>();
            SetOptionsForDropDown();
            LoadLevel(0);
        }
        void CreateVoxelMap()
        {
            for (int y = 0, i = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++, i++)
                {
                    GameObject obj = Instantiate(_voxelPrefab, transform);
                    _map[i] = obj.GetComponent<LineRenderer>();
                    obj.transform.position = new Vector3(x, y, 1f);
                }
            }
        }

        void SetOptionsForDropDown()
        {
            List<string> options = new List<string>();
            string optionName = "Level - ";
            options.Add("Create new level");
            for (int i = 1; i < _saveLoadController.LevelMapsCount + 1; i++)
            {
                options.Add(optionName + i);
            }
            _dropdown.ClearOptions();
            _dropdown.AddOptions(options);
        }

        void Clicked(PointerEventData pointEvent)
        {
            var pos = Camera.main.ScreenToWorldPoint(pointEvent.position);
            pos.z = 0f;
            pos += new Vector3(0.5f, 0.5f, 0f);
            pos = new Vector3((int)pos.x, (int)pos.y, pos.z);

            if (CheckForRightPosition(pos))
            {
                SetVertice(pos);
            }

        }

        void SetVertice(Vector3 vertice)
        {
            if (_currentIndex > 2)
            {
                if (!FirstCheckCorrectLocation(vertice))
                {
                    ResetDots();
                    return;
                }
            }
            if (_currentIndex > 4)
            {
                if (!IsCorrectLocated(vertice) || IsReadyToCreateMesh(vertice))
                {
                    return;
                }
            }
            _vertices.Add(vertice);
            vertice.z = 1f;
            _vertices.Add(vertice);
            if (_currentLineRenderer != null)
            {
                _currentLineRenderer.SetPosition(1, vertice);
            }
            _currentLineRenderer = _map[(int)(vertice.x + (vertice.y * Width))];
            _currentLineRenderer.SetPosition(0, _currentLineRenderer.transform.position);
            _currentLineRenderer.SetPosition(1, _currentLineRenderer.transform.position);

            _currentIndex += 2;
            if (_currentIndex > 14)
            {
                ResetDots();
            }
        }

        bool IsReadyToCreateMesh(Vector3 vertice)
        {
            if (_vertices[0] == vertice)
            {
                CreateMesh();
                return true;
            }
            return false;
        }

        void CreateMesh()
        {
            Create3DObj();

            var obj = Instantiate(_emptyPrefab).GetComponent<MeshFilter>();
            _createdObjs.Add(obj.gameObject);

            Vector3[] newVertices = new Vector3[_triangles.Count];
            for (int i = 0; i < _triangles.Count; i++)
            {
                newVertices[i] = _vertices[_triangles[i]];
                _triangles[i] = i;
            }

            obj.mesh.vertices = newVertices;
            obj.mesh.triangles = _triangles.ToArray();
            obj.mesh.RecalculateNormals();
            obj.GetComponent<MeshCollider>().sharedMesh = obj.mesh;
            obj.transform.position = Vector3.zero;
            _currentLevelMap.AddVertices(newVertices);
            _currentLevelMap.AddTriangles(_triangles.ToArray());

            ResetDots();

        }

        void Create3DObj()
        {
            int a = 4;
            int b = 2;

            if (_CounterClockWise == false)
            {
                a = 2;
                b = 4;
            }

            for (int i = 0, v = 1; i < _vertices.Count - 4; i += 2, v += 2)
            {
                AddTriangle(0, i + a, i + b);
                AddTriangle(1, v + b, i + a);
                AddSquad(i, v, i + 2, v + 2);
                if(i == _vertices.Count - 6)
                {
                    AddSquad(1, 0, _vertices.Count - 1, _vertices.Count - 2);
                    AddSquad(_vertices.Count - 1, _vertices.Count - 2, _vertices.Count - 3, _vertices.Count - 4);
                }
            }
        }

        void CreateMeshFromMap()
        {
            for (int i = 0; i < _currentLevelMap.TargetsCount; i++)
            {
                var vertices = _currentLevelMap.GetVertices(i);
                var triangles = _currentLevelMap.GetTriangles(i);
                var obj = Instantiate(_emptyPrefab).GetComponent<MeshFilter>();
                _createdObjs.Add(obj.gameObject);
                obj.mesh.vertices = vertices;
                obj.mesh.triangles = triangles;
                obj.mesh.RecalculateNormals();
                obj.transform.position = Vector3.zero;
            }

        }

        void AddTriangle(int a, int b, int c)
        {
            _triangles.Add(a);
            _triangles.Add(b);
            _triangles.Add(c);
        }
        void AddSquad(int a, int b, int c, int d)
        {
            if (_CounterClockWise)
            {

                _triangles.Add(a);
                _triangles.Add(c);
                _triangles.Add(b);
                _triangles.Add(b);
                _triangles.Add(c);
                _triangles.Add(d);
            }
            else
            {
                _triangles.Add(a);
                _triangles.Add(b);
                _triangles.Add(c);
                _triangles.Add(b);
                _triangles.Add(d);
                _triangles.Add(c);
            }

        }

        bool FirstCheckCorrectLocation(Vector3 newDot)
        {
            Vector3 a = _vertices[_currentIndex - 4];
            Vector3 b = _vertices[_currentIndex - 2];

            Vector3 lastDirection = (b - a).normalized;
            Vector3 sideDirection = (newDot - b).normalized;

            float directionAngle = Vector3.SignedAngle(lastDirection, sideDirection, Vector3.forward);

            if (directionAngle == 0 || directionAngle == 180)
            {
                return false;
            }
            return true;
        }

        bool IsCorrectLocated(Vector3 newDot)
        {
            Vector3 a = _vertices[_currentIndex - 4];
            Vector3 b = _vertices[_currentIndex - 2];

            Vector3 firstDir = (_vertices[2] - _vertices[0]).normalized;
            Vector3 secondDir = (_vertices[4] - _vertices[2]).normalized;
            Vector3 lastDirection = (b - a).normalized;
            Vector3 sideDirection = (newDot - b).normalized;

            float firstDirAngle = Vector3.SignedAngle(firstDir, secondDir, Vector3.forward);

            float directionAngle = Vector3.SignedAngle(lastDirection, sideDirection, Vector3.forward);



            if (directionAngle == 0 || directionAngle == 180)
            {
                return false;
            }

            _CounterClockWise = firstDirAngle > 0 ? true : false;

            if (_CounterClockWise != (directionAngle > 0 ? true : false))
            {
                return false;
            }

            return true;
        }

        void ResetDots()
        {
            foreach (LineRenderer line in _map)
            {
                line.SetPosition(0, line.transform.position);
                line.SetPosition(1, line.transform.position);
            }
            _currentIndex = 0;
            _currentLineRenderer = null;
            _vertices.Clear();
            _triangles.Clear();
        }
        bool CheckForRightPosition(Vector3 pos)
        {
            bool value = true;
            if (pos.x < 0 || pos.x > Width - 1)
            {
                value = false;
            }
            else if (pos.y < 0 || pos.y > Height - 1)
            {
                value = false;
            }
            return value;
        }
    }
}


