using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Moq;
using SportsStore.Controllers;
using SportsStore.Infrastructure;
using SportsStore.Models;
using SportsStore.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SportsStore.Tests
{
    public class PageLinkTagHelperTests
    {
        [Fact]
        public void CanGeneratePageLinks()
        {
            //Arrange
            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.SetupSequence(x => x.Action(It.IsAny<UrlActionContext>()))
                .Returns("Test/Page1")
                .Returns("Test/Page2")
                .Returns("Test/Page3"); ;
            var urlHelperFactory = new Mock<IUrlHelperFactory>();
            urlHelperFactory.Setup(f =>
                    f.GetUrlHelper(It.IsAny<ActionContext>()))
                .Returns(urlHelper.Object);
            var helper = new PageLinkTagHelper(urlHelperFactory.Object)
            {
                PageModel = new PagingInfo
                {
                    CurrentPage = 2,
                    TotalItems = 28,
                    ItemsPerPage = 10
                },
                PageAction = "Test"
            };
            var ctx = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<object, object>(), "");
            var content = new Mock<TagHelperContent>();
            var output = new TagHelperOutput("div", new TagHelperAttributeList(), (cache, encoder) => Task.FromResult(content.Object));
            // Act
            helper.Process(ctx, output);
            // Assert
            Assert.Equal(@"<a href=""Test/Page1"">1</a>"
                         + @"<a href=""Test/Page2"">2</a>"
                         + @"<a href=""Test/Page3"">3</a>",
                output.Content.GetContent());
        }
        [Fact]
        public void Can_Filter_Products()
        {
            // Arrange
            var mock = new Mock<IStoreRepository>();
            mock.Setup(p => p.Products).Returns((new Product[]
            {
                new Product { ProductId = 1, Name ="P1", Category = "Cat1"},
                new Product { ProductId = 2, Name ="P2", Category = "Cat2"},
                new Product { ProductId = 3, Name = "P3", Category = "Cat3"},
                new Product { ProductId = 4, Name = "P4", Category = "Cat4"},
                new Product { ProductId = 5, Name = "P5", Category = "Cat5"}
            }).AsQueryable<Product>);

            //Arrange - create a controller and make the page size 3 items
            var controller = new HomeController(mock.Object)
            {
                PageSize = 3
            };

            //Action
            var result = (controller.Index("Cat2", 1).ViewData.Model as ProductsListViewModel)?.Products.ToArray();

            //Assert
            if (result == null)
            {
                return;
            }

            Assert.Single(result);
            Assert.Equal("P2", result[0].Name);
            Assert.Equal("Cat2", result[0].Category);
        }
    }
}
