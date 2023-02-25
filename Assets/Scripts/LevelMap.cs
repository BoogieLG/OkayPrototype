using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    public class LevelMap
    {
        public int Level;

        public int TargetsCount => _targetsCount;
        [SerializeField] int _targetsCount;


        [SerializeField] List<int[]> _triangles = new List<int[]>();

        [SerializeField] List<SerializableVector3[]> _vertices = new List<SerializableVector3[]>();

        public void AddVertices(Vector3[] vertices)
        {
            SerializableVector3[] result = new SerializableVector3[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                result[i] = vertices[i];
            }
            _vertices.Add(result);
            _targetsCount++;
        }

        public void AddTriangles(int[] triangles)
        {
            _triangles.Add(triangles);
        }

        public Vector3[] GetVertices(int index)
        {
            SerializableVector3[] temp = _vertices[index];
            Vector3[] result = new Vector3[temp.Length];

            for (int i = 0; i < temp.Length; i++)
            {
                result[i] = temp[i];
            }

            return result;
        }

        public int[] GetTriangles(int index)
        {
            return _triangles[index];
        }

        public void ClearData()
        {
            _triangles.Clear();
            _vertices.Clear();
            _targetsCount = 0;
        }
    }
}
