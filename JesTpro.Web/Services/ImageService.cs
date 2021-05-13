// This file is part of JesTpro project.
//
// JesTpro is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (if needed) any later version.
//
// JesTpro has NO WARRANTY!! It is distributed for test, study or 
// personal environments. Any commercial distribution
// has no warranty! 
// See the GNU General Public License in root project folder  
// for more details or  see <http://www.gnu.org/licenses/>

using jt.jestpro.dal;
using jt.jestpro.dal.Entities;
using jt.jestpro.Helpers;
using jt.jestpro.Helpers.ExtensionMethods;
using jt.jestpro.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace jt.jestpro.Services
{
    public interface IImageService : ICRUDService<ImageDto, ImageFilterDto, ImageEditDto>
    {
        Task<ImageDto[]> UploadMulti(IFormFileCollection files, bool isDefault = false);
        Task<ImageDto> OverWrite(IFormFile file, Guid idImage);
        Task<ImageDto[]> SetDefault(ImageEditDto itemToEdit);
        Task Clone(List<Image> imgList, bool saveChanges = true);
    }
    public class ImageService : IImageService
    {
        MyDBContext _dbCtx;
        ILogger<ImageService> _logger;
        IOptions<AppSettings> _appSettings;
        public ImageService(MyDBContext dbCtx, IOptions<AppSettings> appSettings, ILogger<ImageService> logger)
        {
            _dbCtx = dbCtx;
            _logger = logger;
            _appSettings = appSettings;
        }

        public async Task<bool> Delete(Guid id)
        {
            _logger.LogDebug($"Calling delete image for id=[{id}]");
            var t = await this.GetInner(id);
            t.XDeleteDate = DateTime.UtcNow;
            _dbCtx.Images.Update(t);
            _dbCtx.SaveChanges();
            return true;
        }

        public async Task<ImageDto> Get(Guid id)
        {
            _logger.LogDebug($"Calling get image for id=[{id}]");
            var t = await GetInner(id);
            return t.ToDto();
        }

        private async Task<Image> GetInner(Guid id)
        {
            return await _dbCtx.Images.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ImageDto[]> GetList(ImageFilterDto filter)
        {
            _logger.LogDebug($"Calling getList image");

            IQueryable<Image> query = _dbCtx.Images;

            if (filter.Id != Guid.Empty)
            {
                query = query.Where(x => x.Id == filter.Id);
            }

            var result = await query.ToArrayAsync();
            return result.Select(x => x.ToDto()).ToArray();

        }

        public async Task<ImageDto> Save(ImageEditDto itemToEdit)
        {
            Image res;
            if (itemToEdit.Id != Guid.Empty)
            {
                _logger.LogDebug($"Calling Update image for id=[{itemToEdit.Id}]");
                //edit
                res = await this.GetInner(itemToEdit.Id);
                if (res == null)
                {
                    throw new NotFoundException($"Image with id={itemToEdit.Id} not exists!");
                }
                res.Path = itemToEdit.Path;
                _dbCtx.Images.Update(res);
                _dbCtx.SaveChanges();
            }
            else
            {
                //insert
                res = itemToEdit.ToEntity();
                res.Id = Guid.NewGuid();
                _logger.LogDebug($"Calling Insert image for id=[{res.Id}] (temp id, not created yet!)");
                await _dbCtx.Images.AddAsync(res);
                _dbCtx.SaveChanges();

            }
            return res.ToDto();
        }

        public async Task<ImageDto[]> UploadMulti(IFormFileCollection files, bool isDefault = false)
        {
            var res = new List<ImageDto>();
            if (files.Any(f => f.Length == 0))
            {
                throw new Exception("No file to be uploaded");
            }
            foreach (var file in files)
            {
                var img = await UploadImage(file,isDefault);
                res.Add(img);
            }
            return res.ToArray();
        }

        public async Task<ImageDto> OverWrite(IFormFile file, Guid idImage)
        {
            var res = await UploadImage(file, false, idImage);
            return res;
        }

        private async Task<ImageDto> UploadImage(IFormFile file, bool isDefault,  Guid? idImage = null)
        {
            Image img = new Image();
            if (idImage.HasValue)
            {
                img = await GetInner(idImage.Value);
            }
            else
            {
                img.Id = Guid.NewGuid();
                img.IsDefault = isDefault;
            }
            var pathToSave = Path.Combine(_appSettings.Value.ImagePath, img.Id.ToString());
            Directory.CreateDirectory(pathToSave);
            
            var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fullPath = Path.Combine(pathToSave, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            img.Path = fullPath;

            if (idImage.HasValue)
            {
                //immagine sovrascritta
                _dbCtx.Update(img);
            } 
            else
            {
                //nuova immagine
                await _dbCtx.AddAsync(img);
            }

            await _dbCtx.SaveChangesAsync();
            return img.ToDto();
        }

        public async Task<ImageDto[]> SetDefault(ImageEditDto itemToEdit)
        {
            Image[] imageList = new Image[] { };
                
            foreach(var image in imageList)
            {
                if (image.Id == itemToEdit.Id)
                {
                    image.IsDefault = true;
                } 
                else
                {
                    image.IsDefault = false;
                }
                
            }
            _dbCtx.UpdateRange(imageList);
            await _dbCtx.SaveChangesAsync();
            return imageList.Select(x => x.ToDto()).ToArray();
        }

        public async Task Clone(List<Image> imgList, bool saveChanges = true)
        {
            foreach (var img in imgList)
            {
                if (System.IO.File.Exists(img.Path))
                {
                    var newImg = new Image();
                    newImg.Id = Guid.NewGuid();
                    newImg.IsDefault = img.IsDefault;
                    newImg.Path = img.Path.Replace(img.Id.ToString(), newImg.Id.ToString());
                    var imgInfo = new System.IO.FileInfo(newImg.Path);
                    System.IO.Directory.CreateDirectory(imgInfo.DirectoryName);
                    System.IO.File.Copy(img.Path, newImg.Path, true);
                    //devo copiare fisicamente l'immagine
                    await _dbCtx.Images.AddAsync(newImg);
                }
                else
                {
                    _logger.LogWarning($"Unable to clone image: please check your phisycal img path [{img.Path}]!!");
                }

            }
            if (saveChanges)
                await _dbCtx.SaveChangesAsync();
        }

    }
}
