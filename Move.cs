using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SincronizadorLocal
{
    public class Move
    {
        private string appExe;
        private IniFile arquivoIni;

        public bool DirectoryCopy(string sourceDirName, string destDirName, bool isDirectoryMain, bool copySubDirs)
        {
            try
            {
                // informa se algum arquivo foi copiado
                bool atualizado = false;
                // informa se é o diretório principal
                if (isDirectoryMain)
                {
                    // lista o diretório onde o programa se encontra
                    destDirName = Directory.GetCurrentDirectory();
                    // diretório atual + file.ini
                    arquivoIni = new IniFile(destDirName + "\\Move.ini");
                    // recupera o valor da tag AppName
                    appExe = arquivoIni.IniReadValue("Updater", "AppName");
                    // recupera o valor da tag Folder
                    sourceDirName = arquivoIni.IniReadValue("Updater", "Folder");
                }

                // diretório de origem
                var dirSource = new DirectoryInfo(sourceDirName);
                // lista o subdiretórios
                var dirs = dirSource.GetDirectories();
                // diretório de destino
                var dirDest = new DirectoryInfo(destDirName);

                // Se o diretório de origem não existe, lançar uma exceção.
                if (!dirSource.Exists)
                {
                    throw new DirectoryNotFoundException(
                        "Diretório de origem não existe ou não pôde ser encontrado: "
                        + sourceDirName);
                }

                // lista os tipos especificados no arquivo Move.ini
                string[] fileType = arquivoIni.IniReadValue("Updater", "Type").Split('|');
                // lista somente os arquivos com os tipos passados no Move.ini
                List<FileInfo> strFiles = new List<FileInfo>();
                for (int i = 0; i < fileType.Length; i++)
                {
                    // Obter o conteúdo do diretório de arquivos para copiar .
                    strFiles.AddRange(dirSource.GetFiles("*." + fileType[i]));
                }

                foreach (var file in strFiles)
                {
                    // verifica se o arquivo de origem já existem no diretório de destino
                    if (File.Exists(destDirName + '\\' + file.Name))
                    {
                        // lista o md5 dos arquivos
                        string md5DirSource = listarMd5(sourceDirName + '\\' + file.Name);
                        string md5DirDest = listarMd5(destDirName + '\\' + file.Name);
                        // caso sejam iguais não atualizar
                        if (md5DirSource.Equals(md5DirDest))
                            continue;
                    }
                    // Cria o caminho para a nova cópia do arquivo.
                    var temppath = Path.Combine(destDirName, file.Name);
                    Console.WriteLine("Copiado " + file.Name);
                    // Copia o arquivo
                    file.CopyTo(temppath, true);
                    // um arquivo foi copiado
                    atualizado = true;
                }

                // Se copySubDirs é true, copiar os subdiretórios.
                if (!copySubDirs) return false;

                foreach (var subdir in dirs)
                {
                    // Cria os subdiretórios.
                    var temppath = Path.Combine(destDirName, subdir.Name);

                    // Copia os subdiretórios
                    DirectoryCopy(subdir.FullName, temppath, false, copySubDirs);
                }
                return atualizado;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (File.Exists(destDirName + '\\' + appExe))
                    Process.Start(destDirName + '\\' + appExe);
            }
        }

        public string listarMd5(string fileName)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(fileName))
                {
                    //return md5.ComputeHash(stream);
                    return Encoding.Default.GetString(md5.ComputeHash(stream));
                }
            }
        }
    }
}
