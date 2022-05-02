using Microsoft.AspNetCore.Mvc;
using Moq;
using SportsStore.Controllers;
using SportsStore.Models;
using SportsStore.Models.ViewModels;
using System.Linq;
using Xunit;

namespace SportsStore.Tests
{

    public class ProductControllerTests
    {

        [Fact]
        public void Can_Use_Repository()
        {
            // Arrange
            var mock = new Mock<IStoreRepository>();
            mock.Setup(m => m.Products).Returns((new Product[] {
                new Product {ProductId = 1, Name = "P1"},
                new Product {ProductId = 2, Name = "P2"}
            }).AsQueryable<Product>());

            var controller = new HomeController(mock.Object);

            // Act
            var result = controller.Index(null).ViewData.Model as ProductsListViewModel;

            // Assert

            var prodArray = result?.Products.ToArray();
            Assert.True(prodArray is { Length: 2 });
            Assert.Equal("P1", prodArray[0].Name);
            Assert.Equal("P2", prodArray[1].Name);
        }

        [Fact]
        public void Can_Paginate()
        {
            // Arrange
            var mock = new Mock<IStoreRepository>();
            mock.Setup(m => m.Products).Returns((new Product[] {
                new Product {ProductId = 1, Name = "P1"},
                new Product {ProductId = 2, Name = "P2"},
                new Product {ProductId = 3, Name = "P3"},
                new Product {ProductId = 4, Name = "P4"},
                new Product {ProductId = 5, Name = "P5"}
            }).AsQueryable<Product>());

            var controller = new HomeController(mock.Object)
            {
                PageSize = 3
            };

            // Act
            var result = controller.Index(null, 2).ViewData.Model as ProductsListViewModel;
            // Assert

            var prodArray = result?.Products.ToArray();
            Assert.True(prodArray is { Length: 2 });
            Assert.Equal("P4", prodArray[0].Name);
            Assert.Equal("P5", prodArray[1].Name);
        }

        [Fact]
        public void Can_Send_Pagination_View_Model()
        {

            // Arrange
            var mock = new Mock<IStoreRepository>();
            mock.Setup(m => m.Products).Returns((new Product[] {
                new Product {ProductId = 1, Name = "P1"},
                new Product {ProductId = 2, Name = "P2"},
                new Product {ProductId = 3, Name = "P3"},
                new Product {ProductId = 4, Name = "P4"},
                new Product {ProductId = 5, Name = "P5"}
            }).AsQueryable<Product>());

            // Arrange
            var controller =
                new HomeController(mock.Object) { PageSize = 3 };

            // Act
            var result = controller.Index(null, 2).ViewData.Model as ProductsListViewModel;
            // Assert

            var pageInfo = result?.PagingInfo;
            if (pageInfo == null)
            {
                return;
            }

            Assert.Equal(2, pageInfo.CurrentPage);
            Assert.Equal(3, pageInfo.ItemsPerPage);
            Assert.Equal(5, pageInfo.TotalItems);
            Assert.Equal(2, pageInfo.TotalPages);
        }

        [Fact]
        public void Generate_Category_Specific_Product_Count()
        {
            // Arrange
            var mock = new Mock<IStoreRepository>();
            mock.Setup(m => m.Products).Returns((new Product[] {
                new Product {ProductId = 1, Name = "P1", Category = "Cat1"},
                new Product {ProductId = 2, Name = "P2", Category = "Cat2"},
                new Product {ProductId = 3, Name = "P3", Category = "Cat1"},
                new Product {ProductId = 4, Name = "P4", Category = "Cat2"},
                new Product {ProductId = 5, Name = "P5", Category = "Cat3"}
            }).AsQueryable<Product>());
            var target = new HomeController(mock.Object)
            {
                PageSize = 3
            };
            static ProductsListViewModel GetModel(ViewResult result) =>
                result?.ViewData?.Model as ProductsListViewModel;

            // Action
            var res1 = GetModel(target.Index("Cat1"))?.PagingInfo.TotalItems;
            var res2 = GetModel(target.Index("Cat2"))?.PagingInfo.TotalItems;
            var res3 = GetModel(target.Index("Cat3"))?.PagingInfo.TotalItems;
            var resAll = GetModel(target.Index(null))?.PagingInfo.TotalItems;

            // Assert
            Assert.Equal(2, res1);
            Assert.Equal(2, res2);
            Assert.Equal(1, res3);
            Assert.Equal(5, resAll);
        }
    }
}