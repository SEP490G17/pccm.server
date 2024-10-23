using Application.Core;
using Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Photos;

public class Add
{
    public class Command : IRequest<Result<PhotoUploadResult>>
    {
        public IFormFile File { get; set; }
    }

    public class Handler(IPhotoAccessor _photoAccessor) : IRequestHandler<Command, Result<PhotoUploadResult>>
    {
        public async Task<Result<PhotoUploadResult>> Handle(Command request, CancellationToken cancellationToken)
        {
            try
            {
                var photoUploadResult = await _photoAccessor.AddPhoto(request.File);
                return Result<PhotoUploadResult>.Success(photoUploadResult);
            }
            catch (Exception ex)
            {
                return Result<PhotoUploadResult>.Failure($"{ex.Message}");

            }
        }
    }


}
