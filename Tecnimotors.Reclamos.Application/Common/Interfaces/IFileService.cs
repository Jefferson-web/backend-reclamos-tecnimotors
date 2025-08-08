using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Application.Common.Interfaces
{
    public interface IFileService
    {
        Task DeleteFileAsync(string path);
        void DeleteFile(string path);
        Task<string> SaveFileAsync(IFormFile file, string folder);
        Task<byte[]> GetFileAsync(string rutaCompleta);
        string GetContentType(string extension);
    }
}
