using AutoMapper;
using EnergyCo.Application.Commands;
using EnergyCo.Application.Common.Exceptions;
using EnergyCo.Application.Common.Interfaces;
using EnergyCo.Application.Common.Mapping;
using EnergyCo.Domain.Entities;
using EnergyCo.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace EnergyCo.Application.Tests;

public class CalculateDiscountCommandHandlerXUnitTests
{
    private readonly IMapper _mapper;

    public CalculateDiscountCommandHandlerXUnitTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>(), new NullLoggerFactory());
        _mapper = config.CreateMapper();
    }

    #region Handle Method Tests

    [Fact]
    public async Task Handle_WithValidCommandAndNoDiscounts_ShouldReturnCorrectResponse()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var transactionDate = new DateTime(2020, 01, 15);
        var productId = "PRD01";

        var products = new List<Product>
        {
            new Product
            {
                ProductId = productId,
                ProductName = "Fuel",
                Category = Category.Fuel,
                UnitPrice = 10m,
                DiscountPromotions = new List<DiscountPromotion>()
            }
        };

        var pointsPromotions = new List<PointsPromotion>
        {
            new PointsPromotion
            {
                PointsPromotionId = "PP001",
                PromotionName = "Fuel Promo",
                StartDate = new DateTime(2020, 01, 01),
                EndDate = new DateTime(2020, 01, 30),
                Category = Category.Fuel,
                PointsPerDollar = 2m
            }
        };

        var mockProductService = new Mock<IProductService>();
        mockProductService
            .Setup(c => c.GetProductDiscountPromotions(It.IsAny<List<string>>()))
            .ReturnsAsync(products);
        mockProductService
            .Setup(c => c.GetPointsPromotions())
            .ReturnsAsync(pointsPromotions);

        var command = new CalculateDiscountCommand
        {
            CustomerId = customerId,
            LoyaltyCard = "CARD123",
            TransactionDate = transactionDate,
            Basket = new List<CalculateDiscountCommandChild>
            {
                new CalculateDiscountCommandChild
                {
                    ProductId = productId,
                    Quantity = 5,
                    UnitPrice = 10m
                }
            }
        };

        var handler = new CalculateDiscountCommandHandler(_mapper, mockProductService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.CustomerId.Should().Be(customerId);
        result.LoyaltyCard.Should().Be("CARD123");
        result.TransactionDate.Should().Be(transactionDate);
        result.TotalAmount.Should().Be(50m);
        result.DiscountApplied.Should().Be(0m);
        result.GrandTotal.Should().Be(50m);
        result.PointsEarned.Should().Be(100m); // 5 * 10 * 2
    }

    [Fact]
    public async Task Handle_WithValidCommandAndApplicableDiscount_ShouldApplyDiscount()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var transactionDate = new DateTime(2020, 01, 15);
        var productId = "PRD01";

        var products = new List<Product>
        {
            new Product
            {
                ProductId = productId,
                ProductName = "Fuel",
                Category = Category.Fuel,
                UnitPrice = 10m,
                DiscountPromotions = new List<DiscountPromotion>
                {
                    new DiscountPromotion
                    {
                        DiscountPromotionId = "DP001",
                        PromotionName = "Fuel Discount",
                        StartDate = new DateTime(2020, 01, 01),
                        EndDate = new DateTime(2020, 01, 30),
                        DiscountPercent = 10m
                    }
                }
            }
        };

        var pointsPromotions = new List<PointsPromotion>
        {
            new PointsPromotion
            {
                PointsPromotionId = "PP001",
                PromotionName = "Fuel Promo",
                StartDate = new DateTime(2020, 01, 01),
                EndDate = new DateTime(2020, 01, 30),
                Category = Category.Fuel,
                PointsPerDollar = 1m
            }
        };

        var mockProductService = new Mock<IProductService>();
        mockProductService
            .Setup(c => c.GetProductDiscountPromotions(It.IsAny<List<string>>()))
            .ReturnsAsync(products);
        mockProductService
            .Setup(c => c.GetPointsPromotions())
            .ReturnsAsync(pointsPromotions);

        var command = new CalculateDiscountCommand
        {
            CustomerId = customerId,
            LoyaltyCard = "CARD123",
            TransactionDate = transactionDate,
            Basket = new List<CalculateDiscountCommandChild>
            {
                new CalculateDiscountCommandChild
                {
                    ProductId = productId,
                    Quantity = 5,
                    UnitPrice = 10m
                }
            }
        };

        var handler = new CalculateDiscountCommandHandler(_mapper, mockProductService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.TotalAmount.Should().Be(50m);
        result.DiscountApplied.Should().Be(5m); // 50 * 0.10
        result.GrandTotal.Should().Be(45m);
        result.PointsEarned.Should().Be(50m); // 50 * 1
    }

    [Fact]
    public async Task Handle_WithMultipleProductsAndMixedDiscounts_ShouldCalculateCorrectly()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var transactionDate = new DateTime(2020, 01, 15);

        var products = new List<Product>
        {
            new Product
            {
                ProductId = "PRD01",
                ProductName = "Fuel",
                Category = Category.Fuel,
                UnitPrice = 10m,
                DiscountPromotions = new List<DiscountPromotion>
                {
                    new DiscountPromotion
                    {
                        DiscountPromotionId = "DP001",
                        PromotionName = "Fuel Discount",
                        StartDate = new DateTime(2020, 01, 01),
                        EndDate = new DateTime(2020, 01, 30),
                        DiscountPercent = 20m
                    }
                }
            },
            new Product
            {
                ProductId = "PRD02",
                ProductName = "Snack",
                Category = Category.Shop,
                UnitPrice = 5m,
                DiscountPromotions = new List<DiscountPromotion>()
            }
        };

        var pointsPromotions = new List<PointsPromotion>
        {
            new PointsPromotion
            {
                PointsPromotionId = "PP001",
                PromotionName = "General Promo",
                StartDate = new DateTime(2020, 01, 01),
                EndDate = new DateTime(2020, 01, 30),
                Category = Category.Any,
                PointsPerDollar = 1m
            }
        };

        var mockProductService = new Mock<IProductService>();
        mockProductService
            .Setup(c => c.GetProductDiscountPromotions(It.IsAny<List<string>>()))
            .ReturnsAsync(products);
        mockProductService
            .Setup(c => c.GetPointsPromotions())
            .ReturnsAsync(pointsPromotions);

        var command = new CalculateDiscountCommand
        {
            CustomerId = customerId,
            LoyaltyCard = "CARD123",
            TransactionDate = transactionDate,
            Basket = new List<CalculateDiscountCommandChild>
            {
                new CalculateDiscountCommandChild { ProductId = "PRD01", Quantity = 2, UnitPrice = 10m },
                new CalculateDiscountCommandChild { ProductId = "PRD02", Quantity = 3, UnitPrice = 5m }
            }
        };

        var handler = new CalculateDiscountCommandHandler(_mapper, mockProductService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.TotalAmount.Should().Be(35m); // (2 * 10) + (3 * 5)
        result.DiscountApplied.Should().Be(4m); // (2 * 10) * 0.20
        result.GrandTotal.Should().Be(31m);
        result.PointsEarned.Should().Be(35m); // 35 * 1
    }

    [Fact]
    public async Task Handle_WithInvalidProductId_ShouldThrowNotFoundException()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var transactionDate = new DateTime(2020, 01, 15);

        var products = new List<Product>
        {
            new Product
            {
                ProductId = "PRD01",
                ProductName = "Fuel",
                Category = Category.Fuel,
                DiscountPromotions = new List<DiscountPromotion>()
            }
        };

        var mockProductService = new Mock<IProductService>();
        mockProductService
            .Setup(c => c.GetProductDiscountPromotions(It.IsAny<List<string>>()))
            .ReturnsAsync(products);

        var command = new CalculateDiscountCommand
        {
            CustomerId = customerId,
            LoyaltyCard = "CARD123",
            TransactionDate = transactionDate,
            Basket = new List<CalculateDiscountCommandChild>
            {
                new CalculateDiscountCommandChild
                {
                    ProductId = "INVALID_ID",
                    Quantity = 1,
                    UnitPrice = 10m
                }
            }
        };

        var handler = new CalculateDiscountCommandHandler(_mapper, mockProductService.Object);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithMultipleInvalidProductIds_ShouldThrowNotFoundExceptionWithDetailedMessage()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var transactionDate = new DateTime(2020, 01, 15);

        var products = new List<Product>
        {
            new Product
            {
                ProductId = "PRD01",
                ProductName = "Fuel",
                Category = Category.Fuel,
                DiscountPromotions = new List<DiscountPromotion>()
            }
        };

        var mockProductService = new Mock<IProductService>();
        mockProductService
            .Setup(c => c.GetProductDiscountPromotions(It.IsAny<List<string>>()))
            .ReturnsAsync(products);

        var command = new CalculateDiscountCommand
        {
            CustomerId = customerId,
            LoyaltyCard = "CARD123",
            TransactionDate = transactionDate,
            Basket = new List<CalculateDiscountCommandChild>
            {
                new CalculateDiscountCommandChild { ProductId = "INVALID1", Quantity = 1, UnitPrice = 10m },
                new CalculateDiscountCommandChild { ProductId = "INVALID2", Quantity = 1, UnitPrice = 10m }
            }
        };

        var handler = new CalculateDiscountCommandHandler(_mapper, mockProductService.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Contain("2 Product Id(s) not found");
    }

    [Fact]
    public async Task Handle_WithEmptyBasket_ShouldReturnResponseWithZeroAmounts()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var transactionDate = new DateTime(2020, 01, 15);

        var mockProductService = new Mock<IProductService>();
        mockProductService
            .Setup(c => c.GetProductDiscountPromotions(It.IsAny<List<string>>()))
            .ReturnsAsync(new List<Product>());
        mockProductService
            .Setup(c => c.GetPointsPromotions())
            .ReturnsAsync(new List<PointsPromotion>());

        var command = new CalculateDiscountCommand
        {
            CustomerId = customerId,
            LoyaltyCard = "CARD123",
            TransactionDate = transactionDate,
            Basket = new List<CalculateDiscountCommandChild>()
        };

        var handler = new CalculateDiscountCommandHandler(_mapper, mockProductService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.TotalAmount.Should().Be(0m);
        result.DiscountApplied.Should().Be(0m);
        result.GrandTotal.Should().Be(0m);
        result.PointsEarned.Should().Be(0m);
    }

    #endregion

    #region Points Calculation Tests

    [Theory]
    [InlineData("2020-01-05", 20, Category.Fuel)]
    [InlineData("2020-02-05", 30, Category.Fuel)]
    [InlineData("2020-03-01", 40, Category.Shop)]
    public async Task Handle_ShouldCalculatePointsCorrectly_GivenValidData(DateTime transactionDate, decimal expectedPoints, Category category)
    {
        // Arrange
        var products = new List<Product>
        {
            new Product
            {
                ProductId = "PRD01",
                ProductName = "Product",
                Category = category,
                DiscountPromotions = new List<DiscountPromotion>()
            }
        };

        var pointsPromotions = new List<PointsPromotion>
        {
            new PointsPromotion
            {
                PointsPromotionId = "PP001",
                PromotionName = "New Year Promo",
                StartDate = new DateTime(2020, 01, 01),
                EndDate = new DateTime(2020, 01, 30),
                Category = Category.Any,
                PointsPerDollar = 2m
            },
            new PointsPromotion
            {
                PointsPromotionId = "PP002",
                PromotionName = "Fuel Promo",
                StartDate = new DateTime(2020, 02, 05),
                EndDate = new DateTime(2020, 02, 15),
                Category = Category.Fuel,
                PointsPerDollar = 3m
            },
            new PointsPromotion
            {
                PointsPromotionId = "PP003",
                PromotionName = "Shop Promo",
                StartDate = new DateTime(2020, 03, 01),
                EndDate = new DateTime(2020, 03, 20),
                Category = Category.Shop,
                PointsPerDollar = 4m
            }
        };

        var mockProductService = new Mock<IProductService>();
        mockProductService
            .Setup(c => c.GetProductDiscountPromotions(It.IsAny<List<string>>()))
            .ReturnsAsync(products);
        mockProductService
            .Setup(c => c.GetPointsPromotions())
            .ReturnsAsync(pointsPromotions);

        var command = new CalculateDiscountCommand
        {
            CustomerId = Guid.NewGuid(),
            LoyaltyCard = "CARD123",
            TransactionDate = transactionDate,
            Basket = new List<CalculateDiscountCommandChild>
            {
                new CalculateDiscountCommandChild { ProductId = "PRD01", Quantity = 5, UnitPrice = 2m }
            }
        };

        var handler = new CalculateDiscountCommandHandler(_mapper, mockProductService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.PointsEarned.Should().Be(expectedPoints);
    }

    [Fact]
    public async Task Handle_WithNoApplicablePointsPromotion_ShouldReturn0Points()
    {
        // Arrange
        var transactionDate = new DateTime(2020, 06, 01); // Outside all promotion periods

        var products = new List<Product>
        {
            new Product
            {
                ProductId = "PRD01",
                ProductName = "Fuel",
                Category = Category.Fuel,
                DiscountPromotions = new List<DiscountPromotion>()
            }
        };

        var pointsPromotions = new List<PointsPromotion>
        {
            new PointsPromotion
            {
                PointsPromotionId = "PP001",
                PromotionName = "Fuel Promo",
                StartDate = new DateTime(2020, 02, 05),
                EndDate = new DateTime(2020, 02, 15),
                Category = Category.Fuel,
                PointsPerDollar = 3m
            }
        };

        var mockProductService = new Mock<IProductService>();
        mockProductService
            .Setup(c => c.GetProductDiscountPromotions(It.IsAny<List<string>>()))
            .ReturnsAsync(products);
        mockProductService
            .Setup(c => c.GetPointsPromotions())
            .ReturnsAsync(pointsPromotions);

        var command = new CalculateDiscountCommand
        {
            CustomerId = Guid.NewGuid(),
            LoyaltyCard = "CARD123",
            TransactionDate = transactionDate,
            Basket = new List<CalculateDiscountCommandChild>
            {
                new CalculateDiscountCommandChild { ProductId = "PRD01", Quantity = 5, UnitPrice = 2m }
            }
        };

        var handler = new CalculateDiscountCommandHandler(_mapper, mockProductService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.PointsEarned.Should().Be(0m);
    }

    [Fact]
    public async Task Handle_WithMultipleCategoriesAndPointsPromotions_ShouldCalculateCorrectly()
    {
        // Arrange
        var transactionDate = new DateTime(2020, 01, 15);

        var products = new List<Product>
        {
            new Product
            {
                ProductId = "PRD01",
                ProductName = "Fuel",
                Category = Category.Fuel,
                DiscountPromotions = new List<DiscountPromotion>()
            },
            new Product
            {
                ProductId = "PRD02",
                ProductName = "Snack",
                Category = Category.Shop,
                DiscountPromotions = new List<DiscountPromotion>()
            }
        };

        var pointsPromotions = new List<PointsPromotion>
        {
            new PointsPromotion
            {
                PointsPromotionId = "PP001",
                PromotionName = "General Promo",
                StartDate = new DateTime(2020, 01, 01),
                EndDate = new DateTime(2020, 01, 30),
                Category = Category.Any,
                PointsPerDollar = 1m
            },
            new PointsPromotion
            {
                PointsPromotionId = "PP002",
                PromotionName = "Fuel Promo",
                StartDate = new DateTime(2020, 01, 01),
                EndDate = new DateTime(2020, 01, 30),
                Category = Category.Fuel,
                PointsPerDollar = 3m
            }
        };

        var mockProductService = new Mock<IProductService>();
        mockProductService
            .Setup(c => c.GetProductDiscountPromotions(It.IsAny<List<string>>()))
            .ReturnsAsync(products);
        mockProductService
            .Setup(c => c.GetPointsPromotions())
            .ReturnsAsync(pointsPromotions);

        var command = new CalculateDiscountCommand
        {
            CustomerId = Guid.NewGuid(),
            LoyaltyCard = "CARD123",
            TransactionDate = transactionDate,
            Basket = new List<CalculateDiscountCommandChild>
            {
                new CalculateDiscountCommandChild { ProductId = "PRD01", Quantity = 2, UnitPrice = 10m }, // Fuel: 20, uses 3 points/dollar = 60
                new CalculateDiscountCommandChild { ProductId = "PRD02", Quantity = 3, UnitPrice = 5m }   // Shop: 15, uses 1 point/dollar = 15
            }
        };

        var handler = new CalculateDiscountCommandHandler(_mapper, mockProductService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.PointsEarned.Should().Be(75m); // 60 + 15
    }

    #endregion

    #region Discount Calculation Tests

    [Theory]
    [InlineData("2020-01-05", 2.0, Category.Fuel)]
    [InlineData("2020-02-21", 0.0, Category.Fuel)]
    [InlineData("2020-03-06", 1.5, Category.Fuel)]
    public async Task Handle_ShouldCalculateDiscountCorrectly_GivenValidData(DateTime transactionDate, decimal expectedDiscount, Category category)
    {
        // Arrange
        var products = new List<Product>
        {
            new Product
            {
                ProductId = "PRD02",
                ProductName = "Vortex 98",
                Category = category,
                DiscountPromotions = new List<DiscountPromotion>
                {
                    new DiscountPromotion
                    {
                        DiscountPromotionId = "DP001",
                        DiscountPercent = 20m,
                        PromotionName = "Fuel Discount Promo",
                        StartDate = new DateTime(2020, 01, 01),
                        EndDate = new DateTime(2020, 02, 15)
                    },
                    new DiscountPromotion
                    {
                        DiscountPromotionId = "DP002",
                        DiscountPercent = 15m,
                        PromotionName = "Happy Promo",
                        StartDate = new DateTime(2020, 03, 02),
                        EndDate = new DateTime(2020, 03, 20)
                    }
                }
            }
        };

        var pointsPromotions = new List<PointsPromotion>
        {
            new PointsPromotion
            {
                PointsPromotionId = "PP001",
                PromotionName = "New Year Promo",
                StartDate = new DateTime(2020, 01, 01),
                EndDate = new DateTime(2020, 01, 30),
                Category = Category.Any,
                PointsPerDollar = 2m
            }
        };

        var mockProductService = new Mock<IProductService>();
        mockProductService
            .Setup(c => c.GetProductDiscountPromotions(It.IsAny<List<string>>()))
            .ReturnsAsync(products);
        mockProductService
            .Setup(c => c.GetPointsPromotions())
            .ReturnsAsync(pointsPromotions);

        var command = new CalculateDiscountCommand
        {
            CustomerId = Guid.NewGuid(),
            LoyaltyCard = "CARD123",
            TransactionDate = transactionDate,
            Basket = new List<CalculateDiscountCommandChild>
            {
                new CalculateDiscountCommandChild { ProductId = "PRD02", Quantity = 5, UnitPrice = 2m }
            }
        };

        var handler = new CalculateDiscountCommandHandler(_mapper, mockProductService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.DiscountApplied.Should().Be(expectedDiscount);
    }

    [Fact]
    public async Task Handle_WithMultipleDiscountsApplicable_ShouldUseHighestDiscount()
    {
        // Arrange
        var transactionDate = new DateTime(2020, 01, 15);

        var products = new List<Product>
        {
            new Product
            {
                ProductId = "PRD01",
                ProductName = "Fuel",
                Category = Category.Fuel,
                DiscountPromotions = new List<DiscountPromotion>
                {
                    new DiscountPromotion
                    {
                        DiscountPromotionId = "DP001",
                        DiscountPercent = 10m,
                        PromotionName = "Discount 1",
                        StartDate = new DateTime(2020, 01, 01),
                        EndDate = new DateTime(2020, 01, 30)
                    },
                    new DiscountPromotion
                    {
                        DiscountPromotionId = "DP002",
                        DiscountPercent = 25m,
                        PromotionName = "Discount 2",
                        StartDate = new DateTime(2020, 01, 01),
                        EndDate = new DateTime(2020, 01, 30)
                    },
                    new DiscountPromotion
                    {
                        DiscountPromotionId = "DP003",
                        DiscountPercent = 5m,
                        PromotionName = "Discount 3",
                        StartDate = new DateTime(2020, 01, 01),
                        EndDate = new DateTime(2020, 01, 30)
                    }
                }
            }
        };

        var mockProductService = new Mock<IProductService>();
        mockProductService
            .Setup(c => c.GetProductDiscountPromotions(It.IsAny<List<string>>()))
            .ReturnsAsync(products);
        mockProductService
            .Setup(c => c.GetPointsPromotions())
            .ReturnsAsync(new List<PointsPromotion>());

        var command = new CalculateDiscountCommand
        {
            CustomerId = Guid.NewGuid(),
            LoyaltyCard = "CARD123",
            TransactionDate = transactionDate,
            Basket = new List<CalculateDiscountCommandChild>
            {
                new CalculateDiscountCommandChild { ProductId = "PRD01", Quantity = 10, UnitPrice = 10m }
            }
        };

        var handler = new CalculateDiscountCommandHandler(_mapper, mockProductService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.DiscountApplied.Should().Be(25m); // 100 * 0.25
    }

    [Fact]
    public async Task Handle_WithExpiredDiscount_ShouldNotApplyDiscount()
    {
        // Arrange
        var transactionDate = new DateTime(2020, 02, 20); // After discount end date

        var products = new List<Product>
        {
            new Product
            {
                ProductId = "PRD01",
                ProductName = "Fuel",
                Category = Category.Fuel,
                DiscountPromotions = new List<DiscountPromotion>
                {
                    new DiscountPromotion
                    {
                        DiscountPromotionId = "DP001",
                        DiscountPercent = 20m,
                        PromotionName = "Expired Discount",
                        StartDate = new DateTime(2020, 01, 01),
                        EndDate = new DateTime(2020, 02, 15)
                    }
                }
            }
        };

        var mockProductService = new Mock<IProductService>();
        mockProductService
            .Setup(c => c.GetProductDiscountPromotions(It.IsAny<List<string>>()))
            .ReturnsAsync(products);
        mockProductService
            .Setup(c => c.GetPointsPromotions())
            .ReturnsAsync(new List<PointsPromotion>());

        var command = new CalculateDiscountCommand
        {
            CustomerId = Guid.NewGuid(),
            LoyaltyCard = "CARD123",
            TransactionDate = transactionDate,
            Basket = new List<CalculateDiscountCommandChild>
            {
                new CalculateDiscountCommandChild { ProductId = "PRD01", Quantity = 5, UnitPrice = 10m }
            }
        };

        var handler = new CalculateDiscountCommandHandler(_mapper, mockProductService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.DiscountApplied.Should().Be(0m);
    }

    [Fact]
    public async Task Handle_WithUpcomingDiscount_ShouldNotApplyDiscount()
    {
        // Arrange
        var transactionDate = new DateTime(2019, 12, 31); // Before discount start date

        var products = new List<Product>
        {
            new Product
            {
                ProductId = "PRD01",
                ProductName = "Fuel",
                Category = Category.Fuel,
                DiscountPromotions = new List<DiscountPromotion>
                {
                    new DiscountPromotion
                    {
                        DiscountPromotionId = "DP001",
                        DiscountPercent = 20m,
                        PromotionName = "Future Discount",
                        StartDate = new DateTime(2020, 01, 01),
                        EndDate = new DateTime(2020, 02, 15)
                    }
                }
            }
        };

        var mockProductService = new Mock<IProductService>();
        mockProductService
            .Setup(c => c.GetProductDiscountPromotions(It.IsAny<List<string>>()))
            .ReturnsAsync(products);
        mockProductService
            .Setup(c => c.GetPointsPromotions())
            .ReturnsAsync(new List<PointsPromotion>());

        var command = new CalculateDiscountCommand
        {
            CustomerId = Guid.NewGuid(),
            LoyaltyCard = "CARD123",
            TransactionDate = transactionDate,
            Basket = new List<CalculateDiscountCommandChild>
            {
                new CalculateDiscountCommandChild { ProductId = "PRD01", Quantity = 5, UnitPrice = 10m }
            }
        };

        var handler = new CalculateDiscountCommandHandler(_mapper, mockProductService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.DiscountApplied.Should().Be(0m);
    }

    #endregion

    #region Total Amount Calculation Tests

    [Fact]
    public async Task Handle_ShouldCalculateTotalAmountCorrectly()
    {
        // Arrange
        var transactionDate = new DateTime(2020, 01, 15);

        var products = new List<Product>
        {
            new Product
            {
                ProductId = "PRD01",
                ProductName = "Product1",
                Category = Category.Fuel,
                DiscountPromotions = new List<DiscountPromotion>()
            },
            new Product
            {
                ProductId = "PRD02",
                ProductName = "Product2",
                Category = Category.Shop,
                DiscountPromotions = new List<DiscountPromotion>()
            }
        };

        var mockProductService = new Mock<IProductService>();
        mockProductService
            .Setup(c => c.GetProductDiscountPromotions(It.IsAny<List<string>>()))
            .ReturnsAsync(products);
        mockProductService
            .Setup(c => c.GetPointsPromotions())
            .ReturnsAsync(new List<PointsPromotion>());

        var command = new CalculateDiscountCommand
        {
            CustomerId = Guid.NewGuid(),
            LoyaltyCard = "CARD123",
            TransactionDate = transactionDate,
            Basket = new List<CalculateDiscountCommandChild>
            {
                new CalculateDiscountCommandChild { ProductId = "PRD01", Quantity = 2, UnitPrice = 15.5m },
                new CalculateDiscountCommandChild { ProductId = "PRD02", Quantity = 3, UnitPrice = 7.25m }
            }
        };

        var handler = new CalculateDiscountCommandHandler(_mapper, mockProductService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        var expectedTotal = (2 * 15.5m) + (3 * 7.25m); // 31 + 21.75 = 52.75
        result.TotalAmount.Should().Be(expectedTotal);
    }

    #endregion

    #region Grand Total Calculation Tests

    [Fact]
    public async Task Handle_ShouldCalculateGrandTotalCorrectly()
    {
        // Arrange
        var transactionDate = new DateTime(2020, 01, 15);

        var products = new List<Product>
        {
            new Product
            {
                ProductId = "PRD01",
                ProductName = "Fuel",
                Category = Category.Fuel,
                DiscountPromotions = new List<DiscountPromotion>
                {
                    new DiscountPromotion
                    {
                        DiscountPromotionId = "DP001",
                        DiscountPercent = 10m,
                        PromotionName = "Discount",
                        StartDate = new DateTime(2020, 01, 01),
                        EndDate = new DateTime(2020, 01, 30)
                    }
                }
            }
        };

        var mockProductService = new Mock<IProductService>();
        mockProductService
            .Setup(c => c.GetProductDiscountPromotions(It.IsAny<List<string>>()))
            .ReturnsAsync(products);
        mockProductService
            .Setup(c => c.GetPointsPromotions())
            .ReturnsAsync(new List<PointsPromotion>());

        var command = new CalculateDiscountCommand
        {
            CustomerId = Guid.NewGuid(),
            LoyaltyCard = "CARD123",
            TransactionDate = transactionDate,
            Basket = new List<CalculateDiscountCommandChild>
            {
                new CalculateDiscountCommandChild { ProductId = "PRD01", Quantity = 10, UnitPrice = 10m }
            }
        };

        var handler = new CalculateDiscountCommandHandler(_mapper, mockProductService.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.TotalAmount.Should().Be(100m);
        result.DiscountApplied.Should().Be(10m);
        result.GrandTotal.Should().Be(90m);
    }

    #endregion

    #region Service Integration Tests

    [Fact]
    public async Task Handle_ShouldCallGetProductDiscountPromotionsWithCorrectProductIds()
    {
        // Arrange
        var productIds = new List<string> { "PRD01", "PRD02" };
        var products = new List<Product>
        {
            new Product { ProductId = "PRD01", ProductName = "Product1", Category = Category.Fuel, DiscountPromotions = new List<DiscountPromotion>() },
            new Product { ProductId = "PRD02", ProductName = "Product2", Category = Category.Shop, DiscountPromotions = new List<DiscountPromotion>() }
        };

        var mockProductService = new Mock<IProductService>();
        mockProductService
            .Setup(c => c.GetProductDiscountPromotions(It.IsAny<List<string>>()))
            .ReturnsAsync(products);
        mockProductService
            .Setup(c => c.GetPointsPromotions())
            .ReturnsAsync(new List<PointsPromotion>());

        var command = new CalculateDiscountCommand
        {
            CustomerId = Guid.NewGuid(),
            LoyaltyCard = "CARD123",
            TransactionDate = new DateTime(2020, 01, 15),
            Basket = productIds.Select(id => new CalculateDiscountCommandChild { ProductId = id, Quantity = 1, UnitPrice = 10m }).ToList()
        };

        var handler = new CalculateDiscountCommandHandler(_mapper, mockProductService.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        mockProductService.Verify(
            c => c.GetProductDiscountPromotions(It.Is<List<string>>(list => list.Count == 2 && list.Contains("PRD01") && list.Contains("PRD02"))),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCallGetPointsPromotionsOnce()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { ProductId = "PRD01", ProductName = "Product1", Category = Category.Fuel, DiscountPromotions = new List<DiscountPromotion>() }
        };

        var mockProductService = new Mock<IProductService>();
        mockProductService
            .Setup(c => c.GetProductDiscountPromotions(It.IsAny<List<string>>()))
            .ReturnsAsync(products);
        mockProductService
            .Setup(c => c.GetPointsPromotions())
            .ReturnsAsync(new List<PointsPromotion>());

        var command = new CalculateDiscountCommand
        {
            CustomerId = Guid.NewGuid(),
            LoyaltyCard = "CARD123",
            TransactionDate = new DateTime(2020, 01, 15),
            Basket = new List<CalculateDiscountCommandChild> { new CalculateDiscountCommandChild { ProductId = "PRD01", Quantity = 1, UnitPrice = 10m } }
        };

        var handler = new CalculateDiscountCommandHandler(_mapper, mockProductService.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        mockProductService.Verify(c => c.GetPointsPromotions(), Times.Once);
    }

    #endregion
}
