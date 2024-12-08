using Application.DTOs;
using Application.Handler.CourtClusters;
using FluentAssertions;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace Pccm.UnitTest.CourtClusters
{
   [TestFixture]
    public class CourtClusterValidatorTests
    {
        private CourtClusterValidator _validator;

        [SetUp]
        public void SetUp()
        {
            // Khởi tạo CourtClusterValidator
            _validator = new CourtClusterValidator();
        }

        [Test]
        public void Validate_TitleIsEmpty_ReturnsValidationError()
        {
            // Arrange
            var inputDto = new CourtClustersInputDto
            {
                Title = "" // Tên cụm sân trống
            };

            // Act
            var result = _validator.TestValidate(inputDto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Title)
                .WithErrorMessage("Court cluster name is required");
        }

        [Test]
        public void Validate_TitleIsNull_ReturnsValidationError()
        {
            // Arrange
            var inputDto = new CourtClustersInputDto
            {
                Title = null // Tên cụm sân null
            };

            // Act
            var result = _validator.TestValidate(inputDto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Title)
                .WithErrorMessage("Court cluster name is required");
        }

        [Test]
        public void Validate_TitleIsValid_NoValidationError()
        {
            // Arrange
            var inputDto = new CourtClustersInputDto
            {
                Title = "Valid Court Cluster Name" // Tên hợp lệ
            };

            // Act
            var result = _validator.TestValidate(inputDto);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Title);
        }
    }
}