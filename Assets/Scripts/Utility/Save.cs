using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Utility
{
    public static class Save 
    {
        public static string DataFilePath = @"DataFiles";

        public static string CreateFile(string content)
        {
            string FileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".txt";
            return CreateFile(content, FileName);
        }

        public static string CreateFile(string content, string filename)
        {
            string relativePath;
            if (Application.isEditor)
            {
                if (!Directory.Exists(Path.Combine("Assets", DataFilePath)))
                {
                    Directory.CreateDirectory(Path.Combine("Assets", DataFilePath));
                }
                relativePath = Path.Combine("Assets", DataFilePath, filename);
            }
            else
            {
                if (!Directory.Exists(DataFilePath))
                {
                    Directory.CreateDirectory(DataFilePath);
                }
                relativePath = Path.Combine(DataFilePath, filename);
            }

            using (StreamWriter streamWriter = File.CreateText(relativePath))
            {
                streamWriter.Write(content);
            }
            return Path.Combine(DataFilePath, filename);
        }
    }
}

