# xUnit Test Suite for CalculateDiscountCommandHandler

## Overview
Comprehensive xUnit test suite for the `CalculateDiscountCommandHandler` class containing 21 tests organized into 6 logical categories.

## Test Categories and Details

### 1. Handle Method Tests (6 tests)
Tests core functionality of the Handle method with various scenarios:
- **Handle_WithValidCommandAndNoDiscounts_ShouldReturnCorrectResponse**: Validates correct response structure and calculations when no discounts are applicable
- **Handle_WithValidCommandAndApplicableDiscount_ShouldApplyDiscount**: Verifies discount application when promotions are active
- **Handle_WithMultipleProductsAndMixedDiscounts_ShouldCalculateCorrectly**: Tests multi-product baskets with varied discount eligibility
- **Handle_WithInvalidProductId_ShouldThrowNotFoundException**: Validates exception throwing for non-existent products
- **Handle_WithMultipleInvalidProductIds_ShouldThrowNotFoundExceptionWithDetailedMessage**: Verifies error messages contain count and IDs of invalid products
- **Handle_WithEmptyBasket_ShouldReturnResponseWithZeroAmounts**: Tests edge case of empty shopping basket

### 2. Points Calculation Tests (4 tests)
Tests the points earning logic:
- **Handle_ShouldCalculatePointsCorrectly_GivenValidData** (Theory with 3 data sets): Tests category-specific points multipliers
- **Handle_WithNoApplicablePointsPromotion_ShouldReturn0Points**: Validates behavior when transaction is outside promotion periods
- **Handle_WithMultipleCategoriesAndPointsPromotions_ShouldCalculateCorrectly**: Tests points aggregation across multiple product categories

### 3. Discount Calculation Tests (5 tests)
Tests the discount logic:
- **Handle_ShouldCalculateDiscountCorrectly_GivenValidData** (Theory with 3 data sets): Tests various discount percentage scenarios
- **Handle_WithMultipleDiscountsApplicable_ShouldUseHighestDiscount**: Verifies the handler applies the highest available discount
- **Handle_WithExpiredDiscount_ShouldNotApplyDiscount**: Tests that expired promotions don't apply discounts
- **Handle_WithUpcomingDiscount_ShouldNotApplyDiscount**: Tests that future promotions don't apply discounts

### 4. Total Amount Calculation Tests (1 test)
- **Handle_ShouldCalculateTotalAmountCorrectly**: Verifies accurate calculation of total transaction amount

### 5. Grand Total Calculation Tests (1 test)
- **Handle_ShouldCalculateGrandTotalCorrectly**: Tests the final amount after discount application

### 6. Service Integration Tests (2 tests)
Tests integration with IProductService:
- **Handle_ShouldCallGetProductDiscountPromotionsWithCorrectProductIds**: Verifies correct product IDs are passed to service
- **Handle_ShouldCallGetPointsPromotionsOnce**: Validates service method invocation count

## Test Statistics
- **Total Tests**: 21
- **Passing Tests**: 21
- **Failing Tests**: 0
- **Test Duration**: ~2.5 seconds
- **Coverage**: Handle method, discount calculation, points calculation, validation, and service integration

## Test Data Characteristics
- Uses Moq for mocking IProductService
- Employs xUnit [Theory] attribute with [InlineData] for parametrized tests
- Includes AutoMapper configuration with MappingProfile
- Uses FluentAssertions for readable assertions
- Tests real-world scenarios including:
  - Multiple products with different categories
  - Time-dependent promotions (start/end dates)
  - Multiple applicable discounts (selects highest)
  - Invalid product IDs
  - Empty baskets
  - Decimal precision handling

## Key Testing Patterns
1. **Arrange-Act-Assert**: All tests follow AAA pattern
2. **Mocking**: Isolates handler from external dependencies
3. **Theory Tests**: Parametrized tests for multiple scenarios
4. **Exception Testing**: Uses Assert.ThrowsAsync for validation
5. **Service Verification**: Uses Moq.Verify for method call verification

## Build and Run Instructions
```bash
# Restore packages
dotnet restore EnergyCo.Application.Tests

# Run all tests
dotnet test EnergyCo.Application.Tests

# Run specific test class
dotnet test EnergyCo.Application.Tests --filter "CalculateDiscountCommandHandlerXUnitTests"

# Run with verbose output
dotnet test EnergyCo.Application.Tests -v normal
```

## Framework and Dependencies
- **xUnit**: v2.8.1 - Modern test framework for .NET
- **Moq**: v4.20.72 - Mock object library
- **FluentAssertions**: v8.10.0 - Assertion library for readable tests
- **AutoMapper**: v16.1.1 - Object mapping
- **Microsoft.EntityFrameworkCore**: v10.0.8
- **Microsoft.NET.Test.Sdk**: v17.11.0 - Test SDK for .NET

## File Location
`EnergyCo.Application.Tests\CalculateDiscountCommandHandlerXUnitTests.cs`
