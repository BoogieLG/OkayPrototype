using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Assets.Scripts
{
    public class SaveLoadController : MonoBehaviour
    {
        public int LevelMapsCount => _levelMaps.Count;

        public Action<LevelMap> OnLoad;

        string _directory = "Levels";
        string _fileName = "null";

        List<LevelMap> _levelMaps;


        public void LoadLevelMap(int level)
        {
            if (level < _levelMaps.Count + 1)
            {
                OnLoad?.Invoke(_levelMaps[level - 1]);
            }
            else
            {
                Debug.Log("Wrong index for SaveLoadController.LoadLevelMap(index) or you passed last level :) ");
            }

        }

        public void Save(LevelMap levelMap)
        {
            _fileName = $"Level - {levelMap.Level}.levelMap";
            Debug.Log(GetFullPath());
            if (!DirectoryExist()) Directory.CreateDirectory(Application.persistentDataPath + "/" + _directory);
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/" + _directory + "/" + _fileName);
            bf.Serialize(file, levelMap);
            file.Close();
            Load();

        }

        void Awake()
        {
            _levelMaps = new List<LevelMap>();
            Load();
        }

        void Load()
        {
            _levelMaps.Clear();
            string[] files = Directory.GetFiles(Application.persistentDataPath + "/" + _directory, "*.levelMap");
            for (int i = 0; i < files.Length; i++)
            {
                _fileName = $"Level - {i + 1}.levelMap";
                if (DataExist())
                {
                    try
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        FileStream file = File.Open(GetFullPath(), FileMode.Open);
                        LevelMap savedGame = (LevelMap)bf.Deserialize(file);
                        file.Close();
                        _levelMaps.Add(savedGame);
                    }

                    catch (SerializationException)
                    {
                        Debug.Log("Failed to load saveData");
                    }
                }
                else
                {
                    Debug.Log("Wrong index loop for SaveLoadController.Load()");
                }
            }
        }

        bool DataExist()
        {
            return File.Exists(GetFullPath());
        }

        bool DirectoryExist()
        {
            return Directory.Exists(Application.persistentDataPath + "/" + _directory);
        }

        string GetFullPath()
        {
            return Application.persistentDataPath + "/" + _directory + "/" + _fileName;
        }
    }
}
