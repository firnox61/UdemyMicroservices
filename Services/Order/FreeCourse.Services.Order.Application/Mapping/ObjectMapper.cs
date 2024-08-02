using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeCourse.Services.Order.Application.Mapping
{
    public static class ObjectMapper
    {//Lazy istendiği zaman initialize edilemsini sağlıyor
        private static readonly Lazy<IMapper> lazy = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CustomMapping>();
            });
            return config.CreateMapper();
        });

        //Ben mapperi çağırana kadar yukarıdaki kodlar çalışmayacak demek
        public static IMapper Mapper=> lazy.Value;
        
    }
}
