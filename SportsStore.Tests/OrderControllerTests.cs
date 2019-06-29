using Microsoft.AspNetCore.Mvc;
using Moq;
using SportsStore.Controllers;
using SportsStore.Models;
using Xunit;

namespace SportsStore.Tests
{
    public class OrderControllerTests
    {
        [Fact]
        public void Cannot_Checkout_Empty_Cart()
        {
            //Arrange - создание имитированного хранилища заказов
            Mock<IOrderRepository> mock = new Mock<IOrderRepository>();

            //Arrange - создание пустой корзины
            Cart cart = new Cart();

            //Arrange - создание заказа
            Order order = new Order();

            //Arrange - создание экземпляра контроллера
            OrderController target = new OrderController(mock.Object, cart);

            //Act
            ViewResult result = target.Checkout(order) as ViewResult;

            //Assert - проверка, что заказ не был сохранен
            mock.Verify(m => m.SaveOrder(It.IsAny<Order>()), Times.Never);

            //Assert - проверка, что метод возвращает стандартное представление
            Assert.True(string.IsNullOrEmpty(result.ViewName));

            //Assert - проверка, что представлению передана недопустимая модель
            Assert.False(result.ViewData.ModelState.IsValid);
        }

        [Fact]
        public void Cannot_Checkout_Invalid_ShippingDetails()
        {
            //Arrange - создание имитированного хранилища заказов
            Mock<IOrderRepository> mock = new Mock<IOrderRepository>();

            //Arrange - создание корзины с одним элементом
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);

            //Arrange - создание экземпляра контроллера
            OrderController target = new OrderController(mock.Object, cart);

            //Arrange - добавление ошибки в модель
            target.ModelState.AddModelError("error", "error");

            //Act - попытка перехода к оплате
            ViewResult result = target.Checkout(new Order()) as ViewResult;

            //Assert - проверка, что заказ не был сохранен
            mock.Verify(m => m.SaveOrder(It.IsAny<Order>()), Times.Never);

            //Assert - проверка, что метод возвращает стандартное представление
            Assert.True(string.IsNullOrEmpty(result.ViewName));

            //Assert - проверка, что представлению передана недопустимая модель
            Assert.False(result.ViewData.ModelState.IsValid);
        }

        [Fact]
        public void Can_Checkout_And_Submit_Order()
        {
            //Arrange - создание имитированного хранилища заказов
            Mock<IOrderRepository> mock = new Mock<IOrderRepository>();

            //Arrange - создание корзины с одним элементом
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);

            //Arrange - создание экземпляра контроллера
            OrderController target = new OrderController(mock.Object, cart);

            //Act - попытка перехода к оплате
            RedirectToActionResult result = target.Checkout(new Order()) as RedirectToActionResult;

            //Assert - проверка, что заказ был сохранен
            mock.Verify(m => m.SaveOrder(It.IsAny<Order>()), Times.Once);

            //Assert - проверка, что метод перенаправляет на действие Completed
            Assert.Equal("Completed", result.ActionName);
        }
    }
}
