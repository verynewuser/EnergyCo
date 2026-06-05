using EnergyCo.Domain.Entities;
using EnergyCo.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EnergyCo.Infrastructure.Persistence;

public static class ModelBuilderExtensions
{
    /// <summary>
    /// Database Seed Data
    /// </summary>
    /// <param name="modelBuilder"></param>
    public static void Seed(this ModelBuilder modelBuilder)
    {
        // -- Add Points Promotions
        modelBuilder.Entity<PointsPromotion>().HasData(
                new PointsPromotion()
                {
                    PointsPromotionId = "PP001",
                    PromotionName = "New Year Promo",
                    StartDate = new DateTime(2020, 01, 01),
                    EndDate = new DateTime(2020, 01, 30),
                    Category = Category.Any,
                    PointsPerDollar = 2
                },
                new PointsPromotion()
                {
                    PointsPromotionId = "PP002",
                    PromotionName = "New Year Promo",
                    StartDate = new DateTime(2020, 02, 05),
                    EndDate = new DateTime(2020, 02, 15),
                    Category = Category.Fuel,
                    PointsPerDollar = 3
                },
                new PointsPromotion()
                {
                    PointsPromotionId = "PP003",
                    PromotionName = "Shop Promo",
                    StartDate = new DateTime(2020, 03, 01),
                    EndDate = new DateTime(2020, 03, 20),
                    Category = Category.Shop,
                    PointsPerDollar = 4
                }
            );

        // Discount Product
        Product product1 = new Product()
        {
            ProductId = "PRD01",
            ProductName = "Vortex 95",
            UnitPrice = (decimal)1.2,
            Category = Category.Fuel
        };
        Product product2 = new Product()
        {
            ProductId = "PRD02",
            ProductName = "Vortex 98",
            UnitPrice = (decimal)1.3,
            Category = Category.Fuel
        };




        // -- Add Products
        modelBuilder.Entity<Product>().HasData(
                product1,
                product2,
                new Product()
                {
                    ProductId = "PRD03",
                    ProductName = "Diesel",
                    UnitPrice = (decimal)1.1,
                    Category = Category.Fuel
                },
                new Product()
                {
                    ProductId = "PRD04",
                    ProductName = "Twix 55g",
                    UnitPrice = (decimal)2.3,
                    Category = Category.Shop
                },
                new Product()
                {
                    ProductId = "PRD05",
                    ProductName = "Mars 72g",
                    UnitPrice = (decimal)5.1,
                    Category = Category.Shop
                },
                new Product()
                {
                    ProductId = "PRD06",
                    ProductName = "SNICKERS 72G ",
                    UnitPrice = (decimal)3.4,
                    Category = Category.Shop
                },
                new Product()
                {
                    ProductId = "PRD07",
                    ProductName = "Bounty 3 63g",
                    UnitPrice = (decimal)6.9,
                    Category = Category.Shop
                },
                new Product()
                {
                    ProductId = "PRD08",
                    ProductName = "Snickers 50g",
                    UnitPrice = (decimal)4.0,
                    Category = Category.Shop
                }
            );

        //// -- Add Discount Promotions
        modelBuilder.Entity<DiscountPromotion>().HasData(
                new DiscountPromotion()
                {
                    DiscountPromotionId = "DP001",
                    DiscountPercent = 20,
                    PromotionName = "Fuel Discount Promo",
                    StartDate = new DateTime(2020, 01, 01),
                    EndDate = new DateTime(2020, 02, 15)                        

                },
                new DiscountPromotion()
                {
                    DiscountPromotionId = "DP002",
                    DiscountPercent = 15,
                    PromotionName = "Happy Promo",
                    StartDate = new DateTime(2020, 03, 02),
                    EndDate = new DateTime(2020, 03, 20)
                }
            );
    }
}
