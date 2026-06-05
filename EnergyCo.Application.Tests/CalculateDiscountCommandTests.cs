using AutoMapper;
using EnergyCo.Application.Commands;
using EnergyCo.Application.Common.Exceptions;
using EnergyCo.Application.Common.Interfaces;
using EnergyCo.Application.Common.Mapping;
using EnergyCo.Domain.Entities;
using EnergyCo.Domain.Enums;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace EnergyCo.Application.Tests;

public class CalculateDiscountCommandTests
{
    private IMapper _mapper;

    [SetUp]
    public void SetUp()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>(), new NullLoggerFactory());
        _mapper = config.CreateMapper();
    }

    [Test]
    public async Task CalculateDiscountCommand_ShouldRaise_NotFoundException_GivenProductID_NotExist()
    {

        var mockSet = new Mock<DbSet<Product>>();
        var products = new List<Product>
            {
                new Product { ProductId = "PRD01", ProductName = "Fuel", Category = Category.Fuel },
            };

        var mockProductService = new Mock<IProductService>();
        mockProductService.Setup(c => c.GetProductDiscountPromotions(It.IsAny<List<string>>())).ReturnsAsync(products);

        var command = new CalculateDiscountCommand
        {
            CustomerId = Guid.NewGuid(),
            TransactionDate = new DateTime(2020, 01, 6),
            Basket = new List<CalculateDiscountCommandChild>
                        {
                            new CalculateDiscountCommandChild(){ ProductId = "abc", Quantity = 5, UnitPrice = 2 }
                        }

        };
        var applicationDbContext = new Mock<IApplicationDbContext>();

        var mediator = new Mock<IMediator>();

        CalculateDiscountCommandHandler handler = new CalculateDiscountCommandHandler(_mapper, mockProductService.Object);

        await handler.Awaiting(m => m.Handle(command, new CancellationToken())).Should().ThrowAsync<NotFoundException>();

    }

    [TestCase("2020-01-05", 20, Category.Fuel)]
    [TestCase("2020-02-05", 30, Category.Fuel)]
    [TestCase("2020-03-01", 40, Category.Shop)]
    public async Task CalculateDiscountCommand_Should_CalulatePoints_Given_ValidData(DateTime transactionDate,
                                                                                                        decimal expectedPoints,
                                                                                                        Category category)
    {
        var mockSet = new Mock<DbSet<Product>>();
        var products = new List<Product>
            {
                new Product { ProductId = "PRD01", ProductName = "Fuel", Category = category },
            };

        var pointsPromotions = new List<PointsPromotion>
            {
                new PointsPromotion { PointsPromotionId = "PP001",
                                      PromotionName = "New Year Promo",
                                      StartDate = new DateTime(2020,01,01),
                                      EndDate = new DateTime(2020,01,30),
                                      Category = Category.Any,
                                      PointsPerDollar = 2},
                new PointsPromotion { PointsPromotionId = "PP002",
                                      PromotionName = "Fuel Promo",
                                      StartDate = new DateTime(2020,02,05),
                                      EndDate = new DateTime(2020,02,15),
                                      Category = Category.Fuel,
                                      PointsPerDollar = 3},
                new PointsPromotion { PointsPromotionId = "PP003",
                                      PromotionName = "Shop Promo",
                                      StartDate = new DateTime(2020,03,01),
                                      EndDate = new DateTime(2020,03,20),
                                      Category = Category.Shop,
                                      PointsPerDollar = 4}
            };

        var mockProductService = new Mock<IProductService>();
        mockProductService.Setup(c => c.GetProductDiscountPromotions(It.IsAny<List<string>>())).ReturnsAsync(products);
        mockProductService.Setup(c => c.GetPointsPromotions()).ReturnsAsync(pointsPromotions);

        var command = new CalculateDiscountCommand
        {
            CustomerId = Guid.NewGuid(),
            TransactionDate = transactionDate,
            Basket = new List<CalculateDiscountCommandChild>
                        {
                            new CalculateDiscountCommandChild(){ ProductId = "PRD01", Quantity = 5, UnitPrice = 2 }
                        }

        };

        var mediator = new Mock<IMediator>();
        CalculateDiscountCommandHandler handler = new CalculateDiscountCommandHandler(_mapper, mockProductService.Object);

        var result = await handler.Handle(command, new CancellationToken());
        result.PointsEarned.Should().Be(expectedPoints);
    }


    [TestCase("2020-01-05", 2.0, Category.Fuel)]
    [TestCase("2020-02-21", 0.0, Category.Fuel)]
    [TestCase("2020-03-06", 1.5, Category.Fuel)]
    public async Task CalculateDiscountCommand_Should_CalulateDiscount_Given_ValidData(DateTime transactionDate,
                                                                                                decimal expectedPoints,
                                                                                                Category category)
    {
        var mockSet = new Mock<DbSet<Product>>();
        var products = new List<Product>
            {
                new Product { ProductId = "PRD02",
                              ProductName = "Vortex 98",
                              Category = category,
                              DiscountPromotions = new List<DiscountPromotion> {
                                                    new DiscountPromotion() { DiscountPromotionId = "DP001",
                                                        DiscountPercent = 20,
                                                        PromotionName = "Fuel Discount Promo",
                                                        StartDate = new DateTime(2020,01,01),
                                                        EndDate = new DateTime(2020,02,15)
                                                          },
                                                     new DiscountPromotion() { DiscountPromotionId = "DP002",
                                                        DiscountPercent = 15,
                                                        PromotionName = "Happy Promo",
                                                        StartDate = new DateTime(2020,03,02),
                                                        EndDate = new DateTime(2020,03,20)
                                                          }}
                                },
            };

        var pointsPromotions = new List<PointsPromotion>
            {
                new PointsPromotion { PointsPromotionId = "PP001",
                                      PromotionName = "New Year Promo",
                                      StartDate = new DateTime(2020,01,01),
                                      EndDate = new DateTime(2020,01,30),
                                      Category = Category.Any,
                                      PointsPerDollar = 2},
                new PointsPromotion { PointsPromotionId = "PP002",
                                      PromotionName = "Fuel Promo",
                                      StartDate = new DateTime(2020,02,05),
                                      EndDate = new DateTime(2020,02,15),
                                      Category = Category.Fuel,
                                      PointsPerDollar = 3},
                new PointsPromotion { PointsPromotionId = "PP003",
                                      PromotionName = "Shop Promo",
                                      StartDate = new DateTime(2020,03,01),
                                      EndDate = new DateTime(2020,03,20),
                                      Category = Category.Shop,
                                      PointsPerDollar = 4}
            };

        var mockProductService = new Mock<IProductService>();
        mockProductService.Setup(c => c.GetProductDiscountPromotions(It.IsAny<List<string>>())).ReturnsAsync(products);
        mockProductService.Setup(c => c.GetPointsPromotions()).ReturnsAsync(pointsPromotions);

        var command = new CalculateDiscountCommand
        {
            CustomerId = Guid.NewGuid(),
            TransactionDate = transactionDate,
            Basket = new List<CalculateDiscountCommandChild>
                        {
                            new CalculateDiscountCommandChild(){ ProductId = "PRD02", Quantity = 5, UnitPrice = 2 }
                        }

        };

        var mediator = new Mock<IMediator>();
        CalculateDiscountCommandHandler handler = new CalculateDiscountCommandHandler(_mapper, mockProductService.Object);

        var result = await handler.Handle(command, new CancellationToken());
        result.DiscountApplied.Should().Be(expectedPoints);
    }
}