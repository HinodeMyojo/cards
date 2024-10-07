﻿using CardsServer.BLL.Dto.Image;
using CardsServer.BLL.Infrastructure.Result;
using Microsoft.AspNetCore.Http;

namespace CardsServer.BLL.Services
{
    public interface IImageService
    {
        Task<Result> UploadImage(CreateImage info, CancellationToken cancellationToken);
    }
}