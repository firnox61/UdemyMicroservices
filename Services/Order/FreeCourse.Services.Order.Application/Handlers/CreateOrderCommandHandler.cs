﻿using FreeCourse.Services.Order.Application.Commands;
using FreeCourse.Services.Order.Application.Dtos;
using FreeCourse.Services.Order.Domain.OrderAggregate;
using FreeCourse.Services.Order.Infrastructure;
using FreeCourses.Shared.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeCourse.Services.Order.Application.Handlers
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Response<CreatedOrderDto>>
    {
        private readonly OrderDbContext _context;

        public CreateOrderCommandHandler(OrderDbContext context)
        {
            _context = context;
        }

        public async Task<Response<CreatedOrderDto>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {//burda önce newadres degerine request işlemindeki adress değerlerini atadık sonra
         //neworder a requestdeki buyer ıd ve eklenen newadres eklendi sonra
         //requestdeki orderıtems leri neworderdaki itemlere foreach ile ekliyoruz
         //async olarak ekleme metodumuzu çağırıyoruz
         //_contexe değişiklikleri kaydediyoruz
         //dünüş olarak success dondürüp createdOrdetDto nesnesini döndürüp orderID sini neworderId yapıyoruz
            var newAddress = new Address(request.Address.Province, request.Address.District, request.Address.Street,
                request.Address.ZipCode, request.Address.Line);
            Domain.OrderAggregate.Order newOrder= new Domain.OrderAggregate.Order(request.BuyerId, newAddress);
            request.OrderItems.ForEach(x =>
            {
                newOrder.AddOrderItem(x.ProductId, x.ProductName, x.Price, x.PictureUrl);
            });
            await _context.Orders.AddAsync(newOrder);
            await _context.SaveChangesAsync();
            return Response<CreatedOrderDto>.Success(new CreatedOrderDto { OrderId = newOrder.Id }, 200);

        }
    }
}
