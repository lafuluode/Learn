using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using JetBrains.Annotations;

namespace Game.Framework.Storage
{
    /// <summary>
    /// 本地文件存储实现
    /// </summary>
    public sealed class LocalFileStorage : IFileStorage
    {
        private readonly string rootPath;

        public LocalFileStorage(string rootPath)
        {
            if (string.IsNullOrEmpty(rootPath))
            {
                throw new ArgumentException("存储根目录不能为空", nameof(rootPath));
            }

            this.rootPath = rootPath;
            if (!Directory.Exists(this.rootPath))
            {
                Directory.CreateDirectory(this.rootPath);
            }
        }

        public bool Exists(string relativePath)
        {
            string fullPath = GetFullPath(relativePath);

            return File.Exists(fullPath);
        }

        public byte[] Read(string relativePath)
        {
            string fullPath = GetFullPath(relativePath);
            
            if(!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"文件不存在: {relativePath}");
            }
            try
            {
                return File.ReadAllBytes(fullPath);
            }
            catch (Exception ex)
            {
                throw new IOException($"读取文件失败: {relativePath}", ex);
            }
        }

        public void Write(string relativePath, byte[] bytes)
        {
            if(bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            string fullPath = GetFullPath(relativePath);
            string directory = Path.GetDirectoryName(fullPath);

            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string tempPath = fullPath + ".tmp";
            string backupPath = fullPath + ".bak";
            try
            {
                File.WriteAllBytes(tempPath, bytes);
                if (File.Exists(fullPath))
                {
                    File.Copy(fullPath, backupPath, true);
                    File.Delete(fullPath);
                }
                File.Move(tempPath, fullPath);
            }
            catch (Exception ex)
            {
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
                throw new IOException($"写入文件失败: {relativePath}", ex);
            }
        }
        public void Delete(string relativePath)
        {
            string fullPath = GetFullPath(relativePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        private string GetFullPath(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
            {
                throw new ArgumentException("相对路径不能为空", nameof(relativePath));
            }
            if (Path.IsPathRooted(relativePath))
            {
                throw new ArgumentException("相对路径不能是绝对路径", nameof(relativePath));
            }
            return Path.Combine(rootPath, relativePath);
        } 
    }
}