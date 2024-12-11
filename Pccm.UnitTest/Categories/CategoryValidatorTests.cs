using Application.Handler.Categories;
using Domain.Entity;
using FluentValidation.TestHelper;

namespace Pccm.UnitTest.Categories
{
    [TestFixture]
    public class CategoryValidatorTests
    {
        private CategoryValidator _validator;

        [SetUp]
        public void SetUp()
        {
            // Khởi tạo CategoryValidator
            _validator = new CategoryValidator();
        }

        [Test]
        public void Validate_CategoryNameIsEmpty_ReturnsValidationError()
        {
            // Arrange
            var category = new Category
            {
                CategoryName = "" // Tên category trống
            };

            // Act
            var result = _validator.TestValidate(category);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.CategoryName)
                .WithErrorMessage("Category Name is required");
        }

        [Test]
        public void Validate_CategoryNameIsValid_NoValidationError()
        {
            // Arrange
            var category = new Category
            {
                CategoryName = "Valid Category Name" // Tên hợp lệ
            };

            // Act
            var result = _validator.TestValidate(category);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.CategoryName);
        }

        [Test]
        public void Validate_CategoryNameIsNull_ReturnsValidationError()
        {
            // Arrange
            var category = new Category
            {
                CategoryName = null // Tên category null
            };

            // Act
            var result = _validator.TestValidate(category);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.CategoryName)
                .WithErrorMessage("Category Name is required");
        }
    }
}