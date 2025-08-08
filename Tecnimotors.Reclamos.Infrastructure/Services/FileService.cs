using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Application.Common.Interfaces;
using Tecnimotors.Reclamos.Domain.Models;

namespace Tecnimotors.Reclamos.Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly FileStorageOptions _options;
        public FileService(IOptions<FileStorageOptions> options)
        {
            _options = options.Value;
        }

        public void DeleteFile(string path)
        {
            if (File.Exists(path)) 
            {
                File.Delete(path);
            }
        }

        public async Task DeleteFileAsync(string path)
        {
            await Task.Run(() => DeleteFile(path));
        }

        public string GetContentType(string extension)
        {
            return extension.ToLower() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".txt" => "text/plain",
                ".csv" => "text/csv",
                _ => "application/octet-stream"
            };
        }

        public async Task<byte[]> GetFileAsync(string rutaCompleta)
        {
            try
            {
                if (string.IsNullOrEmpty(rutaCompleta))
                    throw new ArgumentException("La ruta del archivo no puede estar vacía", nameof(rutaCompleta));

                if (!File.Exists(rutaCompleta))
                {
                    throw new FileNotFoundException($"No se pudo encontrar el archivo en: {rutaCompleta}");
                }

                return await File.ReadAllBytesAsync(rutaCompleta);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> SaveFileAsync(IFormFile file, string folder)
        {
            string uploadPath = Path.Combine(_options.BasePath, folder);
            Directory.CreateDirectory(uploadPath);

            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            string filePath = Path.Combine(uploadPath, fileName);

            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fs);
            }

            return filePath;
        }
    }
}
