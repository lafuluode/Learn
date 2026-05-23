using System;
using Game.Framework.Serialization;
using Game.Framework.Storage;

namespace Game.Framework.Save
{
    /// <summary>
    /// 本地存档服务
    /// </summary>
    public class SaveService : ISaveService
    {
        private readonly IDataSerializer serializer;
        private readonly IFileStorage fileStorage;

        private readonly string saveDirectory;
        private readonly string fileExtension;

        public SaveService(
            IDataSerializer serializer, 
            IFileStorage storage,
            string saveDirectory = "save",
            string fileExtension = ".json"
            )
        {
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            this.fileStorage = storage ?? throw new ArgumentNullException(nameof(storage));

            if(string.IsNullOrWhiteSpace(saveDirectory))
            {
                throw new ArgumentException("存档目录不能为空", nameof(saveDirectory));
            }
            if(string.IsNullOrWhiteSpace(fileExtension))
            {
                throw new ArgumentException("文件扩展名不能为空", nameof(fileExtension));
            }

            this.saveDirectory = saveDirectory;
            this.fileExtension = fileExtension.StartsWith(".") ? fileExtension : "." + fileExtension;
        }

        public bool HasSave(string saveName)
        {
            string relativePath = GetSavePath(saveName);
            return fileStorage.Exists(relativePath);
        }


        public T Load<T>(string saveName) where T : class, new()
        {
            if(TryLoad<T>(saveName,out T data))
            {
                return data;
            }
            return new T();
        }

        public bool TryLoad<T>(string saveName, out T data) where T : class, new()
        {
            data = null;
            string path = GetSavePath(saveName);
            if (!fileStorage.Exists(path))
            {
                return false;
            }

            try
            {
                byte[] bytes = fileStorage.Read(path);
                data = serializer.Deserialize<T>(bytes);
                return data != null;
            }
            catch
            {
                data = null;
                return false;
            }
        }

        public void Save<T>(string saveName, T data) where T : class, new()
        {
            if(data == null)
            {
                throw new ArgumentNullException(nameof(data), "要保存的数据不能为空");
            }
            string path = GetSavePath(saveName);
            byte[] bytes = serializer.Serialize(data);
            fileStorage.Write(path, bytes);
        }

        public void Delete(string saveName)
        {
            string path = GetSavePath(saveName);
            fileStorage.Delete(path);
        }

        private string GetSavePath(string saveName)
        {
            ValidateSaveName(saveName);
            string fileName = saveName.EndsWith(fileExtension, StringComparison.OrdinalIgnoreCase)
                ? saveName
                : saveName + fileExtension;
            return $"{saveDirectory}/{fileName}";
        }
        private static void ValidateSaveName(string saveName)
        {
            if (string.IsNullOrWhiteSpace(saveName))
            {
                throw new ArgumentException("存档名称不能为空", nameof(saveName));
            }
            if (saveName.Contains("/") || saveName.Contains("\\") || saveName.Contains(".."))
            {
                throw new ArgumentException("存档名称不能包含路径符号", nameof(saveName));
            }
        }
    }
}