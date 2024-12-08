using Application.Photos;
using Application.Core;
using Application.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pccm.UnitTest.Notifications
{
    [TestFixture]
    public class AddHandlerTests
    {
       private Mock<IPhotoAccessor> _photoAccessorMock;
        private Add.Handler _handler;

        [SetUp]
        public void Setup()
        {
            _photoAccessorMock = new Mock<IPhotoAccessor>();
            _handler = new Add.Handler(_photoAccessorMock.Object);
        }

        [Test]
        public async Task Handle_ShouldReturnSuccess_WhenPhotoUploadIsSuccessful()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            var photoUploadResult = new PhotoUploadResult
            {
                PublicId = "test.jpg",
                Url = "http://example.com/test.jpg"
            };

            _photoAccessorMock.Setup(p => p.AddPhoto(mockFile.Object))
                .ReturnsAsync(photoUploadResult);

            var command = new Add.Command
            {
                File = mockFile.Object
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(photoUploadResult);
            _photoAccessorMock.Verify(p => p.AddPhoto(mockFile.Object), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldReturnFailure_WhenPhotoUploadFails()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            var errorMessage = "Photo upload failed";

            _photoAccessorMock.Setup(p => p.AddPhoto(mockFile.Object))
                .ThrowsAsync(new Exception(errorMessage));

            var command = new Add.Command
            {
                File = mockFile.Object
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Contain(errorMessage);
            _photoAccessorMock.Verify(p => p.AddPhoto(mockFile.Object), Times.Once);
        }
    }

}

