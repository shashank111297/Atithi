﻿using Atithi.Web.Context;
using Atithi.Web.Models.Domain;
using Atithi.Web.Models.DTO;
using Atithi.Web.Services.Interface;
using Microsoft.EntityFrameworkCore.Storage;

namespace Atithi.Web.Services
{
    public class OrderService : IOrderService
    {
        private readonly AtithiDbContext _atithiDbContext;

        public OrderService(AtithiDbContext atithiDbContext) 
        {
            _atithiDbContext = atithiDbContext;
        }

        public Task<MenuDTO> GetOrderByRoomId(int RoomId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> PlaceOrder(OrderDetailsDTO orderDetails)
        {
            var orderId = Guid.NewGuid();
            var order = new Order
            {
                OrderId = orderId,
                IsDelivered = false,
                OrderDate = DateTime.Now,
                RoomId = orderDetails.RoomId
            };

            var orderItems = new List<OrderItem>();
            foreach (var item in orderDetails.OrderItems)
            {
                var items = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    MenuItemId = item.MenuItemId,
                    Quantity = item.Quantity,
                    OrderId = orderId,
                };
                orderItems.Add(items);
            }

            using (IDbContextTransaction transaction = await _atithiDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    await _atithiDbContext.Order.AddAsync(order);
                    await _atithiDbContext.OrderItem.AddRangeAsync(orderItems);
                    await _atithiDbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            return true;
        }
    }
}