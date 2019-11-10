using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using SportsStore.Controllers;
using SportsStore.Models;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SportsStore.Tests
{
    public class AdminControllerTests
    {
        [Fact]
        public void Index_Contains_All_Products()
        {
            //Arrange - создание имитированного хранилища
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns((new Product[]
            {
                new Product { ProductID = 1, Name = "P1" },
                new Product { ProductID = 2, Name = "P2" },
                new Product { ProductID = 3, Name = "P3" }
            }).AsQueryable<Product>());

            //Arrange - создание контроллера
            AdminController target = new AdminController(mock.Object);

            //Act
            Product[] result = GetViewModel<IEnumerable<Product>>(target.Index())?.ToArray();

            //Assert
            Assert.Equal(3, result.Length);
            Assert.Equal("P1", result[0].Name);
            Assert.Equal("P2", result[1].Name);
            Assert.Equal("P3", result[2].Name);
        }

        [Fact]
        public void Can_Edit_Product()
        {
            //Arrange - создание имитированного хранилища
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns((new Product[]
            {
                new Product { ProductID = 1, Name = "P1" },
                new Product { ProductID = 2, Name = "P2" },
                new Product { ProductID = 3, Name = "P3" }
            }).AsQueryable<Product>());

            //Arrange - создание контроллера
            AdminController target = new AdminController(mock.Object);

            //Act
            Product p1 = GetViewModel<Product>(target.Edit(1));
            Product p2 = GetViewModel<Product>(target.Edit(2));
            Product p3 = GetViewModel<Product>(target.Edit(3));

            //Assert
            Assert.Equal(1, p1.ProductID);
            Assert.Equal(2, p2.ProductID);
            Assert.Equal(3, p3.ProductID);
        }

        [Fact]
        public void Cannot_Edit_Nonexistent_Product()
        {
            //Arrange - создание имитированного хранилища
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns((new Product[]
            {
                new Product { ProductID = 1, Name = "P1" },
                new Product { ProductID = 2, Name = "P2" },
                new Product { ProductID = 3, Name = "P3" }
            }).AsQueryable<Product>());

            //Arrange - создание контроллера
            AdminController target = new AdminController(mock.Object);

            //Act
            Product result = GetViewModel<Product>(target.Edit(4));

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public void Can_Save_Valid_Changes()
        {
            //Arrange - создание имитированного хранилища
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            //Arrange - создание имитированных временных данных
            Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();

            //Arrange - создание контроллера
            AdminController target = new AdminController(mock.Object)
            {
                TempData = tempData.Object
            };

            //Arrange - создание товара
            Product product = new Product { Name = "Test" };

            //Act - попытка сохранить товар
            IActionResult result = target.Edit(product).Result;

            //Assert - проверка того, что к хранилищу было произведено обращение
            mock.Verify(m => m.SaveProductAsync(product));

            //Assert - проверка, что типом результата является перенаправление
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", (result as RedirectToActionResult).ActionName);
        }

        [Fact]
        public void Cannot_Save_Invalid_Changes()
        {
            //Arrange - создание имитированного хранилища
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            //Arrange - создание контроллера
            AdminController target = new AdminController(mock.Object);

            //Arrange - создание товара
            Product product = new Product { Name = "Test" };

            //Arrange - добавление ошибки в состояние модели
            target.ModelState.AddModelError("error", "error");

            //Act - попытка сохранить товар
            IActionResult result = target.Edit(product).Result;

            //Assert - проверка того, что к хранилищу было произведено обращение
            mock.Verify(m => m.SaveProductAsync(It.IsAny<Product>()), Times.Never());

            //Assert - проверка типа результата метода
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Can_Delete_Valid_Products()
        {
            //Arrange - создание объекта Product
            Product prod = new Product { ProductID = 2, Name = "Test" };

            //Arrange - создание имитированного хранилища
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns((new Product[]
            {
                new Product { ProductID = 1, Name = "P1" },
                prod,
                new Product { ProductID = 3, Name = "P3" }
            }).AsQueryable<Product>());

            //Arrange - создание контроллера
            AdminController target = new AdminController(mock.Object);

            //Act - удаление товара
            target.Delete(prod.ProductID);

            //Assert - проверка того, что был вызван метод удаления
            //в хранилище с корректным объектом Product
            mock.Verify(m => m.DeleteProductAsync(prod.ProductID));
        }

        private T GetViewModel<T>(IActionResult result) where T : class
        {
            return (result as ViewResult)?.ViewData.Model as T;
        }
    }
}
