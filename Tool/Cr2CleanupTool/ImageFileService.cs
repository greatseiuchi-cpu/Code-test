using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cr2CleanupTool
{
    public static class ImageFileService
    {
        /// <summary>
        /// 指定された2つのフォルダを比較し、対応するJPEGファイルが存在しないCR2ファイルのフルパスのリストを返します。
        /// </summary>
        /// <param name="cr2FolderPath">CR2ファイルが含まれるフォルダのパス。</param>
        /// <param name="jpegFolderPath">JPEGファイルが含まれるフォルダのパス。</param>
        /// <returns>一致しないCR2ファイルのパスのリスト。</returns>
        public static List<string> FindUnmatchedCr2Files(string cr2FolderPath, string jpegFolderPath)
        {
            // jpgとjpegの両方の拡張子を対象にする
            string[] jpegExtensions = { ".jpg", ".jpeg" };
            
            // JPEGフォルダからJPEGファイルのベース名（拡張子なし）をHashSetに格納する
            var jpegFiles = jpegExtensions.SelectMany(ext => Directory.GetFiles(jpegFolderPath, $"*{ext}", SearchOption.TopDirectoryOnly));
            var jpegBasenames = new HashSet<string>(jpegFiles.Select(file => Path.GetFileNameWithoutExtension(file)), StringComparer.OrdinalIgnoreCase);

            // CR2フォルダからCR2ファイルを取得
            var cr2Files = Directory.GetFiles(cr2FolderPath, "*.cr2", SearchOption.TopDirectoryOnly);

            // 対応するJPEGファイルが存在しないCR2ファイルを見つける
            var unmatchedCr2Files = new List<string>();
            foreach (var cr2File in cr2Files)
            {
                var cr2Basename = Path.GetFileNameWithoutExtension(cr2File);
                if (!jpegBasenames.Contains(cr2Basename))
                {
                    unmatchedCr2Files.Add(cr2File);
                }
            }

            return unmatchedCr2Files;
        }

        /// <summary>
        /// 指定されたファイルパスのリストにあるファイルをすべて削除します。
        /// </summary>
        /// <param name="filesToDelete">削除するファイルのフルパスのリスト。</param>
        public static void DeleteFiles(IEnumerable<string> filesToDelete)
        {
            foreach (var file in filesToDelete)
            {
                File.Delete(file);
            }
        }
    }
}