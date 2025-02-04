# 1. .NET 8 SDK'yı taban imaj olarak kullan
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# 2. Çalışma dizinini ayarla
WORKDIR /app

# 3. Portu aç (isteğe bağlı, runtime aşamasına taşınabilir)
EXPOSE 80

# 4. Proje dosyalarını kopyala
COPY Shared/FreeCourses.Shared/*.csproj Shared/FreeCourses.Shared/
#CLASS LİBRARY
COPY Services/Order/FreeCourse.Services.Order.Application/*.csproj Services/Order/FreeCourse.Services.Order.Application/
COPY Services/Order/FreeCourse.Services.Order.Domain/*.csproj Services/Order/FreeCourse.Services.Order.Domain/
COPY Services/Order/FreeCourse.Services.Order.Domain.Core/*.csproj Services/Order/FreeCourse.Services.Order.Domain.Core/
COPY Services/Order/FreeCourse.Services.Order.Infrastructure/*.csproj Services/Order/FreeCourse.Services.Order.Infrastructure/



COPY Services/Order/FreeCourse.Services.Order.API/*.csproj Services/Order/FreeCourse.Services.Order.API/

# 5. Bağımlılıkları geri yükle
RUN dotnet restore Services/Order/FreeCourse.Services.Order.API/*.csproj

# 6. Tüm dosyaları kopyala ve projeyi yayınla
COPY . .
RUN dotnet publish Services/Order/FreeCourse.Services.Order.API/*.csproj -c Release -o out

# 7. .NET Runtime imajını kullan
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# 8. Çalışma dizinini ayarla
WORKDIR /app

# 9. Yayınlanan dosyaları kopyala
COPY --from=build /app/out .

# 10. Uygulamayı çalıştır
ENTRYPOINT ["dotnet", "FreeCourse.Services.Order.API.dll"]
